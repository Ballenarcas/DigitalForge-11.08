using Votify.Application.DTOs;
using Votify.Application.Interfaces;
using Votify.Domain.Factory;
using Votify.Domain.Interfaces;

namespace Votify.Application.Services
{
    public class VotacionService : IVotacionService
    {
        private readonly IVotacionRepository _repo;
        private readonly IVotoRepository _votoRepo;
        private readonly IProyectoRepository _proyectoRepo;

        public VotacionService(IVotacionRepository repo, IVotoRepository votoRepo, IProyectoRepository proyectoRepo)
        {
            _repo = repo;
            _votoRepo = votoRepo;
            _proyectoRepo = proyectoRepo;
        }

        public async Task CrearVotacionAsync(CrearVotacionDto dto)
        {
            if (dto.FechaInicio >= dto.FechaFin)
            {
                throw new ArgumentException("La fecha de inicio debe ser menor a la fecha de fin.");
            }
            VotacionFactory factory = dto.Tipo.ToUpper() switch
            {
                "ESTANDAR" => new VotacionEstandarFactory(),
                "ANONIMA" => new VotacionAnonimaFactory(),
                _ => throw new ArgumentException("Tipo de votación no válido.")
            };

            var votacion = factory.Crear(
                dto.Nombre,
                dto.FechaInicio,
                dto.FechaFin,
                dto.LimiteProyectos,
                dto.PermiteComentarios
            );
            
            await _repo.GuardarAsync(votacion);
        }
        public async Task<List<CrearVotacionResponse>> ObtenerTodasAsync()
        {
            var entidades = await _repo.ObtenerTodasAsync();

            return entidades.Select(e => new CrearVotacionResponse
            {
                Id = e.Id.ToString(), 
                Nombre = e.Nombre,
                Tipo = e.Tipo(),
                FechaInicio = e.FechaInicio,
                FechaFin = e.FechaFin,
                LimiteProyectos = e.LimiteProyectos,
                PermiteComentarios = e.PermiteComentarios
            }).ToList();
        }

        public async Task<CrearVotacionResponse?> ObtenerPorIdAsync(string id)
        {
            var e = await _repo.ObtenerAsync(id);
            if (e is null) return null;

            return new CrearVotacionResponse
            {
                Id = e.Id.ToString(),
                Nombre = e.Nombre,
                Tipo = e.Tipo(),
                FechaInicio = e.FechaInicio,
                FechaFin = e.FechaFin,
                LimiteProyectos = e.LimiteProyectos,
                PermiteComentarios = e.PermiteComentarios
            };
        }
        public async Task ActualizarVotacionAsync(string id, CrearVotacionDto dto)
        {
            if (dto.FechaInicio >= dto.FechaFin)
                throw new ArgumentException("La fecha de inicio debe ser menor a la fecha de fin.");

            VotacionFactory factory = dto.Tipo.ToUpper() switch
            {
                "ESTANDAR" => new VotacionEstandarFactory(),
                "ANONIMA"  => new VotacionAnonimaFactory(),
                _          => throw new ArgumentException("Tipo de votación no válido.")
            };

            var votacion = factory.Crear(
                dto.Nombre,
                dto.FechaInicio,
                dto.FechaFin,
                dto.LimiteProyectos,
                dto.PermiteComentarios
            );

            var actualizado = await _repo.ActualizarAsync(id, votacion);
            if (!actualizado)
                throw new KeyNotFoundException($"No se encontró la votación con id {id}.");
        }

        public async Task EliminarVotacionAsync(string id)
        {

            var votosEliminados = await _votoRepo.EliminarPorVotacionAsync(id);
            

            var eliminado = await _repo.EliminarAsync(id);
            if (!eliminado)
                throw new KeyNotFoundException($"No se encontró la votación con id {id}.");
        }

        public async Task<List<ResultadoProyectoDto>> ObtenerResultadosAsync(string votacionId)
        {
            var votosporProyecto = await _votoRepo.ObtenerVotosPorVotacionAsync(votacionId);

            if (votosporProyecto.Count == 0)
                return new List<ResultadoProyectoDto>();

            var proyectos = await _proyectoRepo.ObtenerPorVotacionAsync(votacionId);
            var proyectoDict = proyectos.ToDictionary(p => p.Id);

            var resultados = votosporProyecto
                .Select((vp, index) => new ResultadoProyectoDto
                {
                    Id = vp.ProyectoId,
                    Nombre = proyectoDict.ContainsKey(vp.ProyectoId) ? proyectoDict[vp.ProyectoId].Nombre : "Proyecto desconocido",
                    Equipo = proyectoDict.ContainsKey(vp.ProyectoId) ? (proyectoDict[vp.ProyectoId].Equipo_Id ?? "Sin equipo") : "Sin equipo",
                    TotalVotos = vp.Votos,
                    Posicion = index + 1
                })
                .ToList();

            return resultados;
        }
    }
}
