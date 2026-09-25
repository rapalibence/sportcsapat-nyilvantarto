using SportcsapatApi.Data;
using SportcsapatApi.Models;

namespace SportcsapatApi.Endpoints;

public static class TrainingEndpoints
{
    public static RouteGroupBuilder MapTrainingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/trainings").WithTags("Edzések");

        group.MapGet("/", (int? teamId, SportStore store) => Results.Ok(store.Use(() =>
                store.Trainings
                    .Where(t => teamId is null || t.TeamId == teamId)
                    .OrderByDescending(t => t.Date)
                    .ToList())))
            .WithName("GetTrainings");

        group.MapPost("/", (TrainingRequest request, SportStore store) => store.Use(() =>
            {
                if (store.Teams.All(t => t.Id != request.TeamId))
                    return Results.BadRequest("Nincs ilyen csapat.");
                if (request.Location?.Length > 150)
                    return Results.BadRequest("A helyszín legfeljebb 150 karakter lehet.");

                var training = new Training
                {
                    Id = store.NextId(store.Trainings, t => t.Id),
                    Date = request.Date,
                    Location = request.Location?.Trim(),
                    TeamId = request.TeamId,
                };
                store.Trainings.Add(training);
                return Results.Created($"/api/trainings/{training.Id}", training);
            }))
            .WithName("CreateTraining");

        group.MapGet("/{id:int}/attendance", (int id, SportStore store) => store.Use(() =>
                store.Trainings.All(t => t.Id != id)
                    ? Results.NotFound()
                    : Results.Ok(store.Attendances.Where(a => a.TrainingId == id).ToList())))
            .WithName("GetTrainingAttendance");

        // Az edző itt pipálja ki a jelenlétet; egy játékos egy edzésen egyszer szerepel.
        group.MapPost("/{id:int}/attendance", (int id, AttendanceRequest[] requests, SportStore store) =>
                store.Use(() =>
                {
                    var training = store.Trainings.FirstOrDefault(t => t.Id == id);
                    if (training is null) return Results.NotFound("Nincs ilyen edzés.");

                    var teamPlayerIds = store.Players
                        .Where(p => p.TeamId == training.TeamId)
                        .Select(p => p.Id)
                        .ToHashSet();

                    var idegen = requests.Select(r => r.PlayerId).Where(pid => !teamPlayerIds.Contains(pid)).ToList();
                    if (idegen.Count > 0)
                        return Results.BadRequest($"Nem a csapat tagjai: {string.Join(", ", idegen)}");

                    foreach (var request in requests)
                    {
                        var entry = store.Attendances
                            .FirstOrDefault(a => a.TrainingId == id && a.PlayerId == request.PlayerId);

                        if (entry is null)
                        {
                            entry = new Attendance
                            {
                                Id = store.NextId(store.Attendances, a => a.Id),
                                TrainingId = id,
                                PlayerId = request.PlayerId,
                            };
                            store.Attendances.Add(entry);
                        }

                        entry.WasPresent = request.WasPresent;
                    }

                    return Results.Ok(store.Attendances.Where(a => a.TrainingId == id).ToList());
                }))
            .WithName("SaveTrainingAttendance");

        return group;
    }
}
