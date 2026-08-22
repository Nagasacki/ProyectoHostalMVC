using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProyectoHostalMVC.Models;
using System.Data;

namespace ProyectoHostalMVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;

        public HomeController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Index()
        {
            Dashboard dashboard = new();
            using SqlConnection cn = new(_config["ConnectionStrings:cn"]);
            cn.Open();

            SqlCommand resumen = new("dbo.sp_DashboardResumen", cn) { CommandType = CommandType.StoredProcedure };
            using (SqlDataReader dr = resumen.ExecuteReader())
            {
                if (dr.Read())
                {
                    dashboard.LlegadasHoy = Convert.ToInt32(dr["LlegadasHoy"]);
                    dashboard.SalidasHoy = Convert.ToInt32(dr["SalidasHoy"]);
                    dashboard.HuespedesAlojados = Convert.ToInt32(dr["HuespedesAlojados"]);
                    dashboard.HabitacionesLimpieza = Convert.ToInt32(dr["HabitacionesLimpieza"]);
                    dashboard.TotalHabitaciones = Convert.ToInt32(dr["TotalHabitaciones"]);
                    dashboard.HabitacionesDisponibles = Convert.ToInt32(dr["HabitacionesDisponibles"]);
                    dashboard.HabitacionesOcupadas = Convert.ToInt32(dr["HabitacionesOcupadas"]);
                    dashboard.HabitacionesMantenimiento = Convert.ToInt32(dr["HabitacionesMantenimiento"]);
                    dashboard.CobradoHoy = Convert.ToDecimal(dr["CobradoHoy"]);
                }
            }

            SqlCommand movimientos = new("dbo.sp_DashboardMovimientosHoy", cn) { CommandType = CommandType.StoredProcedure };
            using (SqlDataReader dr = movimientos.ExecuteReader())
            {
                while (dr.Read())
                {
                    dashboard.MovimientosHoy.Add(new DashboardMovimiento
                    {
                        IdReserva = Convert.ToInt32(dr["IdReserva"]),
                        Tipo = dr["Tipo"].ToString() ?? string.Empty,
                        Huesped = dr["Huesped"].ToString() ?? string.Empty,
                        Habitacion = dr["Habitacion"].ToString() ?? string.Empty,
                        FechaProgramada = Convert.ToDateTime(dr["FechaProgramada"]),
                        Estado = dr["Estado"].ToString() ?? string.Empty
                    });
                }
            }

            SqlCommand pendientes = new("dbo.sp_DashboardPagosPendientes", cn) { CommandType = CommandType.StoredProcedure };
            using (SqlDataReader dr = pendientes.ExecuteReader())
            {
                while (dr.Read())
                {
                    dashboard.PagosPendientes.Add(new DashboardPagoPendiente
                    {
                        IdReserva = Convert.ToInt32(dr["IdReserva"]),
                        Huesped = dr["Huesped"].ToString() ?? string.Empty,
                        Habitacion = dr["Habitacion"].ToString() ?? string.Empty,
                        Saldo = Convert.ToDecimal(dr["Saldo"])
                    });
                }
            }

            return View(dashboard);
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
