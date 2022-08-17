using Cynteract.CCC;
using Cynteract.CGlove;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaitForConnectionPanel : MonoBehaviour
{
    public TextMeshProUGUI textMesh;
    public Button backButton;
    public void Awake()
    {
        backButton.onClick.AddListener(ReturnToMenu);
    }
    private void ReturnToMenu()
    {
        GloveConnectionCheck.instance.StopChecking(); 
        GameTrainingController.instance.StopGames();
    }

}
