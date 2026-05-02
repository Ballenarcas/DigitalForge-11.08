using Microsoft.VisualBasic;
using Votify.Domain.Entities;
public class EstadoFinalizada : IEstadoVotacion
{
    
    public void IniciarVotacion(Votacion votacion)
    {
        votacion.Estado = EstadoVotacion.Abierta;
    }

    public void FinalizarVotacion(Votacion votacion)
    {
       throw new InvalidOperationException("La votación ya está finalizada.");
    }

    public void PausarVotacion(Votacion votacion)
    {
       throw new InvalidOperationException("No se puede pausar una votación finalizada.");
    }

    public void ReanudarVotacion(Votacion votacion)
    {
       throw new InvalidOperationException("No se puede reanudar una votación finalizada.");
    }

    public void ValidarVoto(Votacion votacion)
    {
        throw new InvalidOperationException("No se pueden emitir votos en una votación finalizada.");
    }

    public string ObtenerResultados()
    {
        // Lógica para obtener los resultados
        return string.Empty;
    }
}