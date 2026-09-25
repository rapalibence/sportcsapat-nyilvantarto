namespace SportcsapatApi.Models;

public record TeamRequest(string Name, string AgeGroup, string? CoachName);

public record TeamDetails(Team Team, IReadOnlyList<Player> Players);

public record PlayerRequest(string Name, DateOnly BirthDate, int TeamId);

public record MatchRequest(DateOnly Date, string Opponent, string? Result, int TeamId);

public record MatchDetails(Match Match, IReadOnlyList<PlayerMatchStat> Stats);

public record PlayerMatchStatRequest(int PlayerId, int Goals, int YellowCards, int RedCards);

public record TrainingRequest(DateOnly Date, string? Location, int TeamId);

public record AttendanceRequest(int PlayerId, bool WasPresent);

/// <summary>Egy játékos összesített szezonstatisztikája.</summary>
public record PlayerSummary(
    int PlayerId,
    string Name,
    int MatchesPlayed,
    int Goals,
    int YellowCards,
    int RedCards,
    int TrainingsHeld,
    int TrainingsAttended,
    int AttendancePercent);
