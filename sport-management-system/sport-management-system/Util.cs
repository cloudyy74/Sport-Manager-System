using Microsoft.VisualBasic.FileIO;
using Serilog;

namespace sport_management_system;

public abstract class Util
{
    private static readonly ILogger Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.File("../../../../../Logger.txt")
        .CreateBootstrapLogger();

    public class FolderParser // путь к папке
    {
        private readonly string _pathFold;
        public FolderParser(string pathFold) => _pathFold = pathFold;

        public string[] Parser() // достает файлы из указанной папки
        {
            Logger.Information("The parser start");
            if (Directory.Exists(_pathFold))
            {
                Logger.Information("the directory is found");
                string[] listFiles = Directory.EnumerateFiles(_pathFold).ToArray();
                if (listFiles.Length == 0)
                {
                    Logger.Error($"the directory {_pathFold} is EMPTY");
                    throw new IOException("the directory is EMPTY");
                }

                return listFiles;
            }
            else
            {
                Logger.Error("the directory is NOT found");
                throw new DirectoryNotFoundException();
            }
        }
    }

    public class InputData // считывание и обработка входных данных
    {
        public static Dictionary<string, Distance> InputDistances(string file) // считывает контрольные пункты для каждой дистанции
        {
            var distances = new Dictionary<string, Distance>();
            using TextFieldParser parser = new TextFieldParser(file);
            parser.SetDelimiters(",");
            parser.ReadFields();
            parser.TextFieldType = FieldType.Delimited;
            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields()!;
                List<int> checkpoints = new List<int>();
                for (int i = 1; i < fields.Length; i++)
                {
                    if (fields[i] == "") break;
                    checkpoints.Add(int.Parse(fields[i]));
                }

                distances.Add(fields[0], new Distance(fields[0], checkpoints));
            }

            return distances;
        }

        public static Tuple<Dictionary<string, Group>, Dictionary<string, Team>> InputApplications(string[] files) // считывает заявки по командам
        {
            Logger.Information("Input of applications started");
            var groups = new Dictionary<string, Group>();
            var teams = new Dictionary<string, Team>();
            var members = new List<Sportsman>();
            try
            {
                foreach (var t in files)
                {
                    var membersOfTeam = new List<Sportsman>();
                    using TextFieldParser parser = new TextFieldParser(t);
                    parser.SetDelimiters(",");
                    string name = parser.ReadFields()![0];
                    parser.ReadFields();
                    parser.TextFieldType = FieldType.Delimited;
                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields()!;
                        members.Add(new Sportsman(name, fields));
                        membersOfTeam.Add(new Sportsman(name, fields));
                    }

                    teams.Add(name, new Team(name, membersOfTeam));
                }

                var groupsDict = GroupByGroup(members);
                int k = 1;
                foreach (var group in groupsDict)
                {
                    var numbers = Enumerable.Range(k, group.Value.Count).ToList();
                    groups.Add(group.Key, new Group(group.Key, group.Value, numbers));
                    k = k + group.Value.Count;
                }

                Logger.Information("Input of applications finished");
                return new Tuple<Dictionary<string, Group>, Dictionary<string, Team>>(groups, teams);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error in the sorting");
                throw;
            }
        }

        private static Dictionary<string, List<Sportsman>> GroupByGroup(List<Sportsman> sportsmen)
            => sportsmen.GroupBy(group => group.PreferredGroup).ToDictionary(q => q.Key, q => q.ToList()); // группирует спортсменов в соответствии с предпочитаемой группой

        public static void InputClasses(string file, Dictionary<string, Group> groups,
            Dictionary<string, Distance> dist) // для каждой группы считывает дистанцию, которую бегут спортсмены этой группы
        {
            using TextFieldParser parser = new TextFieldParser(file);
            parser.SetDelimiters(",");
            parser.ReadFields();
            parser.TextFieldType = FieldType.Delimited;
            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields()!;
                for (int cname = 0; cname < fields.Length / 2; cname += 2)
                {
                    foreach (string k in groups.Keys)
                    {
                        if (fields[cname] == k)
                        {
                            groups[k].Distance =
                                new Distance(fields[cname + 1], dist[fields[cname + 1]].Checkpoints);
                            break;
                        }
                    }
                }
            }
        }
        public static void InputTypes(string file, Dictionary<string, Group> dist)
        {
            using TextFieldParser parser = new TextFieldParser(file);
            parser.SetDelimiters(",");
            parser.ReadFields();
            parser.TextFieldType = FieldType.Delimited;
            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields()!;
                foreach (var name in dist.Keys)
                {
                    if (fields[0] == name)
                    {
                        dist[name].Distance!.NumberOfNecessaryCheckpoints = int.Parse(fields[2]);
                        if (fields[1] == "yes")
                        {
                            dist[name].Distance!.IsCircle = true;
                        }
                        else
                        {
                            dist[name].Distance!.IsCircle = false;
                        }
                    }
                    continue;
                }
            }
        }
    }
}