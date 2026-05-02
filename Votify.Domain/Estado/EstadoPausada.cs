using Votify.Domain.Entities;
public class EstadoPausada : IEstadoVotacion
{
    public void IniciarVotacion(Votacion votacion)
    {
        votacion.Estado = EstadoVotacion.Abierta;
    }

    public void FinalizarVotacion(Votacion votacion)
    {
       votacion.Estado = EstadoVotacion.Detenida;
    }

    public void PausarVotacion(Votacion votacion)
    {
       throw new InvalidOperationException("La votación ya está pausada.");
    }

    public void ReanudarVotacion(Votacion votacion)
    {
       votacion.Estado = EstadoVotacion.Abierta;
    }

    public void ValidarVoto(Votacion votacion)
    {
        throw new InvalidOperationException("No se pueden emitir votos en una votación pausada.");
    }

    public string ObtenerResultados()
    {
        // Lógica para obtener los resultados
        return string.Empty;
    }
}