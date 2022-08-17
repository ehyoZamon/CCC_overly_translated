using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScreenTester : MonoBehaviour
{
    public GameOverScreen gameOverScreen;
    // Start is called before the first frame update
    void Start()
    {
        var popup=Instantiate(gameOverScreen, PopupWindowCanvas.instance.transform);
        popup.Init();
        popup.SetCurrentScore(30);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
