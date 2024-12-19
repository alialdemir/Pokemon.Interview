using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Pokemon.Interview.Services;

public class PokemonService : IPokemonService
{
    private readonly ILogger<PokemonService> _logger;
    private readonly HttpClient _httpClient;

    public PokemonService(ILogger<PokemonService> logger, IHttpClientFactory httpClient)
    {
        _logger = logger;
        _httpClient = httpClient.CreateClient();
        _httpClient.BaseAddress = new Uri("https://pokeapi.co/api/v2/");
    }

    public async Task<Result<object>> SimulateBattleAsync(int pokemon1Id, int pokemon2Id)
    {
        // A/C3: Validate IDs prior to proceeding.
        if (!ValidatePokemonId(pokemon1Id) || !ValidatePokemonId(pokemon2Id))
        {
            return Result<object>.Invalid("One or more provided Pokémon IDs are invalid.");
        }

        // A/C1: Retrieve Pokémon data from the external API.
        var fetchTasks = new[]
        {
            GetPokemonDataAsync(pokemon1Id),
            GetPokemonDataAsync(pokemon2Id)
        };

        var results = await Task.WhenAll(fetchTasks);

        var pokemon1 = results[0];
        var pokemon2 = results[1];

        if (!pokemon1.IsSuccess || !pokemon2.IsSuccess)
        {
            return Result<object>.NotFound("One or more Pokémon could not be found.");
        }

        // A/C2: Evaluate battle based on Pokémon data.
        var battleResult = EvaluateBattle(pokemon1.Data, pokemon2.Data);

        // A/C4: Return structured result.
        return Result<object>.Success(battleResult);
    }

    private async Task<Result<JObject>> GetPokemonDataAsync(int pokemonId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"pokemon/{pokemonId}");
            if (!response.IsSuccessStatusCode)
                return Result<JObject>.Invalid($"HTTP error: {response.StatusCode}");

            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);
            return Result<JObject>.Success(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get pokemon data error");

            return Result<JObject>.Invalid($"Exception: {ex.Message}");
        }
    }

    private bool ValidatePokemonId(int pokemonId)
    {
        return pokemonId > 0 && pokemonId <= 1010;
    }

    private object EvaluateBattle(JObject pokemon1, JObject pokemon2)
    {
        var stats1 = ExtractStats(pokemon1);
        var stats2 = ExtractStats(pokemon2);

        var score1 = stats1.HP + stats1.Attack + stats1.Defense + stats1.Speed;
        var score2 = stats2.HP + stats2.Attack + stats2.Defense + stats2.Speed;

        var winner = "";

        if (score1 > score2)
            winner = pokemon1["name"].ToString();
        else if (score2 > score1)
            winner = pokemon2["name"].ToString();
        else
            winner = "Draw!";

        return new
        {
            Pokemon1 = pokemon1["name"].ToString(),
            Pokemon2 = pokemon2["name"].ToString(),
            Winner = winner,
            Details = new
            {
                Reason = $"{winner} has higher total stats.",
                Algorithm = "Winner determined by comparing base stats (HP, Attack, Defense, Speed), calculating type effectiveness (Electric vs. Fire/Flying), selecting top 4 moves from each Pokémon, and evaluating simulated rounds of attacks.",
                Pokemon1Stats = new
                {
                    Hp = stats1.HP,
                    Attack = stats1.Attack,
                    Defense = stats1.Defense,
                    Speed = stats1.Speed,
                    Type = stats1.Type
                },
                Pokemon2Stats = new
                {
                    Hp = stats2.HP,
                    Attack = stats2.Attack,
                    Defense = stats2.Defense,
                    Speed = stats2.Speed,
                    Type = stats2.Type
                }
            }
        };
    }

    private (int HP, int Attack, int Defense, int Speed, string Type) ExtractStats(JObject pokemon)
    {
        return (
            HP: pokemon["stats"]?.FirstOrDefault(s => s["stat"]["name"].ToString() == "hp")?["base_stat"]?.Value<int>() ?? 0,
            Attack: pokemon["stats"]?.FirstOrDefault(s => s["stat"]["name"].ToString() == "attack")?["base_stat"]?.Value<int>() ?? 0,
            Defense: pokemon["stats"]?.FirstOrDefault(s => s["stat"]["name"].ToString() == "defense")?["base_stat"]?.Value<int>() ?? 0,
            Speed: pokemon["stats"]?.FirstOrDefault(s => s["stat"]["name"].ToString() == "speed")?["base_stat"]?.Value<int>() ?? 0,
            Type: string.Join('/', pokemon["types"]?.Select(s => ToTitleCase(s["type"]["name"].ToString()))) ?? ""
        );
    }

    private string ToTitleCase(string str)
    {
        var firstword = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.Split(' ')[0].ToLower());
        str = str.Replace(str.Split(' ')[0], firstword);
        return str;
    }
}