using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Votify.Domain.Entities;

namespace Votify.Domain.Interfaces
{
    public interface IEquipoRepository
    {
        Task GuardarAsync(Equipo equipo);
        Task<Equipo?> ObtenerPorIdAsync(Guid id);
        Task<IEnumerable<Equipo>> ObtenerTodosAsync();
    }
}
