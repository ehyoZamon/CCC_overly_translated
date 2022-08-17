using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarDisplayer : MonoBehaviour
{
    public DiaryStar[] stars;
    public void Set(int index)
    {
        index--;
        for (int i = 0; i <= index; i++)
        {
            stars[i].SetActive(true);
        }
        for (int i = index + 1; i < stars.Length; i++)
        {
            stars[i].SetActive(false);
        }
    }
}
