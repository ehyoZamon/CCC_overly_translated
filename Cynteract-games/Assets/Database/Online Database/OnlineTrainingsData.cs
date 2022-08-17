using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.OnlineDatabase
{
    public class OnlineTrainingsData
    {
        public long userID;
        public long timestamp;
        public long duration;
        public int score;
        public string game;
        public string movements;
        private TrainingsData x;

        public OnlineTrainingsData(TrainingsData x)
        {
            this.userID = x.UserID;
            this.timestamp = x.Start.Ticks;
            this.duration = x.DurationTicks;
            this.game = x.GameName;
            this.movements =  x.MovementsJson;
            this.score = x.Score;
        }
        [JsonConstructor]
        public OnlineTrainingsData(long userID, long timeStamp, long duration, int score, string game, string movements)
        {
            this.userID = userID;
            this.timestamp = timeStamp;
            this.duration = duration;
            this.score = score;
            this.game = game;
            this.movements = movements;
        }
        public TrainingsData ToTrainingsData()
        {
            var data = new TrainingsData();
            data.UserID = userID;
            data.Start = new System.DateTime(timestamp);
            data.Score = score;
            data.GameName = game;
            data.MovementsJson = movements.ToString();
            data.DurationTicks = duration;
            return data;
        }
    }
}