using Cynteract.CCC;
using Cynteract.CTime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCanvas : MonoBehaviour {
    public bool inMenu;
    public static MainCanvas instance;
    public GameObject mainMenu, closedMenu;
    public Button menuButton, backButton;
    private void Awake()
    {
        menuButton.onClick.AddListener(OnClickedMenu);
        backButton.onClick.AddListener(OnClickedBack);
    }
    public MainCanvas()
    {
        instance = this;
    }
    public void OnClickedMenu()
    {
        TimeControl.instance.MenuPause();
        mainMenu.SetActive(true);
        closedMenu.SetActive(false);
        inMenu = true;
        AudioManager.instance.LowPassMax();
    }
    public void OnClickedBack()
    {
        TimeControl.instance.MenuUnpause();
        mainMenu.SetActive(false);
        closedMenu.SetActive(true);
        inMenu = false;
        AudioManager.instance.LowPassOff();

    }
}
