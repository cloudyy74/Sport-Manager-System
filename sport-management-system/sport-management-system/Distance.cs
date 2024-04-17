namespace sport_management_system;

public class Distance
{
    public readonly string Name;
    public readonly List<int> Checkpoints;
    public bool IsCircle;
    public int NumberOfNecessaryCheckpoints;
    public Distance(string name, List<int> checkpoints)
    {
        Name = name;
        Checkpoints = checkpoints;
    }
}