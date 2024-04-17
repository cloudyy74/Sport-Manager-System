using Serilog;

namespace sport_management_system;

public class SportsManegementSystem
{
    private static readonly ILogger Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File("../../../../../Logger.txt")
        .CreateBootstrapLogger();

    public static void Main(string[] args)
        // args[0] - folder with applications
        // args[1] - group -> distance
        // args[2] - distances
        // args[3] - types
    {
        Logger.Information("program started");
        var test = new Util.FolderParser(args[0]);
        var applications = test.Parser();
        var (groups, teams) = Util.InputData.InputApplications(applications);
        foreach (var group in groups)
        {
            group.Value.CreateStartProtocol();
        }
    
        var distances = Util.InputData.InputDistances(args[1]);
        Util.InputData.InputClasses(args[2], groups, distances);
        Util.InputData.InputTypes(args[3], groups);
        foreach (var dist in distances.Keys)
        {
            Console.WriteLine(distances[dist].NumberOfNecessaryCheckpoints);
        }
        Protocol.CreateSplitsProtocol(groups);
        Console.WriteLine(1);
        Protocol.CreateResultProtocol(groups);
        Protocol.CreateResultProtocolForTeams(teams, groups);
        Logger.Information("program finished");
    }
}