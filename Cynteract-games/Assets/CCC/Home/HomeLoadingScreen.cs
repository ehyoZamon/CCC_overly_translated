using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HomeLoadingScreen : MonoBehaviour
{
    public TextMeshProUGUI dotsText;
    public TextMeshProUGUI loadingText;

    private void Start()
    {
        StartCoroutine( LoadingSymbols.Dots(dotsText, .5f, 10));
    }


}
