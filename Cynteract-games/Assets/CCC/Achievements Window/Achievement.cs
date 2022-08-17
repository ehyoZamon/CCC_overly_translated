using Cynteract.CCC;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Achievement : MonoBehaviour
{
    public GameObject activatedIcon, deactivatedIcon;
    public TextMeshProUGUI titleTM, descriptionTM,scoreCounter;
    private string key;
    private string description;

    public void Set(bool active, int currentScore, int totalScore)
    {
        activatedIcon.SetActive(active);
        deactivatedIcon.SetActive(!active);
        scoreCounter.text = $"{currentScore}/{totalScore}";
    }
    public void Set(AchievementManager.CurrentData data)
    {
        Set(data.active, data.currentValue, data.totalValue);
    }
    public void Init(string key, string description)
    {
        this.key = key;
        this.description = description;

    }
    public void Open()
    {
        string titleText = Lean.Localization.LeanLocalization.GetTranslationText(key) ?? key;
        string descriptionText = Lean.Localization.LeanLocalization.GetTranslationText(description) ?? description;
        titleTM.text = titleText;
        descriptionTM.text = descriptionText;
    }


}
