namespace SportcsapatApi.Models;

// A típusok a Csapatadatbázis.sql tábláit követik:
// csapat, jatekos, merkozes, jatekosstatisztika, edzes, jelenlet.

/// <summary>csapat</summary>
public class Team
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string AgeGroup { get; set; } = "";
    public string? CoachName { get; set; }
}

/// <summary>jatekos</summary>
public class Player
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public DateOnly BirthDate { get; set; }
    public int TeamId { get; set; }
}

/// <summary>merkozes</summary>
public class Match
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public string Opponent { get; set; } = "";

    /// <summary>Az eredmény a saját csapat szemszögéből, "3-1" alakban.</summary>
    public string? Result { get; set; }

    public int TeamId { get; set; }
}

/// <summary>jatekosstatisztika</summary>
public class PlayerMatchStat
{
    public int Id { get; set; }
    public int MatchId { get; set; }
    public int PlayerId { get; set; }
    public int Goals { get; set; }
    public int YellowCards { get; set; }
    public int RedCards { get; set; }
}

/// <summary>edzes</summary>
public class Training
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public string? Location { get; set; }
    public int TeamId { get; set; }
}

/// <summary>jelenlet</summary>
public class Attendance
{
    public int Id { get; set; }
    public int TrainingId { get; set; }
    public int PlayerId { get; set; }
    public bool WasPresent { get; set; }
}
