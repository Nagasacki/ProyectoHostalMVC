using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProyectoHostalMVC.Models;
using System.Data;

namespace ProyectoHostalMVC.Controllers
{
    public class ReportesController : Controller
    {
        private readonly IConfiguration _config;

        public ReportesController(IConfiguration config)
        {
            _config = config;
        }


        public IActionResult Index(
            DateTime? fechaInicio,
            DateTime? fechaFin)
        {
            ReporteViewModel modelo =
                new ReporteViewModel();


            // Si no mandamos fechas,
            // mostramos los últimos 30 días.

            modelo.FechaInicio =
                fechaInicio ?? DateTime.Today.AddDays(-30);

            modelo.FechaFin =
                fechaFin ?? DateTime.Today;


            using (SqlConnection cn =
                new SqlConnection(
                    _config["ConnectionStrings:cn"]))
            {
                cn.Open();


                // ====================================
                // RESUMEN
                // ====================================

                SqlCommand cmdResumen =
                    new SqlCommand(
                        "dbo.sp_ReporteResumen",
                        cn);

                cmdResumen.CommandType =
                    CommandType.StoredProcedure;

                cmdResumen.Parameters.AddWithValue(
                    "@FechaInicio",
                    modelo.FechaInicio);

                cmdResumen.Parameters.AddWithValue(
                    "@FechaFin",
                    modelo.FechaFin);


                using (SqlDataReader dr =
                    cmdResumen.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        modelo.Resumen.TotalReservas =
                            Convert.ToInt32(
                                dr["TotalReservas"]);

                        modelo.Resumen.ReservasFinalizadas =
                            Convert.ToInt32(
                                dr["ReservasFinalizadas"]);

                        modelo.Resumen.ReservasCanceladas =
                            Convert.ToInt32(
                                dr["ReservasCanceladas"]);

                        modelo.Resumen.TotalIngresos =
                            Convert.ToDecimal(
                                dr["TotalIngresos"]);
                    }
                }


                // ====================================
                // RESERVAS
                // ====================================

                SqlCommand cmdReservas =
                    new SqlCommand(
                        "dbo.sp_ReporteReservas",
                        cn);

                cmdReservas.CommandType =
                    CommandType.StoredProcedure;

                cmdReservas.Parameters.AddWithValue(
                    "@FechaInicio",
                    modelo.FechaInicio);

                cmdReservas.Parameters.AddWithValue(
                    "@FechaFin",
                    modelo.FechaFin);


                using (SqlDataReader dr =
                    cmdReservas.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        ReporteReserva reserva =
                            new ReporteReserva();

                        reserva.IdReserva =
                            Convert.ToInt32(
                                dr["IdReserva"]);

                        reserva.NombreCliente =
                            dr["NombreCliente"].ToString();

                        reserva.NumeroHabitacion =
                            dr["NumeroHabitacion"].ToString();

                        reserva.TipoHabitacion =
                            dr["TipoHabitacion"].ToString();

                        reserva.FechaEntrada =
                            Convert.ToDateTime(
                                dr["FechaEntrada"]);

                        reserva.FechaSalida =
                            Convert.ToDateTime(
                                dr["FechaSalida"]);

                        reserva.CantidadDias =
                            Convert.ToInt32(
                                dr["CantidadDias"]);

                        reserva.PrecioDia =
                            Convert.ToDecimal(
                                dr["PrecioDia"]);

                        reserva.Total =
                            Convert.ToDecimal(
                                dr["Total"]);

                        reserva.Estado =
                            dr["Estado"].ToString();

                        modelo.Reservas.Add(reserva);
                    }
                }


                // ====================================
                // HABITACIONES
                // ====================================

                SqlCommand cmdHabitaciones =
                    new SqlCommand(
                        "dbo.sp_ReporteHabitaciones",
                        cn);

                cmdHabitaciones.CommandType =
                    CommandType.StoredProcedure;


                using (SqlDataReader dr =
                    cmdHabitaciones.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Habitacion habitacion =
                            new Habitacion();

                        habitacion.IdHabitacion =
                            Convert.ToInt32(
                                dr["IdHabitacion"]);

                        habitacion.Numero =
                            dr["Numero"].ToString();

                        habitacion.Tipo =
                            dr["Tipo"].ToString();

                        habitacion.Precio =
                            Convert.ToDecimal(
                                dr["Precio"]);

                        habitacion.Estado =
                            dr["Estado"].ToString();

                        modelo.Habitaciones.Add(habitacion);
                    }
                }
            }


            return View(modelo);
        }
    }
}