using Cynteract;
using Cynteract.CCC;
using Cynteract.CGlove;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TunnelrunnerArcadeController : MonoBehaviour
{
    public CGame game;

    
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        StartCoroutine(InitGameRoutine(new TrainingGame()
        {
            cGame = game,
            inputSet=game.inputSets[0],
        })) ;
    }
    public async Task WaitForGlove()
    {
        var connected=await GloveConnectionCheck.instance.StartContinouslyCheckingAsync();
        print($"Connected {connected}");
    }
    IEnumerator InitGameRoutine(TrainingGame trainingGame)
    {
        CheckAndStart(trainingGame);
        yield return new WaitUntil(() => GameController.gameControllerInstance != null);
        GameController.gameControllerInstance.SetUpInput(trainingGame.inputSet);
        GameController.gameControllerInstance.StartGame();
        GameController.gameControllerInstance.shouldStartTutorial = false;
    }

    private async void CheckAndStart(TrainingGame trainingGame)
    {
        await WaitForGlove();
        LoadGame(trainingGame);
    }

    private void LoadGame(TrainingGame trainingGame)
    {
        string sceneName = trainingGame.cGame.sceneToLoad;
        SceneManager.LoadScene(sceneName);
    }
}
