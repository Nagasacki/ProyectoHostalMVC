using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoHostalMVC.Models;
using System.Data;

namespace ProyectoHostalMVC.Controllers
{
    public class ReservasController : Controller
    {
        private readonly IConfiguration _config;

        public ReservasController(IConfiguration config)
        {
            _config = config;
        }


        // LISTAR RESERVAS
        public IActionResult Index()
        {
            List<Reserva> lista = new List<Reserva>();

            using (SqlConnection cn =
                new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                SqlCommand cmd =
                    new SqlCommand("dbo.sp_ListarReservas", cn);

                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Reserva reserva = new Reserva();

                        reserva.IdReserva =
                            Convert.ToInt32(dr["IdReserva"]);

                        reserva.IdCliente =
                            Convert.ToInt32(dr["IdCliente"]);

                        reserva.NombreCliente =
                            dr["NombreCliente"].ToString();

                        reserva.IdHabitacion =
                            Convert.ToInt32(dr["IdHabitacion"]);

                        reserva.NumeroHabitacion =
                            dr["NumeroHabitacion"].ToString();

                        reserva.TipoHabitacion =
                            dr["TipoHabitacion"].ToString();

                        reserva.FechaEntrada =
                            Convert.ToDateTime(dr["FechaEntrada"]);

                        reserva.FechaSalida =
                            Convert.ToDateTime(dr["FechaSalida"]);

                        reserva.CantidadDias =
                            Convert.ToInt32(dr["CantidadDias"]);

                        reserva.PrecioDia =
                            Convert.ToDecimal(dr["PrecioDia"]);

                        reserva.Total =
                            Convert.ToDecimal(dr["Total"]);

                        reserva.Estado =
                            dr["Estado"].ToString();

                        reserva.FechaRegistro =
                            Convert.ToDateTime(dr["FechaRegistro"]);

                        lista.Add(reserva);
                    }
                }
            }

