using Newtonsoft.Json.Linq;

namespace Pokemon.Interview.Services;

public interface IPokemonService
{
    Task<Result<object>> SimulateBattleAsync(int pokemon1Id, int pokemon2Id);
}
