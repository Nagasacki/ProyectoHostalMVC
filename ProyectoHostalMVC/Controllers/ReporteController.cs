using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProyectoHostalMVC.Models;
using System.Data;

namespace ProyectoHostalMVC.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ReportesController : Controller
    {
        private readonly IConfiguration _config;

        public ReportesController(IConfiguration config)
        {
            _config = config;
        }

        public IActionResult Index(DateTime? fechaInicio, DateTime? fechaFin, int pagina = 1)
        {
            ReporteViewModel modelo = new ReporteViewModel();

            // Rango predeterminado: últimos 30 días
            modelo.FechaInicio = fechaInicio ?? DateTime.Today.AddDays(-30);
            modelo.FechaFin = fechaFin ?? DateTime.Today;

            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                cn.Open();

                // 1. Resumen Métricas
                SqlCommand cmdResumen = new SqlCommand("dbo.sp_ReporteResumen", cn);
                cmdResumen.CommandType = CommandType.StoredProcedure;
                cmdResumen.Parameters.AddWithValue("@FechaInicio", modelo.FechaInicio);
                cmdResumen.Parameters.AddWithValue("@FechaFin", modelo.FechaFin);

                using (SqlDataReader dr = cmdResumen.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        modelo.Resumen.TotalReservas = dr["TotalReservas"] != DBNull.Value ? Convert.ToInt32(dr["TotalReservas"]) : 0;
                        modelo.Resumen.ReservasFinalizadas = dr["ReservasFinalizadas"] != DBNull.Value ? Convert.ToInt32(dr["ReservasFinalizadas"]) : 0;
                        modelo.Resumen.ReservasCanceladas = dr["ReservasCanceladas"] != DBNull.Value ? Convert.ToInt32(dr["ReservasCanceladas"]) : 0;
                        modelo.Resumen.TotalIngresos = dr["TotalIngresos"] != DBNull.Value ? Convert.ToDecimal(dr["TotalIngresos"]) : 0m;
                        modelo.Resumen.TotalFacturado = dr["TotalFacturado"] != DBNull.Value ? Convert.ToDecimal(dr["TotalFacturado"]) : 0m;
                        modelo.Resumen.SaldoPendiente = dr["SaldoPendiente"] != DBNull.Value ? Convert.ToDecimal(dr["SaldoPendiente"]) : 0m;
                    }
                }

                // 2. Detalle de Reservas
                SqlCommand cmdReservas = new SqlCommand("dbo.sp_ReporteReservas", cn);
                cmdReservas.CommandType = CommandType.StoredProcedure;
                cmdReservas.Parameters.AddWithValue("@FechaInicio", modelo.FechaInicio);
                cmdReservas.Parameters.AddWithValue("@FechaFin", modelo.FechaFin);

                using (SqlDataReader dr = cmdReservas.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        modelo.Reservas.Add(new ReporteReserva
                        {
                            IdReserva = Convert.ToInt32(dr["IdReserva"]),
                            NombreCliente = dr["NombreCliente"] != DBNull.Value ? dr["NombreCliente"].ToString() : string.Empty,
                            NumeroHabitacion = dr["NumeroHabitacion"] != DBNull.Value ? dr["NumeroHabitacion"].ToString() : string.Empty,
                            TipoHabitacion = dr["TipoHabitacion"] != DBNull.Value ? dr["TipoHabitacion"].ToString() : string.Empty,
                            FechaEntrada = Convert.ToDateTime(dr["FechaEntrada"]),
                            FechaSalida = Convert.ToDateTime(dr["FechaSalida"]),
                            CantidadDias = Convert.ToInt32(dr["CantidadDias"]),
                            PrecioDia = dr["PrecioDia"] != DBNull.Value ? Convert.ToDecimal(dr["PrecioDia"]) : 0m,
                            Total = dr["Total"] != DBNull.Value ? Convert.ToDecimal(dr["Total"]) : 0m,
                            MontoPagado = dr["MontoPagado"] != DBNull.Value ? Convert.ToDecimal(dr["MontoPagado"]) : 0m,
                            Saldo = dr["Saldo"] != DBNull.Value ? Convert.ToDecimal(dr["Saldo"]) : 0m,
                            Estado = dr["Estado"] != DBNull.Value ? dr["Estado"].ToString() : string.Empty
                        });
                    }
                }

                // 3. Estado de Habitaciones
                SqlCommand cmdHabitaciones = new SqlCommand("dbo.sp_ReporteHabitaciones", cn);
                cmdHabitaciones.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader dr = cmdHabitaciones.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        modelo.Habitaciones.Add(new Habitacion
                        {
                            IdHabitacion = Convert.ToInt32(dr["IdHabitacion"]),
                            Numero = dr["Numero"] != DBNull.Value ? dr["Numero"].ToString()! : string.Empty,
                            Tipo = dr["Tipo"] != DBNull.Value ? dr["Tipo"].ToString()! : string.Empty,
                            Precio = dr["Precio"] != DBNull.Value ? Convert.ToDecimal(dr["Precio"]) : 0m,
                            Estado = dr["Estado"] != DBNull.Value ? dr["Estado"].ToString()! : string.Empty
                        });
                    }
                }

                SqlCommand cmdPagos = new SqlCommand("dbo.sp_ReportePagosPorMetodo", cn);
                cmdPagos.CommandType = CommandType.StoredProcedure;
                cmdPagos.Parameters.AddWithValue("@FechaInicio", modelo.FechaInicio);
                cmdPagos.Parameters.AddWithValue("@FechaFin", modelo.FechaFin);

                using (SqlDataReader dr = cmdPagos.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        modelo.PagosPorMetodo.Add(new PagoPorMetodo
                        {
                            Metodo = dr["Metodo"].ToString() ?? string.Empty,
                            Cantidad = Convert.ToInt32(dr["Cantidad"]),
                            Total = Convert.ToDecimal(dr["Total"])
                        });
                    }
                }
            }

            // Aplicar paginación a la lista de reservas
            modelo.ReservasPaginadas = PaginacionRespuesta<ReporteReserva>.Crear(modelo.Reservas, pagina, 5);

            return View(modelo);
        }
    }
}
