using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace AttrapezLesTousApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PokemonController : ControllerBase
    {
        private readonly ILogger<PokemonController> _logger;

        public PokemonController(ILogger<PokemonController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{name}")]
        public async Task<Core.Domain.Pokemon> GetAsync(string name)
        {
            _logger.LogInformation($"Retrieving details for {name}");

            try
            {
                return await Core.Resources.GetPokemonAsync(_logger, name);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed to retrieve details for {name}: {ex.Message}");
                throw;
            }
        }

        [HttpGet("translated/{name}")]
        public async Task<Core.Domain.Pokemon> GetTranslatedAsync(string name)
        {
            _logger.LogInformation($"Retrieving translated detail for {name}");

            try
            {
                return await Core.Resources.GetTranslatedPokemonAsync(_logger, name);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed to retrieve translated details for {name}: {ex.Message}");
                throw;
            }
        }
    }
}
