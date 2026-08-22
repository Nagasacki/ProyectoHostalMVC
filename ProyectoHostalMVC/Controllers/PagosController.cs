using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProyectoHostalMVC.Models;
using System.Data;

namespace ProyectoHostalMVC.Controllers
{
    [Authorize]
    public class PagosController : Controller
    {
        private readonly IConfiguration _config;

        public PagosController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Index()
        {
            List<Pago> pagos = new();
            using SqlConnection cn = new(_config["ConnectionStrings:cn"]);
            SqlCommand cmd = new("dbo.sp_ListarPagos", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@IdReserva", DBNull.Value);
            cn.Open();
            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read()) pagos.Add(MapearPago(dr));
            return View(pagos);
        }

        [HttpGet]
        public IActionResult Create(int idReserva)
        {
            Pago pago = new() { IdReserva = idReserva };
            if (!CargarReserva(idReserva)) return NotFound();
            return View(pago);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Pago pago)
        {
            if (!ModelState.IsValid)
            {
                CargarReserva(pago.IdReserva);
                return View(pago);
            }

            try
            {
                using SqlConnection cn = new(_config["ConnectionStrings:cn"]);
                SqlCommand cmd = new("dbo.sp_RegistrarPago", cn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@IdReserva", pago.IdReserva);
                cmd.Parameters.AddWithValue("@Monto", pago.Monto);
                cmd.Parameters.AddWithValue("@Metodo", pago.Metodo);
                cmd.Parameters.AddWithValue("@Tipo", pago.Tipo);
                cmd.Parameters.AddWithValue("@NumeroOperacion", (object?)pago.NumeroOperacion ?? DBNull.Value);
                cn.Open();
                cmd.ExecuteNonQuery();

                TempData["SweetAlert_Title"] = "Pago registrado";
                TempData["SweetAlert_Text"] = "El abono se agregó al historial de la reserva.";
                TempData["SweetAlert_Type"] = "success";
                return RedirectToAction("Details", "Reservas", new { id = pago.IdReserva });
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                CargarReserva(pago.IdReserva);
                return View(pago);
            }
        }

        private bool CargarReserva(int idReserva)
        {
            using SqlConnection cn = new(_config["ConnectionStrings:cn"]);
            SqlCommand cmd = new("dbo.sp_BuscarReserva", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@IdReserva", idReserva);
            cn.Open();
            using SqlDataReader dr = cmd.ExecuteReader();
            if (!dr.Read()) return false;
            decimal total = Convert.ToDecimal(dr["Total"]);
            decimal pagado = Convert.ToDecimal(dr["MontoPagado"]);
            ViewBag.Reserva = $"Reserva #{idReserva} · {dr["NombreCliente"]} · Hab. {dr["NumeroHabitacion"]}";
            ViewBag.Saldo = Math.Max(0, total - pagado);
            return true;
        }

        private static Pago MapearPago(SqlDataReader dr)
        {
            return new Pago
            {
                IdPago = Convert.ToInt32(dr["IdPago"]),
                IdReserva = Convert.ToInt32(dr["IdReserva"]),
                FechaPago = Convert.ToDateTime(dr["FechaPago"]),
                Monto = Convert.ToDecimal(dr["Monto"]),
                Metodo = dr["Metodo"].ToString() ?? string.Empty,
                Tipo = dr["Tipo"].ToString() ?? string.Empty,
                NumeroOperacion = dr["NumeroOperacion"]?.ToString(),
                NombreCliente = dr["NombreCliente"]?.ToString(),
                NumeroHabitacion = dr["NumeroHabitacion"]?.ToString()
            };
        }
    }
}
