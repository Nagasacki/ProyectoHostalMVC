using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProyectoHostalMVC.Models;
using System.Data;

namespace ProyectoHostalMVC.Controllers
{
    [Authorize]
    public class HabitacionesController : Controller
    {
        private readonly IConfiguration _config;

        public HabitacionesController(IConfiguration config)
        {
            _config = config;
        }

        // LISTAR HABITACIONES CON PAGINACIÓN Y BÚSQUEDA
        public IActionResult Index(int pagina = 1, string? busqueda = null)
        {
            List<Habitacion> lista = new List<Habitacion>();

            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                SqlCommand cmd = new SqlCommand("dbo.sp_ListarHabitaciones", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Habitacion
                        {
                            IdHabitacion = Convert.ToInt32(dr["IdHabitacion"]),
                            Numero = dr["Numero"].ToString() ?? string.Empty,
                            Tipo = dr["Tipo"].ToString() ?? string.Empty,
                            Precio = Convert.ToDecimal(dr["Precio"]),
                            Estado = dr["Estado"].ToString() ?? "Disponible"
                        });
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                lista = lista.Where(h =>
                    h.Numero.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
                    h.Tipo.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
                    h.Estado.Contains(busqueda, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            var paginacion = PaginacionRespuesta<Habitacion>.Crear(lista, pagina, 6, busqueda);
            return View(paginacion);
        }

        // CREAR - GET
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            return View(new Habitacion());
        }

        // CREAR - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Create(Habitacion habitacion)
        {
            if (habitacion.Estado != "Mantenimiento" && habitacion.Estado != "Fuera de servicio")
                habitacion.Estado = "Disponible";
            if (!ModelState.IsValid)
            {
                return View(habitacion);
            }

            try
            {
                using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:cn"]))
                {
                    SqlCommand cmd = new SqlCommand("dbo.sp_MergeHabitacion", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdHabitacion", 0);
                    cmd.Parameters.AddWithValue("@Numero", habitacion.Numero.Trim());
                    cmd.Parameters.AddWithValue("@Tipo", habitacion.Tipo.Trim());
                    cmd.Parameters.AddWithValue("@Precio", habitacion.Precio);
                    cmd.Parameters.AddWithValue("@Estado", habitacion.Estado);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }

                TempData["SweetAlert_Title"] = "¡Habitación Creada!";
                TempData["SweetAlert_Text"] = "La habitación fue registrada con éxito.";
                TempData["SweetAlert_Type"] = "success";
                return RedirectToAction("Index");
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    ModelState.AddModelError("Numero", "Ya existe una habitación registrada con este número.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al registrar habitación: " + ex.Message);
                }
                return View(habitacion);
            }
        }

        // EDITAR - GET
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public IActionResult Edit(int id)
        {
            Habitacion? habitacion = ObtenerHabitacionPorId(id);
            if (habitacion == null) return NotFound();
            return View(habitacion);
        }

        // EDITAR - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult Edit(Habitacion habitacion)
        {
            Habitacion? actual = ObtenerHabitacionPorId(habitacion.IdHabitacion);
            if (actual == null) return NotFound();
            if (actual.Estado is "Ocupada" or "Limpieza")
                habitacion.Estado = actual.Estado;
            else if (habitacion.Estado != "Mantenimiento" && habitacion.Estado != "Fuera de servicio")
                habitacion.Estado = "Disponible";

            if (!ModelState.IsValid)
            {
                return View(habitacion);
            }

            try
            {
                using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:cn"]))
                {
                    SqlCommand cmd = new SqlCommand("dbo.sp_MergeHabitacion", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdHabitacion", habitacion.IdHabitacion);
                    cmd.Parameters.AddWithValue("@Numero", habitacion.Numero.Trim());
                    cmd.Parameters.AddWithValue("@Tipo", habitacion.Tipo.Trim());
                    cmd.Parameters.AddWithValue("@Precio", habitacion.Precio);
                    cmd.Parameters.AddWithValue("@Estado", habitacion.Estado);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }

                TempData["SweetAlert_Title"] = "¡Actualizado!";
                TempData["SweetAlert_Text"] = "Los datos de la habitación fueron actualizados.";
                TempData["SweetAlert_Type"] = "success";
                return RedirectToAction("Index");
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    ModelState.AddModelError("Numero", "Ya existe otra habitación con este número.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al actualizar: " + ex.Message);
                }
                return View(habitacion);
            }
        }

        // DETALLES
        [HttpGet]
        public IActionResult Details(int id)
        {
            Habitacion? habitacion = ObtenerHabitacionPorId(id);
            if (habitacion == null) return NotFound();
            return View(habitacion);
        }

        // ELIMINAR - GET
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            Habitacion? habitacion = ObtenerHabitacionPorId(id);
            if (habitacion == null) return NotFound();
            return View(habitacion);
        }

        // ELIMINAR - POST
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:cn"]))
                {
                    SqlCommand cmd = new SqlCommand("dbo.sp_EliminarHabitacion", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdHabitacion", id);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }

                TempData["SweetAlert_Title"] = "¡Eliminado!";
                TempData["SweetAlert_Text"] = "La habitación fue eliminada exitosamente.";
                TempData["SweetAlert_Type"] = "success";
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547)
                {
                    TempData["SweetAlert_Title"] = "No se puede eliminar";
                    TempData["SweetAlert_Text"] = "La habitación tiene historial de reservas registradas.";
                    TempData["SweetAlert_Type"] = "error";
                }
                else
                {
                    TempData["SweetAlert_Title"] = "Error";
                    TempData["SweetAlert_Text"] = ex.Message;
                    TempData["SweetAlert_Type"] = "error";
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarcarDisponible(int id)
        {
            try
            {
                using SqlConnection cn = new SqlConnection(_config["ConnectionStrings:cn"]);
                SqlCommand cmd = new SqlCommand("dbo.sp_MarcarHabitacionDisponible", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdHabitacion", id);
                cn.Open();
                cmd.ExecuteNonQuery();

                TempData["SweetAlert_Title"] = "Habitación disponible";
                TempData["SweetAlert_Text"] = "La limpieza terminó y la habitación puede volver a reservarse.";
                TempData["SweetAlert_Type"] = "success";
            }
            catch (SqlException ex)
            {
                TempData["SweetAlert_Title"] = "No se pudo actualizar";
                TempData["SweetAlert_Text"] = ex.Message;
                TempData["SweetAlert_Type"] = "error";
            }
            return RedirectToAction("Index");
        }

        private Habitacion? ObtenerHabitacionPorId(int id)
        {
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                SqlCommand cmd = new SqlCommand("dbo.sp_BuscarHabitacion", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdHabitacion", id);

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new Habitacion
                        {
                            IdHabitacion = Convert.ToInt32(dr["IdHabitacion"]),
                            Numero = dr["Numero"].ToString() ?? string.Empty,
                            Tipo = dr["Tipo"].ToString() ?? string.Empty,
                            Precio = Convert.ToDecimal(dr["Precio"]),
                            Estado = dr["Estado"].ToString() ?? "Disponible"
                        };
                    }
                }
            }
            return null;
        }
    }
}
