using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivesDisplayer : MonoBehaviour
{
    public static LivesDisplayer instance;
    public HeartContainer heartContainer;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    public void Init(int lives)
    {
        heartContainer.Init(lives);
    }

    // Update is called once per frame
    public void UpdateLives(int lives)
    {
        heartContainer.UpdateHearts(lives);
    }
}
