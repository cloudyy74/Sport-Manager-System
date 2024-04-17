using System.Globalization;
using System.Runtime.InteropServices;

namespace sport_management_system;

public static class Protocol
{
    private const int TimeLeftBound = 180;
    private const int TimeRightBound = 300;

    public static void CreateSplitsProtocol(Dictionary<string, Group> groups) // создает протокол с временем прохождения каждым спортсменом каждого контрольного пункта дистанции, которую он бежит
    {
        using var textWriter = new StreamWriter(@"../../../../../sample-data/splits.csv");
        foreach (var group in groups)
        {
            foreach (var sportsman in group.Value.Sportsmen)
            {
                var time = sportsman.Time;
                var t = time.ToString(CultureInfo.InvariantCulture).Split(' ');
                textWriter.Write(sportsman.Number + "," + group.Value.Distance?.Name + "," + t[1] + ",");
                Random rand = new Random();
                var checkpoints = group.Value.Distance!.Checkpoints;
                int it = 0;
                int cnt = 0;
                int len = checkpoints.Count();
                List<int> selectedCheckpoints = new List<int>();
                Console.WriteLine(len + " " + group.Value.Distance.NumberOfNecessaryCheckpoints);
                while (it < len && cnt < group.Value.Distance.NumberOfNecessaryCheckpoints)
                {
                    var oper = rand.Next(0, 1 + 1); // 0 - пропускаем несколько элементов, 1 - выбираем элемент с индексом it;
                    if (oper == 0)
                    {
                        int moveIndex = rand.Next(it, len - (group.Value.Distance.NumberOfNecessaryCheckpoints - cnt) + 1);
                        it = moveIndex;
                    }
                    else
                    {
                        selectedCheckpoints.Add(checkpoints[it]);
                        it++;
                        cnt++;
                    }
                    if (it >= len) break;
                }

                foreach (var checkpoint in selectedCheckpoints)
                {
                    var timeGen = rand.Next(TimeLeftBound, TimeRightBound);
                    sportsman.Result += timeGen;
                    var curTime = time.AddSeconds(timeGen);
                    var cur = curTime.ToString(CultureInfo.InvariantCulture).Split(' ');
                    textWriter.Write(checkpoint + "," + cur[1] + ",");
                    time = curTime;
                }

                textWriter.Write('\n');
            }
        }
    }


    public static void CreateResultProtocol(Dictionary<string, Group> groups) // создает протокол результатов по группам
    {
        using var textWriter = new StreamWriter(@"../../../../../sample-data/results.csv");
        textWriter.WriteLine("Протокол результатов.");
        foreach (var group in groups)
        {
            int k = 1;
            textWriter.WriteLine(group.Key);
            textWriter.WriteLine("\u2116,Номер,Фамилия,Имя,Г.р.,Разр.,Команда,Результат,Место,Отставание");
            List<Sportsman> sportsmen = group.Value.Sportsmen;
            var orderedSportsmen = sportsmen.OrderBy(sportsman => sportsman.Result).ToList();
            int best = 0;
            foreach (var sportsman in orderedSportsmen)
            {
                var tm = new DateTime(1, 1, 1, 0, 0, 0);
                var time = tm.AddSeconds(sportsman.Result);
                var t = time.ToString(CultureInfo.InvariantCulture).Split(' ');
                var del = new DateTime(1, 1, 1, 0, 0, 0);
                var delay = del.AddSeconds(sportsman.Result - best);
                textWriter.Write(k + "," + sportsman.Number + "," + sportsman.Surname + "," + sportsman.Name + "," +
                                 sportsman.YearOfBirth + "," + sportsman.Category + "," + sportsman.Team + "," + t[1] +
                                 "," + k);
                if (k != 1)
                {
                    textWriter.Write(",+" + delay.ToString(CultureInfo.InvariantCulture).Split(' ')[1]);
                }
                else
                {
                    best = sportsman.Result;
                    group.Value.WinnerResult = best;
                }

                k++;
                textWriter.Write(Environment.NewLine);
            }
        }
    }

    public static void CreateResultProtocolForTeams(Dictionary<string, Team> teams, Dictionary<string, Group> groups) //  создает протоколы результатов по командам, начисляя командам очки
    {
        using var textWriter = new StreamWriter(@"../../../../../sample-data/results_team.csv");
        var teamsWithResults = new List<Team>();
        textWriter.WriteLine("Протокол результатов для команд.");
        textWriter.WriteLine("Место,Название,Результат");
        foreach (var team in teams)
        {
            double teamResult = 0;
            List<Sportsman> sportsmen = team.Value.Sportsmen;
            foreach (var sportsman in sportsmen)
            {
                double result = Math.Max(0.0,
                    100.0 * (2.0 -
                             (double)groups[sportsman.PreferredGroup]
                                 .Sportsmen[
                                     groups[sportsman.PreferredGroup].Sportsmen.FindIndex(x =>
                                         x.Surname == sportsman.Surname && x.Name == sportsman.Name)].Result /
                             groups[sportsman.PreferredGroup].WinnerResult));
                teamResult += result;
            }

            team.Value.Result = Math.Round(teamResult, 1);
            teamsWithResults.Add(team.Value);
        }

        var orederedTeams = teamsWithResults.OrderByDescending(team => team.Result);
        int k = 1;
        foreach (var team in orederedTeams)
        {
            textWriter.WriteLine(k + "," + team.Name + "," + team.Result);
            k++;
        }
    }
}