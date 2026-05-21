namespace Votify.Domain.Entities
{
    public class Evento
    {
        public Guid Id { get; set; }
        public string Nombre { get; }
        public string Descripcion { get; }
        public DateTime FechaInicio { get; }
        public DateTime FechaFin { get; }
        public string? ImagenUrl { get; }

        internal Evento(string nombre, string descripcion, DateTime fechaInicio, DateTime fechaFin, string? imagenUrl = null, Guid? id = null)
        {
            Id = id ?? Guid.NewGuid();
            Nombre = nombre;
            Descripcion = descripcion;
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
            ImagenUrl = imagenUrl;
        }
    }
}
