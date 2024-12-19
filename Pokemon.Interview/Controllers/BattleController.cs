using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Pokemon.Interview.Controllers;
using Pokemon.Interview.Services;
using System.Threading.Tasks;

[Route("api/pokemon")]
public class BattleController : BaseController
{
    private readonly IPokemonService _pokemonService;

    public BattleController(IPokemonService pokemonService)
    {
        _pokemonService = pokemonService;
    }

    [HttpGet("battle/{pokemon1Id}/{pokemon2Id}")]
    public Task<IActionResult> SimulateBattle(int pokemon1Id, int pokemon2Id)
    {
        return ToActionResult(_pokemonService.SimulateBattleAsync(pokemon1Id, pokemon2Id));
    }
}
