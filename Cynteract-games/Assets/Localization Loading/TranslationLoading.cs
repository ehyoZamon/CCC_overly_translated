using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Localization;
public class TranslationLoading : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        //LeanLocalization.CurrentLanguages.Add("German", new LeanLanguage());
        var phrase=LeanLocalization.AddPhraseToFirst("Hi");
        phrase.AddEntry("English", "Hi");
        phrase.AddEntry("German", "Hallo");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
