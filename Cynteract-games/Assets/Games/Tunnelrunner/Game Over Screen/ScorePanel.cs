using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScorePanel : MonoBehaviour
{
    public TextMeshProUGUI rankText, nameText, scoreText;
    public Image backgroundImage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(RankedScorePoint item)
    {
        rankText.text = (item.rank+1).ToString();
        nameText.text = item.scorePoint.name.ToString();
        scoreText.text = item.scorePoint.score.ToString();
        if (item.isCurrent)
        {
            backgroundImage.color = new Color(0.43f,1, 0.55f);
        }
        else
        {
            backgroundImage.color = Color.white;
        }
    }
}
