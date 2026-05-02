using Votify.Domain.Entities;

public class EstadoActiva : IEstadoVotacion
{
    public void IniciarVotacion(Votacion votacion)
    {
        throw new InvalidOperationException("La votación ya está activa.");
    }

    public void FinalizarVotacion(Votacion votacion)
    {
       votacion.CambiarEstado(new EstadoFinalizada());
       votacion.Estado = EstadoVotacion.Detenida;
    }

    public void PausarVotacion(Votacion votacion)
    {
       votacion.CambiarEstado(new EstadoPausada());
         votacion.Estado = EstadoVotacion.Pausada;
    }

    public void ReanudarVotacion(Votacion votacion)
    {
       throw new InvalidOperationException("No se puede reanudar una votación Activa.");
    }

    public string ObtenerResultados()
    {
        // Lógica para obtener los resultados
        return string.Empty;
    }
}