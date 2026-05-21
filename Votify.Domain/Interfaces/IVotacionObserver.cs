using System.Threading.Tasks;
using Votify.Domain.Entities;

namespace Votify.Domain.Interfaces
{
    /// <summary>
    /// Interfaz del observador en el Patrón Observer para el ciclo de vida de las votaciones.
    /// Los observadores concretos implementan esta interfaz para reaccionar ante
    /// las transiciones de estado de una votación.
    /// 
    /// Relación con el Patrón State: el State Pattern (IEstadoVotacion) valida
    /// SI una transición es legal; el Observer notifica QUE la transición ocurrió.
    /// </summary>
    public interface IVotacionObserver
    {
        Task OnVotacionCreadaAsync(Votacion votacion);
        Task OnVotacionPausadaAsync(Votacion votacion);
        Task OnVotacionDetenidaAsync(Votacion votacion);
        Task OnVotacionAbiertaAsync(Votacion votacion);
    }
}
