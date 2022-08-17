using Cynteract.CGlove;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RawVibrationTester : MonoBehaviour
{
    public Button[] buttons;

    private void Awake()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(()=>Vibrate(index));
        }
    }
    public void Vibrate(int index)
    {
        StartCoroutine(VibrationRoutine(index));
    }

    public void  SetButtonsActive(bool active)
    {
        foreach (var item in buttons)
        {
            item.interactable=active;
        }
    }
    IEnumerator VibrationRoutine(int index)
    {
        var text = buttons[index].gameObject.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        text.text = "Vibrating";
        SetButtonsActive(false);
        Glove.Any.VibrateRawIndex(index, 1);
        yield return new WaitForSeconds(1);
        Glove.Any.VibrateStop();
        text.text = (index+1).ToString();

        SetButtonsActive(true);
    }
}
