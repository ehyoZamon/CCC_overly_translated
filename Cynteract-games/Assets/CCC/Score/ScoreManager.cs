using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreManager
{
    public static readonly string fileName="scores";
    private static ScoreSet score;
    public static ScoreSet Score
    {
        get
        {
            if (score==null)
            {
                score = new ScoreSet();
            }
            return score;
        }
    }
    public static ScoreSet Load()
    {
        ScoreSet score;
        try
        {
            score = JsonFileManager.Load<ScoreSet>(fileName, JsonFileManager.Type.Newtonsoft);
        }
        catch 
        {
            score = new ScoreSet();
        }
        ScoreManager.score = score;
        return score;
    }
    public static void Clear()
    {
        score = new ScoreSet();
    }
    public static void Save()
    {
        JsonFileManager.Save(score, fileName,JsonFileManager.Type.Newtonsoft);
    }
    [Serializable]
    public class ScoreSet
    {
        public List<ScorePoint> scores;

        public ScoreSet()
        {
            scores = new List<ScorePoint>();
        }

        public List<ScorePoint> SortHighscoreList()
        {
            scores.Sort(ScorePoint.CombinedHighScoreComparer);
            return scores;
        }
        public List<ScorePoint> GetHighscoreTotalList()
        {
            var newScores = scores.Select(x=>x).ToList();
            scores.Sort(ScorePoint.CombinedHighScoreComparer);
            return scores;
        }
        public List<ScorePoint> GetHighscoreTodayList()
        {
            var newScores = scores.Where(x => x.time >= DateTime.Today).ToList();
            newScores.Sort(ScorePoint.CombinedHighScoreComparer);
            return newScores;
        }
        public bool ContainsName(string name)
        {
            return scores.Find(x => x.name == name)!=null;
        }
        public bool ContainsName(ScorePoint scorePoint)
        {
            return ContainsName(scorePoint);
        }
        public ScorePoint Find(Predicate <ScorePoint> match)
        {
            return scores.Find(match);
        }
        public ScorePoint Find(string name)
        {
            return scores.Find(x => x.name == name);
        }
        public List<ScorePoint> FindAll(string name)
        {
            return scores.FindAll(x => x.name == name);
        }
        public bool Add(ScorePoint scorePoint)
        {
            var elements = FindAll(scorePoint.name);
                bool add = true;
            if (elements.Count>0)
            {
                foreach (var item in elements)
                {
                    if (item.time.Date == scorePoint.time.Date)
                    {
                        if (item.score < scorePoint.score)
                        {
                            Remove(item);
                        }
                        else
                        {
                            add = false;
                        }
                    }
                }
            }

                if (add)
                {
                    scores.Add(scorePoint);
                    return true;
                }
            return false;
        }

        public bool Remove(ScorePoint element)
        {
            return scores.Remove(element);
        }
        public int RemoveAll(string name)
        {
            return scores.RemoveAll(x=>x.name==name);
        }
    }
    [Serializable]
    public class ScorePoint
    {
        public string name;
        public uint score;
        public DateTime time;

        public ScorePoint(string name, uint score, DateTime time)
        {
            this.name = name;
            this.score = score;
            this.time = time;
        }

        public ScorePoint Copy()
        {
            return new ScorePoint(name, score, time);
        }
        #region Comparers
        public static int ScoreComparerLowestScoreFirst(ScorePoint x,ScorePoint y )
        {
            if (x.score<y.score)
            {
                return -1;
            }
            else if(x.score==y.score)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        public static int ScoreComparerHighestScoreFirst(ScorePoint x, ScorePoint y)
        {
            if (x.score < y.score)
            {
                return 1;
            }
            else if (x.score == y.score)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
        public static int TimeComparerRecentFirst(ScorePoint x, ScorePoint y)
        {
            if (x.time < y.time)
            {
                return 1;
            }
            else if (x.time == y.time)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
        public static int TimeComparerOldestFirst(ScorePoint x, ScorePoint y)
        {
            if (x.time < y.time)
            {
                return -1;
            }
            else if (x.time == y.time)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        public static int CombinedHighScoreComparer(ScorePoint x, ScorePoint y)
        {
            if (x.score < y.score)
            {
                return 1;
            }
            else if (x.score == y.score)
            {
                return TimeComparerOldestFirst(x, y);
            }
            else
            {
                return -1;
            }
        }
        #endregion
    }
}
