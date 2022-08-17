using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractRatingItem 
{
    public int index;
    public abstract void SetActive(bool active);
    public abstract void Activate(StarsSelector starsSelector);
    public abstract void Init(StarsSelector starsSelector);
}

