using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProyectoHostalMVC.Models;
using System.Data;

namespace ProyectoHostalMVC.Controllers
{
    [Authorize]
    public class ClientesController : Controller
    {
        private readonly IConfiguration _config;

        public ClientesController(IConfiguration config)
        {
            _config = config;
        }

        // LISTAR CLIENTES CON PAGINACIÓN Y BÚSQUEDA
        public IActionResult Index(int pagina = 1, string? busqueda = null)
        {
            List<Cliente> lista = new List<Cliente>();

            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                SqlCommand cmd = new SqlCommand("dbo.sp_ListarClientes", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Cliente
                        {
                            IdCliente = Convert.ToInt32(dr["IdCliente"]),
                            Dni = dr["Dni"].ToString() ?? string.Empty,
                            TipoDocumento = dr["TipoDocumento"].ToString() ?? "DNI",
                            Nombre = dr["Nombre"].ToString() ?? string.Empty,
                            Telefono = dr["Telefono"]?.ToString() ?? string.Empty,
                            Correo = dr["Correo"]?.ToString() ?? string.Empty,
                            Nacionalidad = dr["Nacionalidad"]?.ToString(),
                            Direccion = dr["Direccion"]?.ToString(),
                            FechaNacimiento = dr["FechaNacimiento"] == DBNull.Value ? null : Convert.ToDateTime(dr["FechaNacimiento"]),
                            ContactoEmergencia = dr["ContactoEmergencia"]?.ToString(),
                            Observaciones = dr["Observaciones"]?.ToString()
                        });
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                lista = lista.Where(c => 
                    c.Nombre.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
                    c.Dni.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
                    c.Correo.Contains(busqueda, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            var paginacion = PaginacionRespuesta<Cliente>.Crear(lista, pagina, 5, busqueda);
            return View(paginacion);
        }

        // CREAR - GET
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Cliente());
        }

        // CREAR - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Cliente cliente)
        {
            if (!ModelState.IsValid)
            {
                return View(cliente);
            }

            try
            {
                using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:cn"]))
                {
                    SqlCommand cmd = new SqlCommand("dbo.sp_MergeCliente", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdCliente", 0);
                    AgregarParametrosCliente(cmd, cliente);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }

                TempData["SweetAlert_Title"] = "¡Registrado!";
                TempData["SweetAlert_Text"] = "El cliente fue registrado exitosamente.";
                TempData["SweetAlert_Type"] = "success";
                return RedirectToAction("Index");
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    ModelState.AddModelError("Dni", "Ya existe un cliente registrado con este DNI.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al registrar cliente: " + ex.Message);
                }
                return View(cliente);
            }
        }

