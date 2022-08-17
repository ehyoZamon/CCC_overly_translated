using Cynteract.CCC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameOverScreen : PopupWindow
{
    public TextMeshProUGUI pointsText, highscoreTitle, dailyHighscoreTitle, rocketCrashedTitle;
    Action<bool> callbackAction;
    public Button saveButton, skipButton;
    public Scoreboard totalScoreboard, todayScoreBoard;
    public TMP_InputField inputField;
    private uint score;

    public void SubscribeOnClosed(Action<bool> callback)
    {
        this.callbackAction = callback;
    }

    protected override void OnClose()
    {

    }

    protected override void OnInit()
    {

        saveButton.onClick.AddListener(Save);
        skipButton.onClick.AddListener(Skip);
    }

    private void Save()
    {
        if (inputField.text != "" && inputField.text != "Du")
        {
            ScoreManager.Load();
            ScoreManager.Score.Add(new ScoreManager.ScorePoint(inputField.text, score, DateTime.Now));
            ScoreManager.Save();
        }
        Close(true);
    }
    private void Skip()
    {
        Close(false);
    }

    private void Close(bool v)
    {
        if (callbackAction != null)
        {
            callbackAction(v);

        }
        Close();
    }

    private void Update()
    {
        if (inputField.text == "" || inputField.text == "Du")
        {
            saveButton.interactable = false;
            skipButton.interactable = true;
        }
        else
        {
            saveButton.interactable = true;
            skipButton.interactable = false;

        }
    }
    public void SetCurrentScore(uint score)
    {
        this.score = score;
        pointsText.text = score.ToString();
        ScoreManager.Load();

        ScoreManager.ScorePoint newPoint = new ScoreManager.ScorePoint("Du", score, DateTime.Now);
        ScoreManager.Score.Add(newPoint);

        var totalHighScores = ScoreManager.Score.GetHighscoreTotalList();
        var totalHighscorePoints = CreatePanels(newPoint, totalHighScores);
        totalScoreboard.CreatePanels(totalHighscorePoints);



        var dailyHighscores = ScoreManager.Score.GetHighscoreTodayList();
        var dailyHighscorePoints = CreatePanels(newPoint, dailyHighscores);
        todayScoreBoard.CreatePanels(dailyHighscorePoints);
        if (totalHighScores[0].name == "Du")
        {
            HighscoreTitle();
        }
        else if (dailyHighscores[0].name == "Du")
        {
            DailyHighTitle();
        }
        else
        {
            RocketCrashTitle();
        }
        ScoreManager.Score.Remove(newPoint);

    }

    private void RocketCrashTitle()
    {
        highscoreTitle.gameObject.SetActive(false);
        dailyHighscoreTitle.gameObject.SetActive(false);
        rocketCrashedTitle.gameObject.SetActive(true);
    }

    private void DailyHighTitle()
    {
        highscoreTitle.gameObject.SetActive(false);
        dailyHighscoreTitle.gameObject.SetActive(true);
        rocketCrashedTitle.gameObject.SetActive(false);
    }

    private void HighscoreTitle()
    {
        highscoreTitle.gameObject.SetActive(true);
        dailyHighscoreTitle.gameObject.SetActive(false);
        rocketCrashedTitle.gameObject.SetActive(false);
    }

    private List<RankedScorePoint> CreatePanels(ScoreManager.ScorePoint newPoint, List<ScoreManager.ScorePoint> highscores)
    {
        List<RankedScorePoint> highscorePoints = new List<RankedScorePoint>();
        bool addedNew = false;
        for (int i = 0; i < 3 && i < highscores.Count; i++)
        {
            RankedScorePoint item = new RankedScorePoint(i, highscores[i]);
            if (highscores[i].name == "Du")
            {
                addedNew = true;
                item.isCurrent = true;
            }
            highscorePoints.Add(item);
        }
        if (!addedNew)
        {
            RankedScorePoint item = new RankedScorePoint(highscores.IndexOf(newPoint), newPoint);
            item.isCurrent = true;
            highscorePoints.Add(item);
        }
        else if (highscores.Count > 3)
        {
            highscorePoints.Add(new RankedScorePoint(3, highscores[3]));
        }
        if (highscores.Count > 4)
        {
            highscorePoints.Add(new RankedScorePoint(highscores.Count - 1, highscores[highscores.Count - 1]));
        }
        return highscorePoints;
    }
}
