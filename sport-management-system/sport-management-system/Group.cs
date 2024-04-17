using System.Globalization;
using Serilog;

namespace sport_management_system;

public class Group
{
    private static readonly ILogger Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File("../../../../../Logger.txt")
        .CreateBootstrapLogger();

    public Distance? Distance;
    private readonly string _name;
    public readonly List<Sportsman> Sportsmen;
    private readonly List<int> _numbers;
    public int WinnerResult;

    public Group(string name, List<Sportsman> sportsmenn, List<int> numbers)
    {
        _name = name;
        Sportsmen = sportsmenn;
        _numbers = numbers;
    }

    public void CreateStartProtocol() // создает стартовые протоколы для группы
    {
        try
        {
            Logger.Information($"Started to create start protocol for team {_name}");
            using var textWriter =
                new StreamWriter(@"../../../../../sample-data/startProtocols/startProtocol" + _name + ".csv");
            textWriter.WriteLine(_name);
            Random rand = new Random();
            var arr = _numbers.OrderBy(_ => rand.Next()).ToList();
            for (int i = 0; i < Sportsmen.Count; i++)
            {
                Sportsmen[i].Number = arr[i];
                DateTime time = new DateTime(1, 1, 1, 12, i + 1, 00);
                Sportsmen[i].Time = time;
                var t = time.ToString(CultureInfo.InvariantCulture).Split(" ");
                textWriter.WriteLine(arr[i] + "," + Sportsmen[i].Name + "," + Sportsmen[i].Surname + "," +
                                     Sportsmen[i].Category + "," + t[1]);
            }

            Logger.Information($"Finished to create start protocol for team {_name}");
        }
        catch
        {
            Logger.Error("Directory is not found");
            throw new DirectoryNotFoundException();
        }
    }
}