        // EDITAR - GET
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Cliente? cliente = ObtenerClientePorId(id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        // EDITAR - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Cliente cliente)
        {
            if (!ModelState.IsValid)
            {
                return View(cliente);
            }

            try
            {
                using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:cn"]))
                {
                    SqlCommand cmd = new SqlCommand("dbo.sp_MergeCliente", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@IdCliente", cliente.IdCliente);
                    AgregarParametrosCliente(cmd, cliente);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }

                TempData["SweetAlert_Title"] = "¡Actualizado!";
                TempData["SweetAlert_Text"] = "Los datos del cliente fueron actualizados.";
                TempData["SweetAlert_Type"] = "success";
                return RedirectToAction("Index");
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                {
                    ModelState.AddModelError("Dni", "Ya existe otro cliente con este DNI.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al actualizar cliente: " + ex.Message);
                }
                return View(cliente);
            }
        }

        // DETALLE
        [HttpGet]
        public IActionResult Details(int id)
        {
            Cliente? cliente = ObtenerClientePorId(id);
            if (cliente == null) return NotFound();
            return View(cliente);
        }

        // ELIMINAR - GET
        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            Cliente? cliente = ObtenerClientePorId(id);
            if (cliente == null) return NotFound();
            return View(cliente);
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
                    SqlCommand cmd = new SqlCommand("dbo.sp_EliminarCliente", cn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdCliente", id);

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }

                TempData["SweetAlert_Title"] = "¡Eliminado!";
                TempData["SweetAlert_Text"] = "El cliente fue eliminado correctamente.";
                TempData["SweetAlert_Type"] = "success";
            }
            catch (SqlException ex)
            {
                if (ex.Number == 547)
                {
                    TempData["SweetAlert_Title"] = "No se puede eliminar";
                    TempData["SweetAlert_Text"] = "El cliente tiene reservas asociadas en el historial.";
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

        // =============================================
        // ENDPOINTS AJAX / JSON (Front-End Dinámico)
        // =============================================

        [HttpGet]
        public IActionResult BuscarPorDniJson(string dni)
        {
            if (string.IsNullOrWhiteSpace(dni) || dni.Length < 8 || dni.Length > 15)
            {
                return Json(new { success = false, message = "Documento inválido" });
            }

            Cliente? cliente = null;

            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                SqlCommand cmd = new SqlCommand("dbo.sp_BuscarClientePorDni", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Dni", dni.Trim());

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        cliente = new Cliente
                        {
                            IdCliente = Convert.ToInt32(dr["IdCliente"]),
                            Dni = dr["Dni"].ToString() ?? string.Empty,
                            TipoDocumento = dr["TipoDocumento"].ToString() ?? "DNI",
                            Nombre = dr["Nombre"].ToString() ?? string.Empty,
                            Telefono = dr["Telefono"]?.ToString() ?? string.Empty,
                            Correo = dr["Correo"]?.ToString() ?? string.Empty
                        };
                    }
                }
            }

            if (cliente != null)
            {
                return Json(new { success = true, data = cliente });
            }

            return Json(new { success = false, message = "Cliente no encontrado con el DNI ingresado." });
        }

        private Cliente? ObtenerClientePorId(int id)
        {
            using (SqlConnection cn = new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                SqlCommand cmd = new SqlCommand("dbo.sp_BuscarCliente", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdCliente", id);

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new Cliente
                        {
                            IdCliente = Convert.ToInt32(dr["IdCliente"]),
                            Dni = dr["Dni"].ToString() ?? string.Empty,
                            TipoDocumento = dr["TipoDocumento"].ToString() ?? "DNI",
                            Nombre = dr["Nombre"].ToString() ?? string.Empty,
                            Telefono = dr["Telefono"]?.ToString() ?? string.Empty,
                            Correo = dr["Correo"]?.ToString() ?? string.Empty,
                            Nacionalidad = dr["Nacionalidad"]?.ToString(),
                            Direccion = dr["Direccion"]?.ToString(),
                            FechaNacimiento = dr["FechaNacimiento"] == DBNull.Value ? null : Convert.ToDateTime(dr["FechaNacimiento"]),
                            ContactoEmergencia = dr["ContactoEmergencia"]?.ToString(),
                            Observaciones = dr["Observaciones"]?.ToString()
                        };
                    }
                }
            }
            return null;
        }

        private static void AgregarParametrosCliente(SqlCommand cmd, Cliente cliente)
        {
            cmd.Parameters.AddWithValue("@Dni", cliente.Dni.Trim());
            cmd.Parameters.AddWithValue("@TipoDocumento", cliente.TipoDocumento);
            cmd.Parameters.AddWithValue("@Nombre", cliente.Nombre.Trim());
            cmd.Parameters.AddWithValue("@Telefono", cliente.Telefono.Trim());
            cmd.Parameters.AddWithValue("@Correo", cliente.Correo.Trim());
            cmd.Parameters.AddWithValue("@Nacionalidad", (object?)cliente.Nacionalidad ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Direccion", (object?)cliente.Direccion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@FechaNacimiento", (object?)cliente.FechaNacimiento ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@ContactoEmergencia", (object?)cliente.ContactoEmergencia ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Observaciones", (object?)cliente.Observaciones ?? DBNull.Value);
        }
    }
}
