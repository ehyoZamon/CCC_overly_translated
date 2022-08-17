using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManagerTester : MonoBehaviour
{
    [ShowInInspector]
    public ScoreManager.ScoreSet Scoreset{
        get
        {
            return ScoreManager.Score;
        }
    }
    [ShowInInspector]
    public void Load()
    {
        ScoreManager.Load();
    }
    [ShowInInspector]
    public void Save()
    {
        ScoreManager.Save();
    }
    [ShowInInspector]
    public void Clear()
    {
        ScoreManager.Clear();
    }
    [ShowInInspector]
    public bool Add(ScoreManager.ScorePoint scorePoint)
    {
        return ScoreManager.Score.Add(scorePoint.Copy());
    }
    [ShowInInspector]
    public void Sort()
    {
        ScoreManager.Score.SortHighscoreList();
    }
    [ShowInInspector]
    public List<ScoreManager.ScorePoint> GetTodaysHighscore()
    {
        return ScoreManager.Score.GetHighscoreTodayList();
    }
}
