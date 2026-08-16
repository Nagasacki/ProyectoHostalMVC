using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using ProyectoHostalMVC.Models;
using System.Data;

namespace ProyectoHostalMVC.Controllers
{
    public class ClientesController : Controller
    {
        private readonly IConfiguration _config;

        public ClientesController(IConfiguration config)
        {
            _config = config;
        }

        // LISTAR CLIENTES
        public IActionResult Index()
        {
            List<Cliente> lista = new List<Cliente>();

            using (SqlConnection cn =
                new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                SqlCommand cmd =
                    new SqlCommand("dbo.sp_ListarClientes", cn);

                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Cliente cliente = new Cliente();

                        cliente.IdCliente =
                            Convert.ToInt32(dr["IdCliente"]);

                        cliente.Dni =
                            dr["Dni"].ToString();

                        cliente.Nombre =
                            dr["Nombre"].ToString();

                        cliente.Telefono =
                            dr["Telefono"].ToString();

                        cliente.Correo =
                            dr["Correo"].ToString();

                        lista.Add(cliente);
                    }
                }
            }

            return View(lista);
        }
        // CREAR - GET
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        // CREAR - POST
        [HttpPost]
        public IActionResult Create(Cliente cliente)
        {
            using (SqlConnection cn =
                new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                SqlCommand cmd =
                    new SqlCommand("dbo.sp_MergeCliente", cn);

                cmd.CommandType = CommandType.StoredProcedure;

                // 0 porque es un cliente nuevo
                cmd.Parameters.AddWithValue("@IdCliente", 0);
                cmd.Parameters.AddWithValue("@Dni", cliente.Dni);
                cmd.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                cmd.Parameters.AddWithValue("@Telefono", cliente.Telefono);
                cmd.Parameters.AddWithValue("@Correo", cliente.Correo);

                cn.Open();

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Cliente cliente = new Cliente();

            using (SqlConnection cn =
                new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                SqlCommand cmd =
                    new SqlCommand("dbo.sp_BuscarCliente", cn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdCliente", id);

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        cliente.IdCliente =
                            Convert.ToInt32(dr["IdCliente"]);

                        cliente.Dni =
                            dr["Dni"].ToString();

                        cliente.Nombre =
                            dr["Nombre"].ToString();

                        cliente.Telefono =
                            dr["Telefono"].ToString();

                        cliente.Correo =
                            dr["Correo"].ToString();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return View(cliente);
        }
        [HttpPost]
        public IActionResult Edit(Cliente cliente)
        {
            using (SqlConnection cn =
                new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                SqlCommand cmd =
                    new SqlCommand("dbo.sp_MergeCliente", cn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue(
                    "@IdCliente",
                    cliente.IdCliente);

                cmd.Parameters.AddWithValue(
                    "@Dni",
                    cliente.Dni);

                cmd.Parameters.AddWithValue(
                    "@Nombre",
                    cliente.Nombre);

                cmd.Parameters.AddWithValue(
                    "@Telefono",
                    cliente.Telefono);

                cmd.Parameters.AddWithValue(
                    "@Correo",
                    cliente.Correo);

                cn.Open();

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
        // DETALLE DEL CLIENTE
        [HttpGet]
        public IActionResult Details(int id)
        {
            Cliente cliente = new Cliente();

            using (SqlConnection cn =
                new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                SqlCommand cmd =
                    new SqlCommand("dbo.sp_BuscarCliente", cn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdCliente", id);

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        cliente.IdCliente =
                            Convert.ToInt32(dr["IdCliente"]);

                        cliente.Dni =
                            dr["Dni"].ToString();

                        cliente.Nombre =
                            dr["Nombre"].ToString();

                        cliente.Telefono =
                            dr["Telefono"].ToString();

                        cliente.Correo =
                            dr["Correo"].ToString();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return View(cliente);
        }
        // ELIMINAR - GET
        [HttpGet]
        public IActionResult Delete(int id)
        {
            Cliente cliente = new Cliente();

            using (SqlConnection cn =
                new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                SqlCommand cmd =
                    new SqlCommand("dbo.sp_BuscarCliente", cn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdCliente", id);

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        cliente.IdCliente =
                            Convert.ToInt32(dr["IdCliente"]);

                        cliente.Dni =
                            dr["Dni"].ToString();

                        cliente.Nombre =
                            dr["Nombre"].ToString();

                        cliente.Telefono =
                            dr["Telefono"].ToString();

                        cliente.Correo =
                            dr["Correo"].ToString();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return View(cliente);
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
                    new SqlCommand("dbo.sp_EliminarCliente", cn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdCliente", id);

                cn.Open();

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}