            return View(lista);
        }
        private void CargarCombos()
        {
            List<Cliente> clientes = new List<Cliente>();
            List<Habitacion> habitaciones = new List<Habitacion>();

            using (SqlConnection cn =
                new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                cn.Open();

                // ==============================
                // LISTAR CLIENTES
                // ==============================

                SqlCommand cmdClientes =
                    new SqlCommand("dbo.sp_ListarClientes", cn);

                cmdClientes.CommandType = CommandType.StoredProcedure;

                using (SqlDataReader dr = cmdClientes.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Cliente cliente = new Cliente();

                        cliente.IdCliente =
                            Convert.ToInt32(dr["IdCliente"]);

                        cliente.Nombre =
                            dr["Nombre"].ToString();

                        clientes.Add(cliente);
                    }
                }


                // ==============================
                // HABITACIONES DISPONIBLES
                // ==============================

                SqlCommand cmdHabitaciones =
                    new SqlCommand(
                        "dbo.sp_ListarHabitacionesDisponibles",
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

                        habitaciones.Add(habitacion);
                    }
                }
            }

            ViewBag.Clientes = clientes;
            ViewBag.Habitaciones = habitaciones;
        }
        [HttpGet]
        public IActionResult Create()
        {
            CargarCombos();

            Reserva reserva = new Reserva();

            reserva.FechaEntrada = DateTime.Today;
            reserva.FechaSalida = DateTime.Today.AddDays(1);

            return View(reserva);
        }
        [HttpPost]
        public IActionResult Create(Reserva reserva)
        {
            // ==============================
            // VALIDAR FECHAS
            // ==============================

            if (reserva.FechaSalida <= reserva.FechaEntrada)
            {
                ViewBag.Error =
                    "La fecha de salida debe ser mayor a la fecha de entrada.";

                CargarCombos();

                return View(reserva);
            }


            decimal precioHabitacion = 0;


            using (SqlConnection cn =
                new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                cn.Open();


                // ==============================
                // OBTENER PRECIO HABITACIÓN
                // ==============================

                SqlCommand cmdPrecio =
                    new SqlCommand(
                        "dbo.sp_BuscarHabitacion",
                        cn);

                cmdPrecio.CommandType =
                    CommandType.StoredProcedure;

                cmdPrecio.Parameters.AddWithValue(
                    "@IdHabitacion",
                    reserva.IdHabitacion);


                using (SqlDataReader dr =
                    cmdPrecio.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        precioHabitacion =
                            Convert.ToDecimal(
                                dr["Precio"]);
                    }
                    else
                    {
                        ViewBag.Error =
                            "No se encontró la habitación.";

                        CargarCombos();

                        return View(reserva);
                    }
                }


                // ==============================
                // CALCULAR DÍAS
                // ==============================

                int cantidadDias =
                    (reserva.FechaSalida -
                     reserva.FechaEntrada).Days;


                // ==============================
                // CALCULAR TOTAL
                // ==============================

                decimal total =
                    cantidadDias * precioHabitacion;


                // ==============================
                // REGISTRAR RESERVA
                // ==============================

                SqlCommand cmd =
                    new SqlCommand(
                        "dbo.sp_RegistrarReserva",
                        cn);

                cmd.CommandType =
                    CommandType.StoredProcedure;


                cmd.Parameters.AddWithValue(
                    "@IdCliente",
                    reserva.IdCliente);

                cmd.Parameters.AddWithValue(
                    "@IdHabitacion",
                    reserva.IdHabitacion);

                cmd.Parameters.AddWithValue(
                    "@FechaEntrada",
                    reserva.FechaEntrada);

                cmd.Parameters.AddWithValue(
                    "@FechaSalida",
                    reserva.FechaSalida);

                cmd.Parameters.AddWithValue(
                    "@CantidadDias",
                    cantidadDias);

                cmd.Parameters.AddWithValue(
                    "@PrecioDia",
                    precioHabitacion);

                cmd.Parameters.AddWithValue(
                    "@Total",
                    total);


                cmd.ExecuteNonQuery();
            }


            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            Reserva reserva = new Reserva();

            using (SqlConnection cn =
                new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                SqlCommand cmd =
                    new SqlCommand("dbo.sp_BuscarReserva", cn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdReserva", id);

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        reserva.IdReserva =
                            Convert.ToInt32(dr["IdReserva"]);

                        reserva.IdCliente =
                            Convert.ToInt32(dr["IdCliente"]);

                        reserva.NombreCliente =
                            dr["NombreCliente"].ToString();

                        reserva.IdHabitacion =
                            Convert.ToInt32(dr["IdHabitacion"]);

                        reserva.NumeroHabitacion =
                            dr["NumeroHabitacion"].ToString();

                        reserva.TipoHabitacion =
                            dr["TipoHabitacion"].ToString();

                        reserva.FechaEntrada =
                            Convert.ToDateTime(dr["FechaEntrada"]);

                        reserva.FechaSalida =
                            Convert.ToDateTime(dr["FechaSalida"]);

                        reserva.CantidadDias =
                            Convert.ToInt32(dr["CantidadDias"]);

                        reserva.PrecioDia =
                            Convert.ToDecimal(dr["PrecioDia"]);

                        reserva.Total =
                            Convert.ToDecimal(dr["Total"]);

                        reserva.Estado =
                            dr["Estado"].ToString();

                        reserva.FechaRegistro =
                            Convert.ToDateTime(dr["FechaRegistro"]);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }

            return View(reserva);
        }
        [HttpPost]
        public IActionResult Finalizar(int id)
        {
            using (SqlConnection cn =
                new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                SqlCommand cmd =
                    new SqlCommand("dbo.sp_FinalizarReserva", cn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdReserva", id);

                cn.Open();

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Cancelar(int id)
        {
            using (SqlConnection cn =
                new SqlConnection(_config["ConnectionStrings:cn"]))
            {
                SqlCommand cmd =
                    new SqlCommand("dbo.sp_CancelarReserva", cn);

                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdReserva", id);

                cn.Open();

                cmd.ExecuteNonQuery();
            }

            return RedirectToAction("Index");
        }
    }
}