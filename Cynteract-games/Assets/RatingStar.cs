using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RatingStar : MonoBehaviour
{
    public Button button;
    public Color activeColor, inactiveColor;
    internal int index;

    public void SetActive(bool active)
    {
        button.image.color = active ? activeColor : inactiveColor;
    }

    public void Activate(StarsSelector starsSelector)
    {
        starsSelector.SetActive(index);
    }

    public void Init(StarsSelector starsSelector)
    {
        button.onClick.AddListener(() => Activate(starsSelector));
    }
}
