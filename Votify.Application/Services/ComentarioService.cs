using Votify.Application.Interfaces;
using Votify.Domain.Interfaces;

namespace Votify.Application.Services
{
    public class ComentarioService : IComentarioService
    {
        private readonly IComentarioRepository _comentarioRepository;

        public ComentarioService(IComentarioRepository comentarioRepository)
        {
            _comentarioRepository = comentarioRepository;
        }

        public async Task AgregarComentarioAsync(string proyectoId, string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                throw new ArgumentException("El comentario no puede estar vacío.");
            }

            await _comentarioRepository.GuardarAsync(proyectoId, texto);
        }

        public async Task<List<string>> ObtenerComentariosAsync(string proyectoId)
        {
            return await _comentarioRepository.ObtenerAsync(proyectoId);
        }
    }
}
