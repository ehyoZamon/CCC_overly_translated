using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Lean.Localization;
namespace Cynteract.CCC
{
    public class Message : MonoBehaviour
    {
        public TextMeshProUGUI text;
        public void Show()
        {
            gameObject.SetActive(true);
        }
        public void DisplayTranslated(string message, string fallbackText = "No Translation found")
        {
            Display( LeanLocalization.GetTranslationText(message) ?? fallbackText);
        }
        public void Display(string v)
        {
            text.text = v;
            Show();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}