using Votify.Domain.Entities;

namespace Votify.Domain.Estado
{
    public class EstadoActiva : IEstadoVotacion
    {
        public string Nombre => "Activa";

        public void IniciarVotacion(Votacion votacion)
        {
            throw new InvalidOperationException("La votacion ya esta activa.");
        }

        public void FinalizarVotacion(Votacion votacion)
        {
            votacion.CambiarEstado(new EstadoFinalizada());
        }

        public void PausarVotacion(Votacion votacion)
        {
            votacion.CambiarEstado(new EstadoPausada());
        }

        public void ReanudarVotacion(Votacion votacion)
        {
            throw new InvalidOperationException("No se puede reanudar una votacion activa.");
        }

        public void ValidarVoto(Votacion votacion)
        {
        }

        public string ObtenerResultados()
        {
            return string.Empty;
        }
    }
}