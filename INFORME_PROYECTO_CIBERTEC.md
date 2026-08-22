# INSTITUTO DE EDUCACIÓN SUPERIOR CIBERTEC
## DIRECCIÓN ACADÉMICA - CARRERAS PROFESIONALES
### ESCUELA DE TECNOLOGÍAS DE LA INFORMACIÓN
### CARRERA: COMPUTACIÓN E INFORMÁTICA

---

# PLAN DE PROYECTO DE INVESTIGACIÓN APLICADA

**CURSO:** Desarrollo de Servicios Web I (4695)  
**CICLO:** Quinto Ciclo  
**SEMESTRE:** 2026-I  
**TEMA:** Sistema Web de Gestión Hotelera y Control de Reservas "HOSTAL VELAMOR"  

**DOCENTE:** Yuri Renzo Zambrano Macedo  
**COORDINADOR DEL GRUPO:** Estiven Santillan Montoya  
**INTEGRANTES:**
- Estiven Santillan Montoya (Coordinador / Desarrollador Full-Stack)
- Juan Diego Romero Peralta (Desarrollador Backend / Base de Datos)

---

## 1. FUNDAMENTACIÓN

El presente proyecto de investigación aplicada se orienta al diseño, construcción y despliegue de una **Aplicación Web Empresarial bajo la arquitectura ASP.NET Core MVC con .NET 8 y Microsoft SQL Server**. 

En el sector de servicios turísticos y de hospedaje en el Perú, las micro y pequeñas empresas (MYPEs) hoteleras afrontan serias dificultades para gestionar la disponibilidad de habitaciones, el registro de huéspedes y la liquidación de cobros debido al uso de registros manuales y hojas de cálculo desarticuladas. La implementación de esta solución centralizada permite optimizar los tiempos de atención, garantizar la consistencia transaccional de los datos y proveer información gerencial en tiempo real para la toma de decisiones.

---

## 2. OBJETIVOS

Proveer una solución informática moderna, robusta y escalable que digitalice el ciclo completo de reservas, inventario de habitaciones, facturación y generación de reportes financieros del **Hostal Velamor**.

---

## 3. INTEGRANTES Y ROLES

* **Estiven Santillan Montoya:** Coordinador general, diseño de arquitectura MVC, implementación de autenticación basada en roles, interactividad front-end con AJAX/JSON y redacción de la documentación técnica.
* **Juan Diego Romero Peralta:** Diseño y modelado de base de datos relacional en SQL Server, desarrollo de procedimientos almacenados transaccionales, lógica de acceso a datos con ADO.NET y elaboración de pruebas de integración.

---

## 4. ESPECIFICACIÓN Y ALCANCE DEL PROYECTO

El sistema no se limita a un mantenimiento básico de tablas; comprende un **proceso de negocio transaccional complejo**:
1. **Control de Inventario y Estados de Habitaciones:** Seguimiento de estados (*Disponible*, *Ocupada*, *Mantenimiento*).
2. **Registro Asíncrono de Huéspedes:** Búsqueda y autocompletado en tiempo real de clientes por DNI mediante llamadas **AJAX y JSON**.
3. **Liquidación Dinámica de Tarifas:** Cálculo automático en cliente del número de noches y total a pagar según el tipo de habitación.
4. **Transaccionalidad en Base de Datos:** Los procedimientos almacenados ejecutan bloques `BEGIN TRANSACTION` / `COMMIT` para garantizar que la asignación o liberación de la habitación y el estado de la reserva sean atómicos.
5. **Seguridad y Control de Acceso:** Sistema de autenticación con cookies y autorización basada en perfiles (*Administrador* y *Recepcionista*).
6. **Módulo de Analítica y Reportes:** Filtrado por rango de fechas, cálculo de ingresos brutos acumulados, conteo de cancelaciones y paginación en pantalla.

---

## 5. ESTRUCTURA DEL PROYECTO

### 5.1. Resumen
El proyecto **Hostal Velamor** es una plataforma web desarrollada en **ASP.NET Core 8 MVC** que centraliza la administración operativa y financiera de un establecimiento de hospedaje. Integra una base de datos relacional en **Microsoft SQL Server**, persistencia mediante **ADO.NET** con procedimientos almacenados optimizados, interfaz responsiva con **Bootstrap 5**, componentes visuales con **Bootstrap Icons** y **SweetAlert2**, y un front-end reactivo mediante **JavaScript asíncrono (AJAX / Fetch API)**.

### 5.2. Introducción
El rubro hotelero exige agilidad en el *check-in* y *check-out*, precisión en las tarifas y disponibilidad confiable. El manejo tradicional en cuadernos de registro provoca sobreventa (*overbooking*), pérdida de datos de clientes y retrasos en los cierres de caja. Este proyecto resuelve dicha problemática brindando una herramienta accesible desde navegadores web con perfiles diferenciados de usuario.

