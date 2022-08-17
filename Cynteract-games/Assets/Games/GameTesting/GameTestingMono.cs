#if UNITY_EDITOR
using Cynteract;
using Cynteract.CGlove;
using Cynteract.CTime;
using Cynteract.CynteractInput;
using Cynteract.GameTesting;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Presets;
using UnityEngine;

public class GameTestingMono : MonoBehaviour
{
    public GloveManager gloveManager;
    private void Awake()
    {
        var presets = Preset.GetDefaultPresetsForObject(this);
        presets[0].ApplyTo(this);
        DontDestroyOnLoad(gameObject);
        Instantiate(gloveManager);
        var timeControlGO = new GameObject();
        var timeControl= timeControlGO.AddComponent<TimeControl>();
        timeControl.baseTimeGetter = new ConstantBaseTime();
        timeControlGO.name = "Time Control";
    }
    // Start is called before the first frame update
    void Start()
    {
        GameTestingWindow instance = GameTestingWindow.instance;
        GameController.gameControllerInstance.SetUpInput(instance.axes, GameTestingWindow.instance.actions);
        if (GameTestingWindow.instance.useGlove)
        {
            GameController.gameControllerInstance.StartGame();
        }
        else
        {
            GameController.gameControllerInstance.StartGameWithoutGlove();
            var gO = new GameObject();
            gO.AddComponent<InputUpdater>();
            gO.name = "Input Updater";
        }



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
#endif