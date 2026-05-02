using Votify.Domain.Entities;

public interface IEstadoVotacion
{
    void IniciarVotacion(Votacion votacion);
    void FinalizarVotacion(Votacion votacion);
    void PausarVotacion(Votacion votacion);
    void ReanudarVotacion(Votacion votacion);
    string ObtenerResultados();
}