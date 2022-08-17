using Cynteract;
using Cynteract.CCC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UKAPWindow : CCCWindow
{
    public Button startGameButton, saveDataButton;
    public CGame game;

    protected override void OnClose(bool wasDestroyed)
    {
    }

    protected override void OnInit()
    {
        startGameButton.onClick.AddListener(StartGame);
        saveDataButton.onClick.AddListener(SaveData);
    }

    private  void SaveData()
    {
        CynteractControlCenter.Saving();
        //await RotationSaver.instance.SaveDataAsync();
        CynteractControlCenter.StopSaving();
    }

    private void StartGame()
    {
        TrainingGame[] trainingGames = new TrainingGame[game.inputSets.Length];
        for (int i = 0; i < trainingGames.Length; i++)
        {
            trainingGames[i] = new TrainingGame();
            trainingGames[i].cGame = game;
            trainingGames[i].inputSet = game.inputSets[i];
        }
        GameTrainingController.instance.StartGames(trainingGames, timed:false, tutorial: false); ;
    }

    protected override void OnOpen()
    {
    }


}
