using Votify.Domain.Entities;

namespace Votify.Domain.Estado
{
    public interface IEstadoVotacion
    {
        string Nombre { get; }
        void IniciarVotacion(Votacion votacion);
        void FinalizarVotacion(Votacion votacion);
        void PausarVotacion(Votacion votacion);
        void ReanudarVotacion(Votacion votacion);
        void ValidarVoto(Votacion votacion);
        string ObtenerResultados();
    }
}