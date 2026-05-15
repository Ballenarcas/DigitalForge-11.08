using Votify.Application.DTOs;
using Votify.Application.Interfaces;
using Votify.Domain.Interfaces;

namespace Votify.Application.Services.Fachadas
{
    public class ProyectoFachada : IProyectoFachada
    {
        private readonly IProyectoService _proyectoService;
        private readonly IComentarioService _comentarioService;
        private readonly IResumidorComentariosIA _summarizer;

        public ProyectoFachada(
            IProyectoService proyectoService,
            IComentarioService comentarioService,
            IResumidorComentariosIA summarizer)
        {
            _proyectoService = proyectoService;
            _comentarioService = comentarioService;
            _summarizer = summarizer;
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

        public async Task<ResumenComentario> ObtenerResumenComentariosAsync(string proyectoId)
        {
            var comentarios = await _comentarioService.ObtenerComentariosParaResumenAsync(proyectoId);
            var proyecto = await _proyectoService.ObtenerProyectoAsync(proyectoId);

            var items = comentarios.Select(c => new ComentarioResumenItem
            {
                Texto = c.Texto,
                AutorNombre = c.AutorNombre,
                EsAnonimo = c.EsAnonimo,
                FechaCreacion = c.FechaCreacion
            }).ToList();

            return await _summarizer.ResumirComentariosAsync(items, proyecto?.Nombre ?? "Proyecto");
        }
    }
}
