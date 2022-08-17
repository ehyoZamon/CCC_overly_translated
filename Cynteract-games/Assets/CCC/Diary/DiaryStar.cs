using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiaryStar : MonoBehaviour
{
    public Color activeColor, inactiveColor;
    public Image image;
    [Button]
    public void SetActive(bool v)
    {
        if (v)
        {
            image.color = activeColor;
        }
        else
        {
            image.color = inactiveColor;
        }
    }

}
