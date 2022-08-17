using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartContainer : MonoBehaviour
{
    public Heart heartPrefab;

    Stack<Heart> hearts;
    public void Init(int number)
    {
        hearts = new Stack<Heart>();
        for (int i = 0; i < number; i++)
        {
            hearts.Push(Instantiate(heartPrefab, transform));
        }
    }
    public void UpdateHearts(int number)
    {
        if (number<hearts.Count)
        {
            int heartsToRemove =  hearts.Count-number;
            for (int i = 0; i < heartsToRemove; i++)
            {
                var heart = hearts.Pop();
                heart.Destroy();
            }
        }
        else if( number> hearts.Count)
        {
            for (int i = hearts.Count; i < number; i++)
            {
                hearts.Push(Instantiate(heartPrefab, transform));
            }
        }
    }
}
