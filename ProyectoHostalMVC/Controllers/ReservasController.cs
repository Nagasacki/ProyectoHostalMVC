using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProyectoHostalMVC.Models;
using System.Data;

namespace ProyectoHostalMVC.Controllers
{
    [Authorize]
    public class ReservasController : Controller
    {
        private readonly IConfiguration _config;

        public ReservasController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Index(int pagina = 1, string? busqueda = null)
        {
            List<Reserva> lista = new();
            using SqlConnection cn = new(_config["ConnectionStrings:cn"]);
            SqlCommand cmd = new("dbo.sp_ListarReservas", cn) { CommandType = CommandType.StoredProcedure };
            cn.Open();
            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read()) lista.Add(MapearReserva(dr));

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                lista = lista.Where(r =>
                    (r.NombreCliente?.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (r.NumeroHabitacion?.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    r.Estado.Contains(busqueda, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return View(PaginacionRespuesta<Reserva>.Crear(lista, pagina, 6, busqueda));
        }

        [HttpGet]
        public IActionResult Create()
        {
            Reserva reserva = new()
            {
                FechaEntrada = DateTime.Today,
                FechaSalida = DateTime.Today.AddDays(1)
            };
            CargarDatosFormulario(reserva.FechaEntrada, reserva.FechaSalida);
            return View(reserva);
        }

        [HttpGet]
        public IActionResult HabitacionesDisponiblesJson(DateTime fechaEntrada, DateTime fechaSalida)
        {
            if (fechaSalida <= fechaEntrada)
                return Json(new { success = false, message = "La salida debe ser posterior a la entrada." });

            var habitaciones = ObtenerHabitacionesDisponibles(fechaEntrada, fechaSalida)
                .Select(h => new { h.IdHabitacion, h.Numero, h.Tipo, h.Precio });
            return Json(new { success = true, data = habitaciones });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Reserva reserva)
        {
            if (reserva.FechaSalida <= reserva.FechaEntrada)
                ModelState.AddModelError("FechaSalida", "La fecha de salida debe ser posterior a la fecha de entrada.");

            if (!ModelState.IsValid)
            {
                CargarDatosFormulario(reserva.FechaEntrada, reserva.FechaSalida);
                return View(reserva);
            }

            try
            {
                using SqlConnection cn = new(_config["ConnectionStrings:cn"]);
                cn.Open();
                decimal precio = ObtenerPrecioHabitacion(cn, reserva.IdHabitacion);
                int dias = (reserva.FechaSalida.Date - reserva.FechaEntrada.Date).Days;

                SqlCommand cmd = new("dbo.sp_RegistrarReserva", cn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@IdCliente", reserva.IdCliente);
                cmd.Parameters.AddWithValue("@IdHabitacion", reserva.IdHabitacion);
                cmd.Parameters.AddWithValue("@FechaEntrada", reserva.FechaEntrada.Date);
                cmd.Parameters.AddWithValue("@FechaSalida", reserva.FechaSalida.Date);
                cmd.Parameters.AddWithValue("@CantidadDias", dias);
                cmd.Parameters.AddWithValue("@PrecioDia", precio);
                cmd.Parameters.AddWithValue("@Total", dias * precio);
                cmd.ExecuteNonQuery();

                MostrarMensaje("Reserva confirmada", "La habitación quedó separada para las fechas elegidas.", "success");
                return RedirectToAction("Index");
            }
            catch (Exception ex) when (ex is SqlException || ex is InvalidOperationException)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                CargarDatosFormulario(reserva.FechaEntrada, reserva.FechaSalida);
                return View(reserva);
            }
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            Reserva? reserva = ObtenerReserva(id);
            if (reserva == null) return NotFound();
            return View(new ReservaDetalleViewModel { Reserva = reserva, Pagos = ObtenerPagos(id) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckIn(int id) => EjecutarAccion("dbo.sp_CheckInReserva", id,
            "Check-in registrado", "El huésped ingresó y la habitación está ocupada.");

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckOut(int id) => EjecutarAccion("dbo.sp_CheckOutReserva", id,
            "Check-out registrado", "La habitación quedó pendiente de limpieza.");

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Cancelar(int id) => EjecutarAccion("dbo.sp_CancelarReserva", id,
            "Reserva cancelada", "La habitación volvió a quedar libre para esas fechas.");

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NoPresentado(int id) => EjecutarAccion("dbo.sp_NoPresentadoReserva", id,
            "No presentado", "La reserva fue cerrada por inasistencia.");

        private IActionResult EjecutarAccion(string procedimiento, int id, string titulo, string texto)
        {
            try
            {
                using SqlConnection cn = new(_config["ConnectionStrings:cn"]);
                SqlCommand cmd = new(procedimiento, cn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@IdReserva", id);
                cn.Open();
                cmd.ExecuteNonQuery();
                MostrarMensaje(titulo, texto, "success");
            }
            catch (SqlException ex)
            {
                MostrarMensaje("No se pudo completar", ex.Message, "error");
            }
            return RedirectToAction("Details", new { id });
        }

        private void CargarDatosFormulario(DateTime entrada, DateTime salida)
        {
            List<Cliente> clientes = new();
            using (SqlConnection cn = new(_config["ConnectionStrings:cn"]))
            {
                SqlCommand cmd = new("dbo.sp_ListarClientes", cn) { CommandType = CommandType.StoredProcedure };
                cn.Open();
                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    clientes.Add(new Cliente
                    {
                        IdCliente = Convert.ToInt32(dr["IdCliente"]),
                        Dni = dr["Dni"].ToString() ?? string.Empty,
                        Nombre = dr["Nombre"].ToString() ?? string.Empty
                    });
                }
            }
            ViewBag.Clientes = clientes;
            ViewBag.Habitaciones = salida > entrada ? ObtenerHabitacionesDisponibles(entrada, salida) : new List<Habitacion>();
        }

        private List<Habitacion> ObtenerHabitacionesDisponibles(DateTime entrada, DateTime salida)
        {
            List<Habitacion> lista = new();
            using SqlConnection cn = new(_config["ConnectionStrings:cn"]);
            SqlCommand cmd = new("dbo.sp_ListarHabitacionesDisponiblesPorFechas", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@FechaEntrada", entrada.Date);
            cmd.Parameters.AddWithValue("@FechaSalida", salida.Date);
            cn.Open();
            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new Habitacion
                {
                    IdHabitacion = Convert.ToInt32(dr["IdHabitacion"]),
                    Numero = dr["Numero"].ToString() ?? string.Empty,
                    Tipo = dr["Tipo"].ToString() ?? string.Empty,
                    Precio = Convert.ToDecimal(dr["Precio"]),
                    Estado = dr["Estado"].ToString() ?? string.Empty
                });
            }
            return lista;
        }

        private static decimal ObtenerPrecioHabitacion(SqlConnection cn, int idHabitacion)
        {
            SqlCommand cmd = new("dbo.sp_BuscarHabitacion", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@IdHabitacion", idHabitacion);
            using SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read()) return Convert.ToDecimal(dr["Precio"]);
            throw new InvalidOperationException("No se encontró la habitación seleccionada.");
        }

        private Reserva? ObtenerReserva(int id)
        {
            using SqlConnection cn = new(_config["ConnectionStrings:cn"]);
            SqlCommand cmd = new("dbo.sp_BuscarReserva", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@IdReserva", id);
            cn.Open();
            using SqlDataReader dr = cmd.ExecuteReader();
            return dr.Read() ? MapearReserva(dr) : null;
        }

        private List<Pago> ObtenerPagos(int idReserva)
        {
            List<Pago> lista = new();
            using SqlConnection cn = new(_config["ConnectionStrings:cn"]);
            SqlCommand cmd = new("dbo.sp_ListarPagos", cn) { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@IdReserva", idReserva);
            cn.Open();
            using SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                lista.Add(new Pago
                {
                    IdPago = Convert.ToInt32(dr["IdPago"]),
                    IdReserva = Convert.ToInt32(dr["IdReserva"]),
                    FechaPago = Convert.ToDateTime(dr["FechaPago"]),
                    Monto = Convert.ToDecimal(dr["Monto"]),
                    Metodo = dr["Metodo"].ToString() ?? string.Empty,
                    Tipo = dr["Tipo"].ToString() ?? string.Empty,
                    NumeroOperacion = dr["NumeroOperacion"]?.ToString()
                });
            }
            return lista;
        }

        private static Reserva MapearReserva(SqlDataReader dr)
        {
            return new Reserva
            {
                IdReserva = Convert.ToInt32(dr["IdReserva"]),
                IdCliente = Convert.ToInt32(dr["IdCliente"]),
                NombreCliente = dr["NombreCliente"].ToString(),
                IdHabitacion = Convert.ToInt32(dr["IdHabitacion"]),
                NumeroHabitacion = dr["NumeroHabitacion"].ToString(),
                TipoHabitacion = dr["TipoHabitacion"].ToString(),
                FechaEntrada = Convert.ToDateTime(dr["FechaEntrada"]),
                FechaSalida = Convert.ToDateTime(dr["FechaSalida"]),
                CantidadDias = Convert.ToInt32(dr["CantidadDias"]),
                PrecioDia = Convert.ToDecimal(dr["PrecioDia"]),
                Total = Convert.ToDecimal(dr["Total"]),
                Estado = dr["Estado"].ToString() ?? string.Empty,
                FechaRegistro = Convert.ToDateTime(dr["FechaRegistro"]),
                FechaCheckInReal = dr["FechaCheckInReal"] == DBNull.Value ? null : Convert.ToDateTime(dr["FechaCheckInReal"]),
                FechaCheckOutReal = dr["FechaCheckOutReal"] == DBNull.Value ? null : Convert.ToDateTime(dr["FechaCheckOutReal"]),
                MontoPagado = Convert.ToDecimal(dr["MontoPagado"])
            };
        }

        private void MostrarMensaje(string titulo, string texto, string tipo)
        {
            TempData["SweetAlert_Title"] = titulo;
            TempData["SweetAlert_Text"] = texto;
            TempData["SweetAlert_Type"] = tipo;
        }
    }
}
