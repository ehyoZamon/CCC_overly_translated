using Cynteract;
using Cynteract.CCC;
using Cynteract.CGlove;
using Cynteract.CTime;
using Cynteract.CynteractInput;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameTrainingController : MonoBehaviour
{
    public Transform timer;
    public TextMeshProUGUI timerTextMesh;

    public static GameTrainingController instance;
    private Coroutine updateInputsCoroutine;
    private Coroutine trainingTimeRoutine;

    public bool LoadedAnyGame { get; private set; }
    public bool IsRunning { get; private set; }

    private void Awake()
    {
        instance = this;
    }



    public async void StartGame(TrainingGame trainingGame)
    {
        trainingGame.shouldStartTutorial=await CynteractControlCenter.TutorialPopup();
        await PrepareForGame(false);
        LoadGameScene(trainingGame);
        StartCoroutine(SoloGameRoutine(trainingGame));
    }
    public async void StartGames(TrainingGame[] trainingGames, bool timed=true, bool tutorial=true)
    {
        bool shouldStartTutorial=false;
        if (tutorial)
        {
            shouldStartTutorial = await CynteractControlCenter.TutorialPopup();
        }
        foreach (var item in trainingGames)
        {
            item.shouldStartTutorial = shouldStartTutorial;
        }
        await PrepareForGame(true);
        StartCoroutine(TrainingRoutine(trainingGames, timed));
    }
    public void StopGames()
    {
        StopAllCoroutines();
        if (TrainingsManager.instance.startedTraining)
        {
            SaveTraining();
        }
        StopGamesPrivate();
    }
    private void LoadGameScene(TrainingGame trainingGame)
    {
        string sceneName = trainingGame.cGame.sceneToLoad;
        SceneManager.LoadScene(sceneName);
    }
    private async void ShowTrainingWarning()
    {
        TimeControl.instance.MenuPause();
        var continueTraining = await CynteractControlCenter.PlaytimeWarningPopup();
        TimeControl.instance.MenuUnpause();
        if (continueTraining)
        {
            return;
        }
        else
        {
            StopGames();
        }
    }
    private async Task PrepareForGame(bool timerActive)
    {
        CynteractControlCenter.ActiveWindow.Close();
        timer.gameObject.SetActive(timerActive);
        //RotationDataManager.instance.StartSaving();
        //ForceDataManager.instance.StartSaving();
        CCCStatusBar.instance.Hide();
        AudioManager.instance.LowPassOff();
        LoadedAnyGame = false;
        IsRunning = true;
        await GloveConnectionCheck.instance.StartContinouslyCheckingAsync();
        var remainingTrainingTime =TrainingsManager.instance.GetRemainingTrainingTime();
        if (remainingTrainingTime > TimeSpan.Zero)
        {
            trainingTimeRoutine=StartCoroutine(CheckTrainingTimeRoutine(remainingTrainingTime));
        }
    }
    private void InitInputs(TrainingGame trainingGame)
    {
        if (updateInputsCoroutine != null)
        {
            StopCoroutine(updateInputsCoroutine);
        }
        GameController.gameControllerInstance.SetHighScore(TrainingsManager.instance.GetGamesHighestScore(trainingGame.cGame.name));
        GameController.gameControllerInstance.SetUpInput(trainingGame.inputSet);
        GameController.gameControllerInstance.StartGame();
        GameController.gameControllerInstance.shouldStartTutorial = trainingGame.shouldStartTutorial;
        updateInputsCoroutine = StartCoroutine(UpdateInputsRoutine());
    }
    private void DisplayTime(TimeSpan ts)
    {
        string elapsedTime = CCCUtility.FormatTime(ts);
        timerTextMesh.text = elapsedTime;
    }
    private async void StopGamesPrivate()
    {
        if (updateInputsCoroutine != null)
        {
            StopCoroutine(updateInputsCoroutine);
        }
        if (trainingTimeRoutine!=null)
        {
            StopCoroutine(trainingTimeRoutine);

        }
        IsRunning = false;
        GloveConnectionCheck.instance.StopContinouslyChecking();
        SceneManager.LoadScene("EmptyCCC");
        timer.gameObject.SetActive(false);
        CynteractControlCenter.Saving();
        //await RotationDataManager.instance.StopSaving();
        //await ForceDataManager.instance.StopSaving();
        await Cynteract.Database.DatabaseManager.instance.SyncTrainingsData();
        CynteractControlCenter.StopSaving();
        CCCStatusBar.instance.Show();
        if (LoadedAnyGame)
        {
            CynteractControlCenter.FinishWindow();
        }
        else
        {
            CynteractControlCenter.Home();
        }
        LoadedAnyGame = false;
        TimeControl.instance.ResetAll();
    }
    private IEnumerator TrainingRoutine(TrainingGame[] trainingGames, bool timed=true)
    {
        for (int i = 0; i < trainingGames.Length; i++)
        {
            TrainingsManager.instance.StartNewTraining(trainingGames[i].cGame.name);
            LoadGameScene(trainingGames[i]);
            if (timed)
            {
                yield return StartCoroutine(TrainingGameRoutine(trainingGames[i]));
            }
            else
            {
                yield return StartCoroutine(TrainingGameRoutineNoTimeLimit(trainingGames[i]));
            }
            SaveTraining();
        }

        StopGamesPrivate();
    }

    private  void SaveTraining()
    {

        TrainingsManager.instance.SetScore(GameController.gameControllerInstance.GetScore());
        TrainingsManager.instance.SaveTraining();
    }
    private IEnumerator SoloGameRoutine(TrainingGame trainingGame)
    {
        yield return StartCoroutine(InitGameRoutine(trainingGame));
        InitInputs(trainingGame);
        TrainingsManager.instance.StartNewTraining(trainingGame.cGame.name);
    }
    private IEnumerator TrainingGameRoutine(TrainingGame trainingGame)
    {
        yield return StartCoroutine(InitGameRoutine(trainingGame));
        InitInputs(trainingGame);
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        while (stopWatch.Elapsed < trainingGame.time)
        {
            TimeSpan ts = trainingGame.time - stopWatch.Elapsed;
            DisplayTime(ts);
            if (TimeControl.instance.CurrentTimeScale == 0)
            {
                stopWatch.Stop();
            }
            else
            {
                if (!stopWatch.IsRunning)
                {
                    stopWatch.Start();
                }
            }
            yield return null;

        }
        stopWatch.Stop();
    }
    private IEnumerator TrainingGameRoutineNoTimeLimit(TrainingGame trainingGame)
    {
        yield return StartCoroutine(InitGameRoutine(trainingGame));
        InitInputs(trainingGame);
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        while (!GameController.gameControllerInstance.switchGame)
        {
            TimeSpan ts =  stopWatch.Elapsed;
            DisplayTime(ts);
            if (TimeControl.instance.CurrentTimeScale == 0)
            {
                stopWatch.Stop();
            }
            else
            {
                if (!stopWatch.IsRunning)
                {
                    stopWatch.Start();
                }
            }
            yield return null;

        }
        stopWatch.Stop();
    }
    private IEnumerator InitGameRoutine(TrainingGame trainingGame)
    {
        yield return new WaitUntil(() => GameController.gameControllerInstance != null);
        LoadedAnyGame = true;
    }
    private IEnumerator UpdateInputsRoutine()
    {
        CInput.Axis[] axes = GameController.gameControllerInstance.cInput.axes;
        CInput.Action[] actions = GameController.gameControllerInstance.cInput.actions;
        bool[] lastAxisHigh = new bool[axes.Length];
        bool[] lastActions = new bool[actions.Length];
        while (true)
        {
            if (!TrainingsManager.instance.startedTraining)
            {
                yield return null;
            }
            for (int i = 0; i < axes.Length; i++)
            {
                float axis = CInput.GetAxis(axes[i]);
                bool axisHigh = axis > 0.8f;
                bool axisLow = axis < 0.2f;

                if (lastAxisHigh[i] && axisLow || !lastAxisHigh[i] && axisHigh)
                {
                    TrainingsManager.instance.AddMove(GloveInput.GetInputAxis(axes[i].index, axes[i].axis).name);
                }

                if (axisLow)
                {
                    lastAxisHigh[i] = false;
                }
                else if (axisHigh)
                {
                    lastAxisHigh[i] = true;
                }
            }

            for (int i = 0; i < actions.Length; i++)
            {
                bool action = CInput.GetAction(actions[i]);
                if (action != lastActions[i])
                {
                    TrainingsManager.instance.AddMove(GloveInput.GetInputAction(actions[i].index, actions[i].action).name);
                }
                lastActions[i] = action;
            }
            yield return null;
        }


    }
    private IEnumerator CheckTrainingTimeRoutine(TimeSpan remainingTrainingTime)
    {
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        while (stopWatch.Elapsed < remainingTrainingTime)
        {
            if (TimeControl.instance.CurrentTimeScale == 0)
            {
                stopWatch.Stop();
            }
            else
            {
                if (!stopWatch.IsRunning)
                {
                    stopWatch.Start();
                }
            }
            yield return null;
        }
        ShowTrainingWarning();
    }
}
