using System.Threading.Tasks;
using Votify.Domain.Entities;

namespace Votify.Domain.Interfaces
{
    /// <summary>
    /// Interfaz del observable para notificaciones de equipos y proyectos.
    /// </summary>
    public interface INotificacionObservable
    {
        void AgregarObservador(INotificacionObserver observador);
        void RemoverObservador(INotificacionObserver observador);
        Task NotificarEquipoCreadoAsync(Equipo equipo, string nombreEvento);
        Task NotificarProyectoCreadoAsync(Proyecto proyecto, string nombreVotacion);
    }

    public interface INotificacionObserver
    {
        Task OnEquipoCreadoAsync(Equipo equipo, string nombreEvento);
        Task OnProyectoCreadoAsync(Proyecto proyecto, string nombreVotacion);
    }
}