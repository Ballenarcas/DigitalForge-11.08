using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Votify.Domain.Entities;

namespace Votify.Domain.Interfaces
{
    public interface IParticipanteRepository
    {
        Task<Participante?> GetByEmailAsync(string email);
        Task<Participante?> ObtenerPorIdAsync(Guid id);
        Task AddAsync(Participante participante);
        Task ActualizarAsync(Participante participante);
        Task<IEnumerable<Participante>> ObtenerTodosAsync();
    }
}
