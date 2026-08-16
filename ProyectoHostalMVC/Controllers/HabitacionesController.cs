using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using ProyectoHostalMVC.Models;

namespace ProyectoHostalMVC.Controllers
{
    public class HabitacionesController : Controller
    {
        private readonly IConfiguration _config;

        public HabitacionesController(IConfiguration config)
        {
            _config = config;
        }


        // LISTAR HABITACIONES
        public IActionResult Index()
        {
            List<Habitacion> lista = new List<Habitacion>();

            using (SqlConnection cn =
                new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                SqlCommand cmd =
                new SqlCommand("dbo.sp_ListarHabitaciones", cn);

                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Habitacion habitacion = new Habitacion();

                        habitacion.IdHabitacion =
                            Convert.ToInt32(dr["IdHabitacion"]);

                        habitacion.Numero =
                            dr["Numero"].ToString();

                        habitacion.Tipo =
                            dr["Tipo"].ToString();

                        habitacion.Precio =
                            Convert.ToDecimal(dr["Precio"]);

                        habitacion.Estado =
                            dr["Estado"].ToString();

                        lista.Add(habitacion);
                    }
                }
            }

            return View(lista);
        }//fin de la lista 
         // GET: Habitaciones/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        // POST: Habitaciones/Create
        [HttpPost]
        public IActionResult Create(Habitacion habitacion)
        {
            using (SqlConnection cn =
                new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                SqlCommand cmd =
                    new SqlCommand("dbo.sp_MergeHabitacion", cn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdHabitacion", 0);
                cmd.Parameters.AddWithValue("@Numero", habitacion.Numero);
                cmd.Parameters.AddWithValue("@Tipo", habitacion.Tipo);
                cmd.Parameters.AddWithValue("@Precio", habitacion.Precio);
                cmd.Parameters.AddWithValue("@Estado", habitacion.Estado);

                cn.Open();

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }//fin del create 
         // EDITAR - GET
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Habitacion habitacion = new Habitacion();

            using (SqlConnection cn =
                new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                SqlCommand cmd =
                    new SqlCommand("dbo.sp_BuscarHabitacion", cn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdHabitacion", id);

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        habitacion.IdHabitacion =
                            Convert.ToInt32(dr["IdHabitacion"]);

                        habitacion.Numero =
                            dr["Numero"].ToString();

                        habitacion.Tipo =
                            dr["Tipo"].ToString();

                        habitacion.Precio =
                            Convert.ToDecimal(dr["Precio"]);

                        habitacion.Estado =
                            dr["Estado"].ToString();
                    }
                }
            }

            return View(habitacion);
        }//fin del Edit
         // EDITAR - POST
        [HttpPost]
        public IActionResult Edit(Habitacion habitacion)
        {
            using (SqlConnection cn =
                new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                SqlCommand cmd =
                    new SqlCommand("dbo.sp_MergeHabitacion", cn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(
                    "@IdHabitacion",
                    habitacion.IdHabitacion);

                cmd.Parameters.AddWithValue(
                    "@Numero",
                    habitacion.Numero);

                cmd.Parameters.AddWithValue(
                    "@Tipo",
                    habitacion.Tipo);

                cmd.Parameters.AddWithValue(
                    "@Precio",
                    habitacion.Precio);

                cmd.Parameters.AddWithValue(
                    "@Estado",
                    habitacion.Estado);

                cn.Open();

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }//
         // DETALLE DE HABITACIÓN
        [HttpGet]
        public IActionResult Details(int id)
        {
            Habitacion habitacion = new Habitacion();

            using (SqlConnection cn =
                new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                SqlCommand cmd =
                    new SqlCommand("dbo.sp_BuscarHabitacion", cn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdHabitacion", id);

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        habitacion.IdHabitacion =
                            Convert.ToInt32(dr["IdHabitacion"]);

                        habitacion.Numero =
                            dr["Numero"].ToString();

                        habitacion.Tipo =
                            dr["Tipo"].ToString();

                        habitacion.Precio =
                            Convert.ToDecimal(dr["Precio"]);

                        habitacion.Estado =
                            dr["Estado"].ToString();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return View(habitacion);
        }
        // ELIMINAR - GET
        [HttpGet]
        public IActionResult Delete(int id)
        {
            Habitacion habitacion = new Habitacion();

            using (SqlConnection cn =
                new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                SqlCommand cmd =
                    new SqlCommand("dbo.sp_BuscarHabitacion", cn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdHabitacion", id);

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        habitacion.IdHabitacion =
                            Convert.ToInt32(dr["IdHabitacion"]);

                        habitacion.Numero =
                            dr["Numero"].ToString();

                        habitacion.Tipo =
                            dr["Tipo"].ToString();

                        habitacion.Precio =
                            Convert.ToDecimal(dr["Precio"]);

                        habitacion.Estado =
                            dr["Estado"].ToString();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return View(habitacion);
        }
        // ELIMINAR - POST
        [HttpPost]
        [ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            using (SqlConnection cn =
                new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                SqlCommand cmd =
                    new SqlCommand("dbo.sp_EliminarHabitacion", cn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdHabitacion", id);

                cn.Open();

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}