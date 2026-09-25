using SportcsapatApi.Data;
using SportcsapatApi.Models;

namespace SportcsapatApi.Endpoints;

public static class MatchEndpoints
{
    public static RouteGroupBuilder MapMatchEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/matches").WithTags("Mérkőzések");

        // Ezt bárki megnézheti: eredmények a nyilvános oldalon.
        group.MapGet("/", (int? teamId, SportStore store) => Results.Ok(store.Use(() =>
                store.Matches
                    .Where(m => teamId is null || m.TeamId == teamId)
                    .OrderByDescending(m => m.Date)
                    .ToList())))
            .WithName("GetMatches");

        group.MapGet("/{id:int}", (int id, SportStore store) => store.Use(() =>
            {
                var match = store.Matches.FirstOrDefault(m => m.Id == id);
                if (match is null) return Results.NotFound();

                var stats = store.MatchStats.Where(s => s.MatchId == id).ToList();
                return Results.Ok(new MatchDetails(match, stats));
            }))
            .WithName("GetMatch");

        group.MapPost("/", (MatchRequest request, SportStore store) =>
            {
                var error = Validate(request, store);
                if (error is not null) return Results.BadRequest(error);

                var match = store.Use(() =>
                {
                    var created = new Match { Id = store.NextId(store.Matches, m => m.Id) };
                    Apply(created, request);
                    store.Matches.Add(created);
                    return created;
                });

                return Results.Created($"/api/matches/{match.Id}", match);
            })
            .WithName("CreateMatch");

        // Az edző a meccs után rögzíti/javítja az eredményt.
        group.MapPut("/{id:int}", (int id, MatchRequest request, SportStore store) =>
            {
                var error = Validate(request, store);
                if (error is not null) return Results.BadRequest(error);

                return store.Use(() =>
                {
                    var match = store.Matches.FirstOrDefault(m => m.Id == id);
                    if (match is null) return Results.NotFound();

                    Apply(match, request);
                    return Results.Ok(match);
                });
            })
            .WithName("UpdateMatch");

        group.MapGet("/{id:int}/stats", (int id, SportStore store) => store.Use(() =>
                store.Matches.All(m => m.Id != id)
                    ? Results.NotFound()
                    : Results.Ok(store.MatchStats.Where(s => s.MatchId == id).ToList())))
            .WithName("GetMatchStats");

        // Egy játékos teljesítménye a meccsen. Ha már van sora, felülírjuk –
        // így a hibásan felvitt adat javítható.
        group.MapPost("/{id:int}/stats", (int id, PlayerMatchStatRequest request, SportStore store) => store.Use(() =>
            {
                var match = store.Matches.FirstOrDefault(m => m.Id == id);
                if (match is null) return Results.NotFound("Nincs ilyen mérkőzés.");

                var player = store.Players.FirstOrDefault(p => p.Id == request.PlayerId);
                if (player is null) return Results.BadRequest("Nincs ilyen játékos.");
                if (player.TeamId != match.TeamId)
                    return Results.BadRequest("A játékos nem ennek a csapatnak a tagja.");
                if (request.Goals < 0 || request.YellowCards < 0 || request.RedCards < 0)
                    return Results.BadRequest("A statisztikák nem lehetnek negatívak.");

                var stat = store.MatchStats.FirstOrDefault(s => s.MatchId == id && s.PlayerId == request.PlayerId);
                if (stat is null)
                {
                    stat = new PlayerMatchStat
                    {
                        Id = store.NextId(store.MatchStats, s => s.Id),
                        MatchId = id,
                        PlayerId = request.PlayerId,
                    };
                    store.MatchStats.Add(stat);
                }

                stat.Goals = request.Goals;
                stat.YellowCards = request.YellowCards;
                stat.RedCards = request.RedCards;
                return Results.Ok(stat);
            }))
            .WithName("SaveMatchStat");

        return group;
    }

    /// <summary>Az eredmény "3-1" alakú, a saját csapat góljai elöl.</summary>
    private static bool IsValidResult(string result)
    {
        var parts = result.Split('-');
        return parts.Length == 2 &&
               parts.All(part => part.Length is > 0 and <= 3 && part.All(char.IsAsciiDigit));
    }

    private static string? Validate(MatchRequest request, SportStore store)
    {
        if (string.IsNullOrWhiteSpace(request.Opponent))
            return "Az ellenfél neve kötelező.";
        if (request.Opponent.Length > 150)
            return "Az ellenfél neve legfeljebb 150 karakter lehet.";
        if (store.Use(() => store.Teams.All(t => t.Id != request.TeamId)))
            return "Nincs ilyen csapat.";
        if (!string.IsNullOrWhiteSpace(request.Result) && !IsValidResult(request.Result.Trim()))
            return "Az eredményt 3-1 alakban kell megadni.";
        return null;
    }

    private static void Apply(Match match, MatchRequest request)
    {
        match.Date = request.Date;
        match.Opponent = request.Opponent.Trim();
        match.Result = string.IsNullOrWhiteSpace(request.Result) ? null : request.Result.Trim();
        match.TeamId = request.TeamId;
    }
}
