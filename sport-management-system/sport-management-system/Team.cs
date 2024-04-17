namespace sport_management_system;

public class Team
{
    public readonly string Name;
    public readonly List<Sportsman> Sportsmen;
    public double Result;

    public Team(string name, List<Sportsman> team)
    {
        Name = name;
        Sportsmen = team;
    }
}