using Sirenix.OdinInspector;
using SqliteForUnity3D;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;



[Serializable]
public class TrainingsData
{
    [ShowInInspector, ReadOnly]
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [ShowInInspector]
    public long UserID { get; set; }
    [ShowInInspector]
    public DateTime Start { get; set; }
    [ShowInInspector]
    public string GameName { get; set; }
    [ShowInInspector]
    public TimeSpan duration;
    [ShowInInspector]
    public int Score { get; set; }
    public long DurationTicks
    {
        get
        {
            return duration.Ticks;
        }
        set
        {
            duration = new TimeSpan(value);
        }
    }
    [ShowInInspector]
    public Dictionary<string, int> movements = new Dictionary<string, int>();
    [ReadOnly, MultiLineProperty, ShowInInspector]
    public string MovementsJson
    {
        get
        {
            return JObject.FromObject(movements).ToString();
        }
        set
        {
            movements = JObject.Parse(value).ToObject<Dictionary<string, int>>();
        }
    }
    public void AddMove(string name)
    {
        if (movements.ContainsKey(name))
        {
            movements[name]++;
        }
        else
        {
            movements.Add(name, 1);
        }
    }
    public int GetMoves(string name)
    {
        if (movements.ContainsKey(name))
        {
            return movements[name];
        }
        return 0;
    }

    public  string ToFlatString()
    {
        return $"{{{UserID},{Start},{duration},{GameName},{Score}}}";
    }
}
