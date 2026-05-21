using System.Threading.Tasks;
using Votify.Domain.Entities;

namespace Votify.Domain.Interfaces
{
    /// <summary>
    /// Interfaz del subject/observable en el Patrón Observer.
    /// Gestiona la suscripción de observadores y envía notificaciones
    /// cuando ocurren transiciones de estado en las votaciones.
    /// </summary>
    public interface IVotacionObservable
    {
        void AgregarObservador(IVotacionObserver observador);
        void RemoverObservador(IVotacionObserver observador);
        Task NotificarVotacionCreadaAsync(Votacion votacion);
        Task NotificarVotacionPausadaAsync(Votacion votacion);
        Task NotificarVotacionDetenidaAsync(Votacion votacion);
        Task NotificarVotacionAbiertaAsync(Votacion votacion);
    }
}
