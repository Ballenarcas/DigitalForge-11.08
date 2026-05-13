using Votify.Application.DTOs;
using Votify.Application.Interfaces;

namespace Votify.Application.Services.Fachadas
{
    public class ProyectoFachada : IProyectoFachada
    {
        private readonly IProyectoService _proyectoService;
        private readonly IComentarioService _comentarioService;

        public ProyectoFachada(IProyectoService proyectoService, IComentarioService comentarioService)
        {
            _proyectoService = proyectoService;
            _comentarioService = comentarioService;
        }

        public Task<string> CrearProyectoAsync(ProyectoDto dto)
            => _proyectoService.CrearProyectoAsync(dto);

        public Task<ProyectoDto?> ObtenerProyectoAsync(string id)
            => _proyectoService.ObtenerProyectoAsync(id);

        public Task<List<ProyectoDto>> ObtenerProyectosAsync()
            => _proyectoService.ObtenerProyectosAsync();

        public Task<List<ProyectoDto>> ObtenerProyectosPorVotacionAsync(string votacionId)
            => _proyectoService.ObtenerProyectosPorVotacionAsync(votacionId);

        public Task AgregarComentarioAsync(string proyectoId, string texto, Guid? autorId = null)
            => _comentarioService.AgregarComentarioAsync(proyectoId, texto, autorId);

        public Task<List<ComentarioDto>> ObtenerComentariosAsync(string proyectoId, Guid usuarioId, string? votacionId = null)
            => _comentarioService.ObtenerComentariosAsync(proyectoId, usuarioId, votacionId);
    }
}
