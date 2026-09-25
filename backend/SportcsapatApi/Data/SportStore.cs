using SportcsapatApi.Models;

namespace SportcsapatApi.Data;

/// <summary>
/// Ideiglenes, memóriában tartott adattár. A listák a Csapatadatbázis.sql tábláinak
/// felelnek meg; amíg az adatbázis nincs bekötve, a végpontok ezeken dolgoznak.
/// </summary>
public class SportStore
{
    private readonly object _lock = new();

    public List<Team> Teams { get; } = [];
    public List<Player> Players { get; } = [];
    public List<Match> Matches { get; } = [];
    public List<PlayerMatchStat> MatchStats { get; } = [];
    public List<Training> Trainings { get; } = [];
    public List<Attendance> Attendances { get; } = [];

    public SportStore() => Seed();

    /// <summary>Minden írás/olvasás ezen keresztül megy, hogy a listák szálbiztosak maradjanak.</summary>
    public T Use<T>(Func<T> action)
    {
        lock (_lock)
        {
            return action();
        }
    }

    public int NextId<T>(List<T> items, Func<T, int> idSelector) =>
        items.Count == 0 ? 1 : items.Max(idSelector) + 1;

    private void Seed()
    {
        Teams.AddRange(
        [
            new Team { Id = 1, Name = "Sasok U14", AgeGroup = "U14", CoachName = "Kovács Péter" },
            new Team { Id = 2, Name = "Sasok U16", AgeGroup = "U16", CoachName = "Szabó Tamás" },
        ]);

        Players.AddRange(
        [
            new Player { Id = 1, Name = "Nagy Bence", BirthDate = new DateOnly(2011, 4, 12), TeamId = 1 },
            new Player { Id = 2, Name = "Tóth Dániel", BirthDate = new DateOnly(2011, 9, 3), TeamId = 1 },
        ]);

        Matches.Add(new Match
        {
            Id = 1,
            Date = new DateOnly(2025, 9, 13),
            Opponent = "Oroszlánok U14",
            Result = "3-1",
            TeamId = 1,
        });

        MatchStats.AddRange(
        [
            new PlayerMatchStat { Id = 1, MatchId = 1, PlayerId = 1, Goals = 2 },
            new PlayerMatchStat { Id = 2, MatchId = 1, PlayerId = 2, Goals = 1, YellowCards = 1 },
        ]);

        Trainings.Add(new Training
        {
            Id = 1,
            Date = new DateOnly(2025, 9, 10),
            Location = "Városi Sportpálya",
            TeamId = 1,
        });

        Attendances.AddRange(
        [
            new Attendance { Id = 1, TrainingId = 1, PlayerId = 1, WasPresent = true },
            new Attendance { Id = 2, TrainingId = 1, PlayerId = 2, WasPresent = false },
        ]);
    }
}
