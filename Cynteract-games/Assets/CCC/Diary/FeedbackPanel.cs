using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public class FeedbackPanel : MonoBehaviour
{
    public TextMeshProUGUI dateText;
    public StarDisplayer stars, pain;
    public TextField commentsText;
    public GameObject starsNotSpecifiedText, painNotSpecifiedText;
    [Button]
    public void Init(Cynteract.Database.Feedback feedback)
    {
        dateText.text = feedback.GetDateString();
        if (feedback.stars==0)
        {
            starsNotSpecifiedText.SetActive(true);
            stars.gameObject.SetActive(false);
        }
        else
        {
            starsNotSpecifiedText.SetActive(false);
            stars.gameObject.SetActive(true);
            stars.Set(feedback.stars);
        }

        if (feedback.pain == 0)
        {
            painNotSpecifiedText.SetActive(true);
            pain.gameObject.SetActive(false);
        }
        else
        {
            painNotSpecifiedText.SetActive(false);
            pain.gameObject.SetActive(true);
            pain.Set(feedback.pain);
        }
        commentsText.SetText(feedback.comments);
    }
}
