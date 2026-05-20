using Votify.Application.Interfaces;

namespace Votify.Application.Services.Estrategia
{
    public class VotacionStrategyResolver
    {
        private readonly IEnumerable<IVotacionStrategy> _strategies;

        public VotacionStrategyResolver(IEnumerable<IVotacionStrategy> strategies)
        {
            _strategies = strategies;
        }

        public IVotacionStrategy Resolver(string tipo)
        {
            var t = tipo?.Trim().ToUpper() ?? string.Empty;
            var strategy = _strategies.FirstOrDefault(s =>
                string.Equals(s.Tipo, t, StringComparison.OrdinalIgnoreCase));

            if (strategy is null)
            {
                throw new ArgumentException($"No existe estrategia para el tipo de votación: {tipo}");
            }

            return strategy;
        }
    }
}
