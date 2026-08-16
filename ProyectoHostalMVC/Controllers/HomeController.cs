using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProyectoHostalMVC.Models;
using System.Data;

namespace ProyectoHostalMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;

        public HomeController(IConfiguration config)
        {
            _config = config;
        }


        public IActionResult Index()
        {
            Dashboard dashboard = new Dashboard();

            using (SqlConnection cn =
                new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                SqlCommand cmd =
                    new SqlCommand("dbo.sp_DashboardResumen", cn);

                cmd.CommandType =
                    CommandType.StoredProcedure;

                cn.Open();

                using (SqlDataReader dr =
                    cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        dashboard.TotalHabitaciones =
                            Convert.ToInt32(
                                dr["TotalHabitaciones"]);

                        dashboard.HabitacionesDisponibles =
                            Convert.ToInt32(
                                dr["HabitacionesDisponibles"]);

                        dashboard.HabitacionesOcupadas =
                            Convert.ToInt32(
                                dr["HabitacionesOcupadas"]);

                        dashboard.TotalClientes =
                            Convert.ToInt32(
                                dr["TotalClientes"]);

                        dashboard.ReservasActivas =
                            Convert.ToInt32(
                                dr["ReservasActivas"]);
                    }
                }
            }

            return View(dashboard);
        }
    }
}