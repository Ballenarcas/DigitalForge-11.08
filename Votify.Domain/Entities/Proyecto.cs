using System;

namespace Votify.Domain.Entities
{
    public class Proyecto
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public Guid VotacionId { get; set; }
        public string? ImagenUrl { get; set; }
        public string? Equipo_Id { get; set; }

        public Proyecto(string nombre, string descripcion, string? equipo, Guid votacionId, string? imagenUrl = null, string? id = null)
        {
            Id = id ?? Guid.NewGuid().ToString();
            Nombre = nombre;
            Descripcion = descripcion;
            Equipo_Id = equipo;
            VotacionId = votacionId;
            ImagenUrl = imagenUrl;
        }
    }
}
