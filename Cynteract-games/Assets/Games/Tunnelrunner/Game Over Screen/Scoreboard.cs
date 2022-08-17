using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RankedScorePoint
{
    public int rank;
    public ScoreManager.ScorePoint scorePoint;
    public bool isCurrent;

    public RankedScorePoint(int rank, ScoreManager.ScorePoint scorePoint)
    {
        this.rank = rank;
        this.scorePoint = scorePoint;
    }
}
public class Scoreboard : MonoBehaviour
{
    public ScorePanel scorePanelPrefab;
    public Transform dotsPrefab;
    public List<ScorePanel> scorePanels;
    public List<Transform> dots;

    public Transform scorePanelParent;
    public void DeletePanels()
    {
        if (scorePanels!=null)
        {
            foreach (var item in scorePanels)
            {
                Destroy(item.gameObject);
            }
        }
        scorePanels = new List<ScorePanel>();

        if (dots != null)
        {
            foreach (var item in dots)
            {
                Destroy(item.gameObject);
            }
        }
        dots = new List<Transform>();
    }
    
    public void CreatePanels(List<RankedScorePoint> scorePoints)
    {
        DeletePanels();
        int lastRank = 0;
        for (int i = 0; i < scorePoints.Count; i++)
        {
            var rank = scorePoints[i].rank;
            if (rank-lastRank>1)
            {
                var dot = Instantiate(dotsPrefab, scorePanelParent);
                dots.Add(dot);
            }
            var scorePanel = Instantiate(scorePanelPrefab, scorePanelParent);
            scorePanel.Init(scorePoints[i]);
            scorePanels.Add(scorePanel);
            lastRank = rank;
        }
    }
}
