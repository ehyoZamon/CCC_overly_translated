using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Scorer : MonoBehaviour
{
    public TextMeshProUGUI text;
    public TextMeshProUGUI movingNumberPrefab;
    public List<TextMeshProUGUI> flyingNumbers;
    public float speed  = 100;
    public string preText;
    int lastTotal=0;
    public void Score(int score, int total, Transform item)
    {
        CreateFlyingNumber(item, score);
    }
    public void Score(int score, int total)
    {
        text.text =(lastTotal).ToString();
        lastTotal = total;
    }
    public void Score(int score)
    {
        lastTotal += score;
        text.text = lastTotal.ToString();
    }
    private void Update()
    {
        if (flyingNumbers!=null)
        {
            for (int i = 0; i < flyingNumbers.Count; i++)
            {
                var item=flyingNumbers[i];
                item.transform.localPosition -= item.transform.localPosition.normalized * Time.deltaTime * speed;
                if (item.transform.localPosition.x <= 0 || item.transform.localPosition.y >= 0)
                {
                    Score(int.Parse( item.text));
                    Destroy(item.gameObject);
                    flyingNumbers.RemoveAt(i);

                }
            }
        }
    }
    private void CreateFlyingNumber(Transform item, int score)
    {
        var movingNumber = Instantiate(movingNumberPrefab, Camera.main.WorldToScreenPoint(item.position), Quaternion.identity, transform);
        movingNumber.text = score.ToString();
        flyingNumbers.Add(movingNumber);
    }
}
