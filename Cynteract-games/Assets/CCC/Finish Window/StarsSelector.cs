using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarsSelector : MonoBehaviour
{
    public Action<int> onSelection;
    public RatingStar ratingStarPrefab;
    public int stars = 5;
    [ReadOnly]
    public RatingStar[] ratingStars;
    // Start is called before the first frame update
    public void Init()
    {
        ratingStars = new RatingStar[stars];
        for (int i = 0; i < stars; i++)
        {
            ratingStars[i] = Instantiate(ratingStarPrefab, transform);
            ratingStars[i].index = i;
            ratingStars[i].Init(this);
            ratingStars[i].SetActive(false);
        }
       // SetActive(0);
    }
    public void DeactivateAll()
    {
        for (int i = 0; i < stars; i++)
        {
            ratingStars[i].SetActive(false);
        }
    }

    public void SetActive(int index)
    {
        for (int i = 0; i <= index; i++)
        {
            ratingStars[i].SetActive(true);
        }
        for (int i = index+1; i < stars; i++)
        {
            ratingStars[i].SetActive(false);
        }
        onSelection(index);
    }
}