### 5.3. Diagnóstico (Análisis SEPTE)

#### Variable Social
* **Diagnóstico:** El turismo interno y corporativo en el Perú ha mostrado una recuperación sostenida, superando los 37 millones de viajes anuales según reportes del Ministerio de Comercio Exterior y Turismo (MINCETUR). Los usuarios demandan una atención rápida al momento del ingreso, esperando tiempos de registro menores a 2 minutos.
* **Impacto Social:** La automatización reduce las colas de espera en recepción y mejora la satisfacción y percepción de seguridad de los huéspedes.

#### Variable Económica
* **Diagnóstico:** Las pérdidas financieras en hospedajes independientes debidas a descontrol en la facturación y cobro erróneo de estadías alcanzan entre el 8% y 15% de la facturación mensual (Cámara Nacional de Turismo - CANATUR).
* **Impacto Económico:** El sistema asegura el cálculo exacto de tarifas por noche y días reales de estadía, garantizando la trazabilidad de cada sol ingresado y reduciendo a cero las fugas de dinero por cobros manuales.

#### Variable Tecnológica (Variable Principal)
* **Diagnóstico:** La adopción de arquitecturas web modernas basadas en **.NET 8** permite desarrollar soluciones con alto rendimiento, compilación nativa eficiente y soporte multiplataforma. La utilización de **AJAX (Asynchronous JavaScript and XML) y JSON** evita recargas innecesarias del navegador, proporcionando una experiencia fluida al usuario final (*Single-Page-Like Experience* en formularios clave).
* **Seguridad Tecnológica:** La autenticación basada en Cookies criptográficamente seguras y la segregación de privilegios por roles previenen accesos no autorizados a información confidencial del negocio.

---

### 5.4. Objetivos SMART del Proyecto

* **OBJ 1 (Eficiencia Operativa):**
  * **S (Específico):** Reducir el tiempo promedio de registro y asignación de habitaciones (*check-in*).
  * **M (Medible):** Disminuir el tiempo de atención de 6 minutos a menos de 1.5 minutos por cliente.
  * **A (Alcanzable):** Mediante la búsqueda instantánea por DNI con AJAX y la precarga dinámica de habitaciones disponibles.
  * **R (Relevante):** Elimina la fricción en la llegada de huéspedes y previene errores humanos de digitación.
  * **T (Temporal):** Logrado desde el primer mes de puesta en producción del sistema.

* **OBJ 2 (Control Financiero y Reportes):**
  * **S (Específico):** Centralizar la liquidación de ingresos y consolidación de reservas.
  * **M (Medible):** Alcanzar el 100% de conciliación entre habitaciones ocupadas e ingresos registrados en caja.
  * **A (Alcanzable):** Utilizando procedimientos almacenados transaccionales y el módulo de reportes por rango de fechas.
  * **R (Relevante):** Proporciona a la gerencia balances inmediatos y confiables de ingresos netos.
  * **T (Temporal):** En un período de evaluación continua de 30 días tras el despliegue.

---

### 5.5. Justificación del Proyecto y Beneficiarios

* **Justificación:** El software optimiza el flujo administrativo, elimina el sobrecosto de papelería, previene la sobreventa de cuartos y proporciona métricas operativas al instante.
* **Beneficiarios Directos:**
  * **Recepcionistas y Personal de Atención:** Cuentan con un panel intuitivo para consultar disponibilidad y emitir reservas rápidamente.
  * **Administrador / Dueño del Hostal:** Posee acceso restringido a reportes de ingresos, balance de cancelaciones y gestión del catálogo.
* **Beneficiarios Indirectos:**
  * **Los Huéspedes:** Experimentan un servicio ágil, transparente y seguro.

---

### 5.6. Definición y Alcance Técnico

#### A. Arquitectura de Software
* **Patrón Arquitectónico:** Modelo - Vista - Controlador (MVC).
* **Controladores Desarrollados:**
  * `AccountController`: Gestión de autenticación, login, claims, cookies y logout.
  * `HomeController`: Métricas del Dashboard inicial (`sp_DashboardResumen`).
  * `ClientesController`: Mantenimiento CRUD, paginación, filtros y API AJAX (`BuscarPorDniJson`).
  * `HabitacionesController`: Mantenimiento CRUD de inventario y tarifas.
  * `ReservasController`: Proceso de negocio principal con validación de fechas, cálculo en vivo y transacciones.
  * `ReporteController`: Módulo restringido a Administradores con métricas financieras y listados paginados.

#### B. Base de Datos Relacional (`HOSTALVELAMORMVC`)
* **Tablas:** `Usuario`, `Cliente`, `Habitacion`, `Reserva`.
* **Procedimientos Almacenados:** 16 SPs creados para encapsular toda la lógica SQL, aplicando `BEGIN TRANSACTION` / `COMMIT TRANSACTION` en los cambios de estado de habitaciones y reservas.

