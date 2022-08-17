using Cynteract.CCC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeanLanguageSelector : MonoBehaviour
{
    TMP_Dropdown dropdown;
    private void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
    }
    private void OnEnable()
    {
        dropdown.ClearOptions();
        var languages = Lean.Localization.LeanLocalization.CurrentLanguages;
        if (languages != null&&languages.Count>0)
        {
            var options = new List<TMP_Dropdown.OptionData>();
            foreach (var item in languages)
            {
                options.Add(new TMP_Dropdown.OptionData(item.Key));
            }

            dropdown.AddOptions(options);
            /*
            dropdown.value = options.FindIndex(x=>x.text==GeneralSettings.Instance.GetLanguageString());
            dropdown.onValueChanged.AddListener(x => Lean.Localization.LeanLocalization.CurrentLanguage = options[x].text);
            dropdown.onValueChanged.AddListener(x => GeneralSettings.Instance.SetLanguage(options[x].text));*/
        }

        //Lean.Localization.LeanLocalization.CurrentLanguage = GeneralSettings.Instance.GetLanguageString();
    }
}
