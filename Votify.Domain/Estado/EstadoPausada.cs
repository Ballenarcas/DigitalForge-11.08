using Votify.Domain.Entities;

namespace Votify.Domain.Estado
{
    public class EstadoPausada : IEstadoVotacion
    {
        public string Nombre => "Pausada";

        public void IniciarVotacion(Votacion votacion)
        {
            votacion.CambiarEstado(new EstadoActiva());
        }

        public void FinalizarVotacion(Votacion votacion)
        {
            votacion.CambiarEstado(new EstadoFinalizada());
        }

        public void PausarVotacion(Votacion votacion)
        {
            throw new InvalidOperationException("La votacion ya esta pausada.");
        }

        public void ReanudarVotacion(Votacion votacion)
        {
            votacion.CambiarEstado(new EstadoActiva());
        }

        public void ValidarVoto(Votacion votacion)
        {
            throw new InvalidOperationException("No se pueden emitir votos en una votacion pausada.");
        }

        public string ObtenerResultados()
        {
            return string.Empty;
        }
    }
}