#### C. Seguridad y Roles
| Rol | Permisos y Accesos |
| :--- | :--- |
| **Administrador** | Acceso total: Dashboard, Clientes, Habitaciones (Crear/Editar/Eliminar), Reservas, Reportes Financieros. |
| **Recepcionista** | Acceso operativo: Dashboard, Clientes, Reservas (Crear/Finalizar/Cancelar), Consulta de Habitaciones. |

---

### 5.7. Manual de Instalación y Despliegue

1. **Requisitos Previos:**
   * .NET 8.0 SDK instalado.
   * Microsoft SQL Server (LocalDB, SQLEXPRESS o SQL Server Standard).
2. **Inicialización de Base de Datos:**
   * Ejecutar el script `HOSTALVELAMOR.sql` en SQL Server Management Studio o mediante PowerShell.
3. **Configuración de Cadena de Conexión:**
   * Verificar en `appsettings.json`:
     ```json
     "ConnectionStrings": {
       "cn": "Server=localhost\\SQLEXPRESS;Database=HOSTALVELAMORMVC;Integrated Security=True;TrustServerCertificate=True;"
     }
     ```
4. **Ejecución del Proyecto:**
   ```bash
   dotnet restore
   dotnet build
   dotnet run --project ProyectoHostalMVC
   ```
5. **Credenciales de Prueba:**
   * **Administrador:** `admin@hostalvelamor.com` / Clave: `Admin123*`
   * **Recepcionista:** `recepcion@hostalvelamor.com` / Clave: `Recepcion123*`

---

### 5.8. Conclusiones

1. La implementación de la arquitectura **ASP.NET Core MVC con .NET 8** y **ADO.NET mediante Procedimientos Almacenados** demostró ser una solución de alto rendimiento, asegurando una clara separación de responsabilidades y facilitando la mantenibilidad del código.
2. La integración de **AJAX y JSON** en el front-end transformó radicalmente la experiencia de usuario en la recepción, permitiendo validar y autocompletar la información de los huéspedes en milisegundos sin recargar la página.
3. La aplicación estricta de **transacciones SQL (`BEGIN TRANSACTION / COMMIT`)** en el proceso de reservas garantizó la integridad referencial y eliminó por completo el riesgo de inconsistencias en el estado de ocupación de las habitaciones.

---

### 5.9. Recomendaciones

1. Se recomienda implementar a futuro una pasarela de pagos digitales (ej. Niubiz, Stripe, Culqi o Izipay) para permitir el cobro anticipado con tarjetas de crédito/débito y transferencias QR (Yape/Plin).
2. Para entornos de producción en la nube (Azure / AWS), se aconseja almacenar las contraseñas de los usuarios utilizando funciones hash robustas con salt (como `BCrypt` o `PBKDF2`) y administrar las credenciales de conexión mediante *Azure Key Vault*.
3. Implementar un módulo de notificaciones automáticas por correo electrónico (vía SMTP o SendGrid) que envíe al huésped su comprobante de reserva en formato PDF al momento de registrarse.

---

### 5.10. Glosario de Términos

* **MVC (Model-View-Controller):** Patrón de diseño de software que separa los datos (Modelo), la interfaz de usuario (Vista) y la lógica de control (Controlador).
* **AJAX (Asynchronous JavaScript and XML):** Técnica de desarrollo web para enviar y recuperar datos de un servidor de manera asíncrona en segundo plano.
* **JSON (JavaScript Object Notation):** Formato ligero de intercambio de datos, fácil de leer y escribir tanto para humanos como para computadoras.
* **Claims-based Authentication:** Mecanismo de seguridad en ASP.NET Core donde la identidad del usuario se representa mediante un conjunto de atributos o declaraciones (nombre, correo, rol).
* **Stored Procedure (Procedimiento Almacenado):** Conjunto de instrucciones SQL almacenadas y precompiladas en el motor de base de datos.

---

### 5.11. Bibliografía

* Microsoft Learn. (2024). *Información general sobre ASP.NET Core MVC*. Microsoft Documentation. https://learn.microsoft.com/es-es/aspnet/core/mvc/overview
* Microsoft Learn. (2024). *Autenticación basada en cookies en ASP.NET Core*. Microsoft Documentation. https://learn.microsoft.com/es-es/aspnet/core/security/authentication/cookie
* Freeman, A. (2022). *Pro ASP.NET Core 6: Develop Cloud-Ready Web Applications Using MVC, Blazor, and Razor Pages* (9th ed.). Apress.
* CIBERTEC. (2026). *Guía Didáctica de Desarrollo de Servicios Web I*. Instituto de Educación Superior Cibertec.