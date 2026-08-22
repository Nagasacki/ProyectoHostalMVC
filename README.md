# Hostal Velamor

Proyecto académico desarrollado con ASP.NET Core MVC, Razor, Bootstrap, JavaScript, AJAX/JSON, ADO.NET y SQL Server.

## Funciones principales

- Inicio de sesión para Administrador y Recepción.
- Mantenimiento de huéspedes y habitaciones.
- Consulta de disponibilidad por fecha sin cruces de reservas.
- Reserva confirmada, check-in, check-out, cancelación y no presentado.
- Estados de habitación: Disponible, Ocupada, Limpieza, Mantenimiento y Fuera de servicio.
- Registro de adelantos, abonos y pagos finales.
- Panel con llegadas, salidas, hospedados, limpieza y saldos pendientes.
- Reportes por fechas, pagos por método y paginación.

## Preparar la base de datos local

En SQL Server Express ejecutar una sola vez:

1. `ProyectoHostalMVC/Database/HOSTALVELAMOR.sql`

Este único script crea la base de datos, sus tablas, relaciones, índices, datos iniciales y procedimientos almacenados.

La conexión configurada en `appsettings.json` utiliza `localhost\\SQLEXPRESS`, la base `HOSTALVELAMORMVC` y autenticación integrada de Windows.

## Ejecutar

Abrir `ProyectoHostalMVC.slnx` en Visual Studio, seleccionar el proyecto y presionar `Ctrl + F5`.

También puede iniciarse desde una terminal:

```text
dotnet run --project ProyectoHostalMVC/ProyectoHostalMVC.csproj
```

## Usuarios de demostración

```text
Administrador: admin@hostalvelamor.com / Admin123*
Recepción: recepcion@hostalvelamor.com / Recepcion123*
```

## Flujo recomendado para la exposición

1. Buscar disponibilidad cambiando las fechas de una nueva reserva.
2. Buscar un huésped por documento para demostrar AJAX y JSON.
3. Confirmar la reserva y explicar la validación transaccional.
4. Realizar el check-in para ocupar la habitación.
5. Registrar el saldo pendiente.
6. Realizar el check-out y mostrar que la habitación queda en Limpieza.
7. Marcar la limpieza como terminada desde Habitaciones.
8. Revisar el reporte filtrado por fechas.
