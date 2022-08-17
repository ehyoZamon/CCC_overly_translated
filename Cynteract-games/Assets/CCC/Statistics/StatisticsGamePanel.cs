using Cynteract;
using Cynteract.CCC;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Lean.Localization;
using Cynteract.Database;
using System;

public class StatisticsGamePanel : MonoBehaviour
{
    public Image image;
    public CGame cGame;
    public TextMeshProUGUI titleTextMesh, highscoreTextMesh, totalScoreTextMesh, globalScoreTextMesh, rankTextMesh, movementsTextMesh, timeTextMesh;
    public Image backgroundImage;
    public Color gold, silver, bronze;
    public int HighScore { get; private set; }
    public int TotalScore { get; private set; }
    public int GlobalScore { get; private set; }
    public int Rank { get; private set; }

    public int MovementsSum { get; private set; }

    public TimeSpan Time { get; private set; }

    public async void Init(CGame cGame)
    {
        image.sprite = cGame.sprite;
        this.cGame = cGame;
        titleTextMesh.text = LeanLocalization.GetTranslationText(cGame.name);
        HighScore = TrainingsManager.instance.GetGamesHighestScore(cGame.name);
        highscoreTextMesh.text = $"{HighScore}";
        TotalScore = TrainingsManager.instance.GetGameScoreSum(cGame.name);
        totalScoreTextMesh.text = $"{TotalScore}";
        Time = TrainingsManager.instance.GetTrainingTime(cGame.name);
        timeTextMesh.text = $"{Time.Hours}h {Time.Minutes}m {Time.Seconds}s";
        string movementsString = "";
        var movements=TrainingsManager.instance.GetMovements(cGame.name);
        MovementsSum = 0;
        foreach (var item in movements)
        {
            MovementsSum += item.Value;
            movementsString += $"{LeanLocalization.GetTranslationText(item.Key)}: {item.Value} \n";
        }
        movementsString.TrimEnd('\n');
        movementsTextMesh.text = movementsString;
        var result = await DatabaseManager.instance.GetMaxHighscore(cGame.name);
        switch (result)
        {
            case NotConnectedResponse _:
                GlobalScore = int.MinValue;
                return;
            case GenericResponse<int> response:
                GlobalScore = response.content;
                globalScoreTextMesh.text = $"{GlobalScore}";
                break;
        }
        result = await DatabaseManager.instance.GetRank(cGame.name);
        switch (result)
        {
            case NotConnectedResponse _:
                return;
            case GenericResponse<int> response:
                if (response.content == -1)
                {
                    Rank = int.MaxValue;
                    rankTextMesh.text = $"-";
                }
                else
                {
                    Rank = response.content + 1;
                    switch (Rank)
                    {
                        case 1:
                            backgroundImage.color = gold;
                            break;
                        case 2:
                            backgroundImage.color = silver;
                            break;
                        case 3:
                            backgroundImage.color = bronze;
                            break;
                        default:
                            backgroundImage.color = Color.white;
                            break;
                    }
                    rankTextMesh.text = $"<b>#{Rank}</b>";
                }
                break;

        }
    }

}
