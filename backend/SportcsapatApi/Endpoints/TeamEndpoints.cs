using SportcsapatApi.Data;
using SportcsapatApi.Models;

namespace SportcsapatApi.Endpoints;

public static class TeamEndpoints
{
    public static RouteGroupBuilder MapTeamEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/teams").WithTags("Csapatok");

        group.MapGet("/", (SportStore store) =>
                Results.Ok(store.Use(() => store.Teams.ToList())))
            .WithName("GetTeams");

        group.MapGet("/{id:int}", (int id, SportStore store) => store.Use(() =>
            {
                var team = store.Teams.FirstOrDefault(t => t.Id == id);
                if (team is null) return Results.NotFound();

                var players = store.Players.Where(p => p.TeamId == id).OrderBy(p => p.Name).ToList();
                return Results.Ok(new TeamDetails(team, players));
            }))
            .WithName("GetTeam");

        group.MapPost("/", (TeamRequest request, SportStore store) =>
            {
                var error = Validate(request);
                if (error is not null) return Results.BadRequest(error);

                var team = store.Use(() =>
                {
                    var created = new Team { Id = store.NextId(store.Teams, t => t.Id) };
                    Apply(created, request);
                    store.Teams.Add(created);
                    return created;
                });

                return Results.Created($"/api/teams/{team.Id}", team);
            })
            .WithName("CreateTeam");

        // Az edző neve a csapat adata (csapat.EdzoNeve), ezen keresztül állítható.
        group.MapPut("/{id:int}", (int id, TeamRequest request, SportStore store) =>
            {
                var error = Validate(request);
                if (error is not null) return Results.BadRequest(error);

                return store.Use(() =>
                {
                    var team = store.Teams.FirstOrDefault(t => t.Id == id);
                    if (team is null) return Results.NotFound();

                    Apply(team, request);
                    return Results.Ok(team);
                });
            })
            .WithName("UpdateTeam");

        return group;
    }

    private static string? Validate(TeamRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return "A csapat neve kötelező.";
        if (request.Name.Length > 100)
            return "A csapat neve legfeljebb 100 karakter lehet.";
        if (request.AgeGroup.Length > 50)
            return "A korosztály legfeljebb 50 karakter lehet.";
        if (request.CoachName?.Length > 100)
            return "Az edző neve legfeljebb 100 karakter lehet.";
        return null;
    }

    private static void Apply(Team team, TeamRequest request)
    {
        team.Name = request.Name.Trim();
        team.AgeGroup = request.AgeGroup.Trim();
        team.CoachName = request.CoachName?.Trim();
    }
}
