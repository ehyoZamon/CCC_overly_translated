using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LoadingSymbols 
{
    public  static IEnumerator Dots(TMP_Text textMesh, float interval, int number)
    {
        string dot = ".";
        int numberOfDots = 0;
        while (true)
        {
            string text="";
            for (int i = 0; i < numberOfDots; i++)
            {
                text += dot;
            }
            numberOfDots = (numberOfDots + 1) % number + 1;
            textMesh.text = text;
            yield return new WaitForSecondsRealtime(interval);
        }
    }
}
