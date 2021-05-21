using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<Domain.Types.Pokemon> GetAsync(string name)
        {
            _logger.LogInformation($"Retrieving detail for {name}");

            try
            {
                return await APIs.PokeAPI.GetPokemonAsync(name);
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed to retrieve detail for {name}: {ex.Message}");
                throw;
            }
        }
    }
}
