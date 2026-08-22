namespace ProyectoHostalMVC.Models
{
    public class PaginacionRespuesta<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int PaginaActual { get; set; } = 1;
        public int TotalPaginas { get; set; } = 1;
        public int TotalRegistros { get; set; } = 0;
        public int TamanoPagina { get; set; } = 10;
        public string? TerminoBusqueda { get; set; }

        public bool TienePaginaAnterior => PaginaActual > 1;
        public bool TienePaginaSiguiente => PaginaActual < TotalPaginas;

        public static PaginacionRespuesta<T> Crear(IEnumerable<T> fuente, int paginaActual, int tamanoPagina, string? busqueda = null)
        {
            var lista = fuente.ToList();
            var total = lista.Count;
            var totalPaginas = (int)Math.Ceiling(total / (double)tamanoPagina);
            totalPaginas = totalPaginas == 0 ? 1 : totalPaginas;
            paginaActual = Math.Max(1, Math.Min(paginaActual, totalPaginas));

            var items = lista
                .Skip((paginaActual - 1) * tamanoPagina)
                .Take(tamanoPagina)
                .ToList();

            return new PaginacionRespuesta<T>
            {
                Items = items,
                PaginaActual = paginaActual,
                TotalPaginas = totalPaginas,
                TotalRegistros = total,
                TamanoPagina = tamanoPagina,
                TerminoBusqueda = busqueda
            };
        }
    }
}