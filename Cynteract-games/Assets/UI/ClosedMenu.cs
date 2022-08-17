using Cynteract.CTime;
using Cynteract.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClosedMenu : MonoBehaviour
{
    public Button recalibrateButton;
    public UnityEvent recalibrateEvent;
    public bool menu;
    private void Awake()
    {
        recalibrateButton.onClick.AddListener(Recalibrate);
    }
    public void Recalibrate()
    {
        EventSystem.current.SetSelectedGameObject(null);
        recalibrateEvent.Invoke();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (menu)
            {
                MainCanvas.instance.OnClickedMenu();
            }
        }
    }
}
