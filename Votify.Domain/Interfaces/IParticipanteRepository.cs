using System;
using System.Threading.Tasks;
using Votify.Domain.Entities;

namespace Votify.Domain.Interfaces
{
    public interface IParticipanteRepository
    {
        Task<Participante?> GetByEmailAsync(string email);
        Task AddAsync(Participante participante);
    }
}
