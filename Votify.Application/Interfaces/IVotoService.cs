using System.Threading.Tasks;
using Votify.Application.DTOs;

namespace Votify.Application.Interfaces
{
    public interface IVotoService
    {
        Task VotarAsync(VotarDto dto);
    }
}
