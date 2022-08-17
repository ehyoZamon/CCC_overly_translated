using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.CCC
{
    public class LanguageManager : MonoBehaviour
    {
        public static LanguageManager instance;
        private void Awake()
        {
            instance = this;
        }
        // Start is called before the first frame update
        void Start()
        {
            LanguageSettings languageSettings;
            try
            {
                languageSettings=JsonFileManager.Load<LanguageSettings>("languageSettings", JsonFileManager.Type.Newtonsoft);
            }
            catch (Exception)
            {
                languageSettings = new LanguageSettings();
                languageSettings.language = "German";
            }
            
            Lean.Localization.LeanLocalization.CurrentLanguage = languageSettings.language;

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void UpdateLanguage()
        {
            string language = Database.DatabaseManager.instance.GetDatabaseSettings().language;
            if (language != null && language != "")
            {
                Lean.Localization.LeanLocalization.CurrentLanguage = language;
                JsonFileManager.Save(new LanguageSettings() { language = language }, "languageSettings", JsonFileManager.Type.Newtonsoft);
            }
        }
    }
}