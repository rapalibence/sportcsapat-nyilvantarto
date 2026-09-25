using SportcsapatApi.Data;
using SportcsapatApi.Models;

namespace SportcsapatApi.Endpoints;

public static class PlayerEndpoints
{
    public static RouteGroupBuilder MapPlayerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/players").WithTags("Játékosok");

        group.MapGet("/", (int? teamId, SportStore store) => Results.Ok(store.Use(() =>
                store.Players
                    .Where(p => teamId is null || p.TeamId == teamId)
                    .OrderBy(p => p.Name)
                    .ToList())))
            .WithName("GetPlayers");

        group.MapGet("/{id:int}", (int id, SportStore store) => store.Use(() =>
            {
                var player = store.Players.FirstOrDefault(p => p.Id == id);
                return player is null ? Results.NotFound() : Results.Ok(player);
            }))
            .WithName("GetPlayer");

        group.MapPost("/", (PlayerRequest request, SportStore store) =>
            {
                var error = Validate(request, store);
                if (error is not null) return Results.BadRequest(error);

                var player = store.Use(() =>
                {
                    var created = new Player { Id = store.NextId(store.Players, p => p.Id) };
                    Apply(created, request);
                    store.Players.Add(created);
                    return created;
                });

                return Results.Created($"/api/players/{player.Id}", player);
            })
            .WithName("CreatePlayer");

        // A szülők a saját gyerekük adatait ezen keresztül tudják javítani.
        group.MapPut("/{id:int}", (int id, PlayerRequest request, SportStore store) =>
            {
                var error = Validate(request, store);
                if (error is not null) return Results.BadRequest(error);

                return store.Use(() =>
                {
                    var player = store.Players.FirstOrDefault(p => p.Id == id);
                    if (player is null) return Results.NotFound();

                    Apply(player, request);
                    return Results.Ok(player);
                });
            })
            .WithName("UpdatePlayer");

        group.MapDelete("/{id:int}", (int id, SportStore store) => store.Use(() =>
            {
                var player = store.Players.FirstOrDefault(p => p.Id == id);
                if (player is null) return Results.NotFound();

                store.Players.Remove(player);
                store.MatchStats.RemoveAll(s => s.PlayerId == id);
                store.Attendances.RemoveAll(a => a.PlayerId == id);
                return Results.NoContent();
            }))
            .WithName("DeletePlayer");

        // Szezonösszesítő: meccsstatisztikák + edzéslátogatottság egy játékosra.
        group.MapGet("/{id:int}/summary", (int id, SportStore store) => store.Use(() =>
            {
                var player = store.Players.FirstOrDefault(p => p.Id == id);
                if (player is null) return Results.NotFound();

                var stats = store.MatchStats.Where(s => s.PlayerId == id).ToList();
                var trainingsHeld = store.Trainings.Count(t => t.TeamId == player.TeamId);
                var attended = store.Attendances.Count(a =>
                    a.PlayerId == id &&
                    a.WasPresent &&
                    store.Trainings.Any(t => t.Id == a.TrainingId));

                return Results.Ok(new PlayerSummary(
                    player.Id,
                    player.Name,
                    stats.Count,
                    stats.Sum(s => s.Goals),
                    stats.Sum(s => s.YellowCards),
                    stats.Sum(s => s.RedCards),
                    trainingsHeld,
                    attended,
                    trainingsHeld == 0 ? 0 : (int)Math.Round(attended * 100.0 / trainingsHeld)));
            }))
            .WithName("GetPlayerSummary");

        return group;
    }

    private static string? Validate(PlayerRequest request, SportStore store)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return "A játékos neve kötelező.";
        if (request.Name.Length > 100)
            return "A játékos neve legfeljebb 100 karakter lehet.";
        if (store.Use(() => store.Teams.All(t => t.Id != request.TeamId)))
            return "Nincs ilyen csapat.";
        if (request.BirthDate > DateOnly.FromDateTime(DateTime.Today))
            return "A születési dátum nem lehet a jövőben.";
        return null;
    }

    private static void Apply(Player player, PlayerRequest request)
    {
        player.Name = request.Name.Trim();
        player.BirthDate = request.BirthDate;
        player.TeamId = request.TeamId;
    }
}
