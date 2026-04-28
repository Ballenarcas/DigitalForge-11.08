using System.Threading.Tasks;
using Votify.Domain.Entities;

namespace Votify.Domain.Interfaces
{
    public interface IParticipanteEventoRepository
    {
        Task GuardarAsync(ParticipanteEvento participanteEvento);
    }
}