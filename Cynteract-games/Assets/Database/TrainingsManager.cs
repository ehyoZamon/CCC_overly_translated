using Cynteract.CCC;
using Cynteract.CCC.Charts;
using Cynteract.Database;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class TrainingsManager : MonoBehaviour
{
    public static TrainingsManager instance;
    [ShowInInspector]
    public TrainingsData Training { get; private set; }
    public readonly static TimeSpan timeToLookBack = TimeSpan.FromHours(12);
    public readonly static TimeSpan dailyTrainingTime = TimeSpan.FromMinutes(20);
    public bool startedTraining = false;
    private void Awake()
    {
        instance = this;
    }
    [Button]
    public void StartNewTraining(string game)
    {
        startedTraining = true;
        Training = new TrainingsData();
        Training.Start = DateTime.Now;
        Training.GameName = game;
        //Training.UserID = LocalDatabaseManager.instance.currentUser.Id;
    }

    public TrainingGame[] GetGames()
    {
        var games = CynteractControlCenter.instance.cGames;
        TrainingGame[] trainingGames = new TrainingGame[games.Length];

        for (int i = 0; i < trainingGames.Length; i++)
        {
            trainingGames[i] = new TrainingGame();
            trainingGames[i].cGame = games[i];
#warning choose Index of Input set here
            trainingGames[i].inputSet = games[i].inputSets[0];
            trainingGames[i].time = TimeSpan.FromMinutes(4);
        }

        return trainingGames;
    }
    public TrainingGame[] GetRandomGames(int number)
    {
        var games = CynteractControlCenter.instance.cGames;
        TrainingGame[] trainingGames = new TrainingGame[number];
        var gamesPool = games.ToList();
        System.Random random = new System.Random();
        for (int i = 0; i < trainingGames.Length; i++)
        {
            trainingGames[i] = new TrainingGame();
            int index = random.Next(gamesPool.Count);
            var game = gamesPool[index];
            trainingGames[i].cGame = game;
            gamesPool.RemoveAt(index);
            trainingGames[i].inputSet = game.inputSets[0];
            trainingGames[i].time = TimeSpan.FromMinutes(4);
        }

        return trainingGames;
    }

    [Button]
    public void AddMove(string movementName)
    {
        Training.AddMove(movementName);
    }
    [Button]
    public void SetScore(int score)
    {
        Training.Score = score;
    }
    [Button]
    public void SaveTraining()
    {
        Training.duration = DateTime.Now - Training.Start;
        DatabaseManager.instance.AddTrainingsData(Training);
        Training = null;
        startedTraining = false;

    }
    [Button]
    public Dictionary<string, int> GetAllMovements()
    {
        Dictionary<string, int> movements = new Dictionary<string, int>();
        List<TrainingsData> trainingsData = GetTrainingsData();
        foreach (var item in trainingsData)
        {
            foreach (var movement in item.movements)
            {
                if (movements.ContainsKey(movement.Key))
                {
                    movements[movement.Key] += movement.Value;
                }
                else
                {
                    movements.Add(movement.Key, movement.Value);
                }
            }
        }
        return movements;
    }
    public Dictionary<string, int> GetMovements(string gameName)
    {
        Dictionary<string, int> movements = new Dictionary<string, int>();
        List<TrainingsData> trainingsData = GetTrainingsData();
        foreach (var item in trainingsData)
        {
            if (item.GameName != gameName)
            {
                continue;
            }
            foreach (var movement in item.movements)
            {
                if (movements.ContainsKey(movement.Key))
                {
                    movements[movement.Key] += movement.Value;
                }
                else
                {
                    movements.Add(movement.Key, movement.Value);
                }
            }
        }
        return movements;
    }

    public int TotalMovements(string v)
    {
        int sum = 0;
        foreach (var item in GetTrainingsData())
        {
            sum += item.GetMoves(v);
        }
        return sum;
    }

    public uint GetScoreSum()
    {
        return (uint)GetTrainingsData().Sum(x => x.Score);
    }

    public TimeSpan GetRemainingTrainingTime()
    {
        var cumulated = GetCumulatedTrainingTime();
        TimeSpan result = dailyTrainingTime - cumulated;
        if (result > TimeSpan.Zero)
        {
            return result;
        }
        else
        {
            return TimeSpan.Zero;
        }
    }

    public TimestampedPointCollection ToTimestampedPointCollection()
    {
        return new TimestampedPointCollection(GetTrainingsData().Select(x => new TimestampedPoint(x.Start, x.Score)).ToList());
    }
    public TimeSpan GetCumulatedTrainingTime()
    {
        return CumulatedTrainingSessionsLocal(timeToLookBack);
    }
    public TimeSpan CumulatedTrainingSessionsLocal(TimeSpan timeSpan)
    {
        TimeSpan sum = TimeSpan.Zero;

        foreach (var item in GetTrainingsData())
        {
            DateTime startPoint = DateTime.Now - timeSpan;
            if (DateTime.Today < startPoint)
            {
                startPoint = DateTime.Today;
            }
            //Training was entirely before defined span
            if (item.Start + item.duration < startPoint)
            {
                //ignore it
            }
            //Training was entirely inside defined span
            else if (item.Start > startPoint)
            {
                sum += item.duration;
            }
            //Training was partially inside Timespan
            else if (item.Start + item.duration > startPoint && item.Start < startPoint)
            {
                var splitSession = new TrainingsData()
                {
                    Start = startPoint,
                    duration = item.duration - (startPoint - item.Start)
                };
                sum += splitSession.duration;
            }
        }

        return sum;

    }

    public int GetGamesHighestScore(string gameName)
    {
        int maxScore = 0;
        foreach (var item in GetTrainingsData())
        {
            if (item.GameName == gameName)
            {

                maxScore = item.Score > maxScore ? item.Score : maxScore;
            }
        }
        return maxScore;
    }
    public int GetGameScoreSum(string gameName)
    {
        int sum = 0;
        foreach (var item in GetTrainingsData())
        {
            if (item.GameName == gameName)
            {
                sum += item.Score;
            }
        }
        return sum;
    }

    public int GetMaxDaysInARow()
    {
        var sessions = GetTrainingsData();
        if (sessions.Count == 0)
        {
            return 0;
        }
        var lastSession = sessions[0];
        int streak = 1;
        int maxStreak = 0;
        for (int i = 1; i < sessions.Count; i++)
        {
            if ((sessions[i].Start.Date > lastSession.Start.Date))
            {
                if ((sessions[i].Start.Date - lastSession.Start.Date) < TimeSpan.FromDays(2))
                {
                    streak++;
                    if (streak > maxStreak)
                    {
                        maxStreak = streak;
                    }

                }
                else
                {
                    streak = 1;
                }
            }


            lastSession = sessions[i];
        }
        if (maxStreak==0&&sessions.Count>0)
        {
            return 1;
        }
        return maxStreak;
    }

    public int GetHighestScore()
    {
        int maxScore = 0;
        foreach (var item in GetTrainingsData())
        {
            maxScore = item.Score > maxScore ? item.Score : maxScore;
        }
        return maxScore;
    }

    public List<string> GetPlayedGames()
    {
        List<string> gamesPlayed = new List<string>();
        foreach (var item in GetTrainingsData())
        {
            if (!gamesPlayed.Contains(item.GameName))
            {
                gamesPlayed.Add(item.GameName);
            }
        }
        return gamesPlayed;
    }

    private List<TrainingsData> GetTrainingsData()
    {
        return DatabaseManager.instance.GetTrainingsData();
    }

    public int MovementMaximum()
    {
        var movements = GetAllMovements();
        if (movements == null || movements.Count == 0)
        {
            return 0;
        }
        return movements.Max(x => x.Value);
    }

    public TimeSpan MaxTimeAfter8pm()
    {
        var trainingsSessions = GetTrainingsData();
        TrainingsData lastTrainingSession;
        TimeSpan maxTime = TimeSpan.Zero;
        if (trainingsSessions.Count == 0)
        {
            return TimeSpan.Zero;
        }
        lastTrainingSession = trainingsSessions[0];
        TimeSpan timeSum = TimeSpan.Zero;
        UpdateMaxTime(trainingsSessions[0]);
        for (int i = 0; i < trainingsSessions.Count; i++)
        {
            if (trainingsSessions[i].Start - lastTrainingSession.Start > TimeSpan.FromHours(12))
            {
                timeSum = TimeSpan.Zero;
            }

            UpdateMaxTime(trainingsSessions[i]);
            lastTrainingSession = trainingsSessions[i];
        }
        return maxTime;

        void UpdateMaxTime(TrainingsData trainingsSession)
        {
            if (trainingsSession.Start.Hour > 20 || trainingsSession.Start.Hour < 6)
            {
                timeSum += trainingsSession.duration;
            }

            if (timeSum > maxTime)
            {
                maxTime = timeSum;
            }
        }
    }

    public TimeSpan MaxTimeOnWeekends()
    {
        var trainingsSessions = GetTrainingsData();
        TrainingsData lastTrainingSession;
        TimeSpan maxTime = TimeSpan.Zero;
        if (trainingsSessions.Count == 0)
        {
            return TimeSpan.Zero;
        }
        lastTrainingSession = trainingsSessions[0];
        TimeSpan timeSum = TimeSpan.Zero;
        UpdateMaxTime(trainingsSessions[0]);
        for (int i = 0; i < trainingsSessions.Count; i++)
        {
            if (trainingsSessions[i].Start - lastTrainingSession.Start > TimeSpan.FromDays(5))
            {
                timeSum = TimeSpan.Zero;
            }

            UpdateMaxTime(trainingsSessions[i]);
            lastTrainingSession = trainingsSessions[i];
        }
        return maxTime;

        void UpdateMaxTime(TrainingsData trainingsSession)
        {
            if (trainingsSession.Start.DayOfWeek == DayOfWeek.Saturday || trainingsSession.Start.DayOfWeek == DayOfWeek.Sunday)
            {
                timeSum += trainingsSession.duration;
            }

            if (timeSum > maxTime)
            {
                maxTime = timeSum;
            }
        }
    }

    public uint GetMaxTimeGamePlayed()
    {
        var gamesPlayed = GetTimesGamesPlayed();
        if (gamesPlayed == null || gamesPlayed.Count == 0)
        {
            return 0;
        }
        return gamesPlayed.Max(x => x.Value);
    }
    public Dictionary<string, uint> GetTimesGamesPlayed()
    {
        Dictionary<string, uint> gamesPlayed = new Dictionary<string, uint>();
        var sessions = GetTrainingsData();
        foreach (var item in sessions)
        {

            if (!gamesPlayed.ContainsKey(item.GameName))
            {
                gamesPlayed.Add(item.GameName, 0);
            }
            if (item.duration.TotalMinutes >= 1)
            {
                gamesPlayed[item.GameName]++;
            }
        }
        return gamesPlayed;
    }
    public TimeSpan GetTrainingTime(string gameName)
    {
        TimeSpan timeSpan = new TimeSpan();
        var sessions = GetTrainingsData();
        foreach (var item in sessions)
        {
            if (item.GameName==gameName)
            {
                timeSpan += item.duration;
            }
        }
        return timeSpan;
    }
    public TimeSpan GetTotalTrainingTime()
    {
        TimeSpan timeSpan = new TimeSpan();
        var sessions = GetTrainingsData();
        foreach (var item in sessions)
        {
                timeSpan += item.duration;
        }
        return timeSpan;
    }
    public int GetTotalMovements()
    {
        int movements = 0;
        var sessions = GetTrainingsData();
        foreach (var item in sessions)
        {
            foreach (var movement in item.movements)
            {
                movements += movement.Value; 
            }
        }
        return movements;
    }
    private void OnDestroy()
    {
        if (Training != null)
        {
            SaveTraining();
        }
    }
}
