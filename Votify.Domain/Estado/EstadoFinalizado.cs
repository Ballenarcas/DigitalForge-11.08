using Votify.Domain.Entities;

namespace Votify.Domain.Estado
{
    public class EstadoFinalizada : IEstadoVotacion
    {
        public string Nombre => "Finalizada";

        public void IniciarVotacion(Votacion votacion)
        {
            throw new InvalidOperationException("No se puede reabrir una votacion finalizada.");
        }

        public void FinalizarVotacion(Votacion votacion)
        {
            throw new InvalidOperationException("La votacion ya esta finalizada.");
        }

        public void PausarVotacion(Votacion votacion)
        {
            throw new InvalidOperationException("No se puede pausar una votacion finalizada.");
        }

        public void ReanudarVotacion(Votacion votacion)
        {
            throw new InvalidOperationException("No se puede reanudar una votacion finalizada.");
        }

        public void ValidarVoto(Votacion votacion)
        {
            throw new InvalidOperationException("No se pueden emitir votos en una votacion finalizada.");
        }

        public string ObtenerResultados()
        {
            return string.Empty;
        }
    }
}