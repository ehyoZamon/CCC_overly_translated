using Cynteract.CCC;
using Cynteract.Database;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WelcomeScreenWindow : CCCWindow
{
    public Button continueButton;
    public Toggle dontShowAgainToggle;
    public TextMeshProUGUI welcomeText;
    protected override void OnInit()
    {
        continueButton.onClick.AddListener(Continue);
    }

    protected override void OnOpen()
    {
        
        dontShowAgainToggle.isOn = false;
        welcomeText.text = Lean.Localization.LeanLocalization.GetTranslationText("Welcome") + " " + DatabaseManager.instance.User.username +"!";
        
    }
    private void Continue()
    {
        DatabaseManager.instance.ShowWelcomeScreen(!dontShowAgainToggle.isOn);
        CynteractControlCenter.Home();
        Close();
    }
    protected override void OnClose(bool wasDestroyed)
    {

    }
}
