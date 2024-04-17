namespace sport_management_system;

public class Sportsman
{
    public readonly string Team;
    public readonly string Name;
    public readonly string Surname;
    public readonly int YearOfBirth;
    public readonly string? Category; // разряд 
    public readonly string PreferredGroup; // желаемая группа
    public int Number;
    public int Result = 0;
    public DateTime Time;

    public Sportsman(string team, string[] fields)
    {
        this.Team = team;
        PreferredGroup = fields[0];
        Surname = fields[1];
        Name = fields[2];
        YearOfBirth = int.Parse(fields[3]);
        if (fields.Length > 4)
        {
            Category = fields[4];
        }
    }
}