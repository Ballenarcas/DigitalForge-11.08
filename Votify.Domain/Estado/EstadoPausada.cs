using Votify.Domain.Entities;
public class EstadoPausada : IEstadoVotacion
{
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
       throw new InvalidOperationException("La votación ya está pausada.");
    }

    public void ReanudarVotacion(Votacion votacion)
    {
       votacion.CambiarEstado(new EstadoActiva());
    }

    public string ObtenerResultados()
    {
        // Lógica para obtener los resultados
        return string.Empty;
    }
}