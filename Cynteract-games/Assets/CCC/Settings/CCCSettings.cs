using UnityEngine.UI;
using Cynteract.Database;
using System.Threading.Tasks;
using UnityEngine;
using System.Linq;
using System;

namespace Cynteract.CCC
{
    public class CCCSettings : CCCWindow
    {
        public Button logoutButton, backButton;
        public Toggle vibrationToggle;
        public Slider globalVolumeSlider;
        public TMPro.TMP_Dropdown languageDropdown;
        string[] languages;
        protected override void OnClose(bool wasDestroyed)
        {
            if (!wasDestroyed)
            {
                SaveSettings();
                CynteractControlCenter.ActiveWindow.Open();
            }
        }

        private static async void SaveSettings()
        {
            await DatabaseManager.instance.SyncSettings();
        }

        protected override void OnInit()
        {
            
            logoutButton.onClick.AddListener(() => CCCStatusBar.instance.Logout()) ;
            vibrationToggle.onValueChanged.AddListener(OnToggledVibration);
            UserSettings userSettings = DatabaseManager.instance.GetDatabaseSettings();
            vibrationToggle.isOn = userSettings.useVibration;

            globalVolumeSlider.onValueChanged.AddListener(OnChangedVolume);
            languageDropdown.onValueChanged.AddListener(ChangedLanguage);
            languages=Lean.Localization.LeanLocalization.CurrentLanguages.Keys.ToArray();
            var language = userSettings.language;
            languageDropdown.options = languages.Select(x=>new TMPro.TMP_Dropdown.OptionData(x)).ToList();
            languageDropdown.value=Array.FindIndex(languages,x=>x==language);
            languageDropdown.onValueChanged.AddListener(ChangedLanguage);
            globalVolumeSlider.value = userSettings.globalVolume;
            backButton.onClick.AddListener(() => CCCStatusBar.instance.CloseAllOverlays());
            
        }

        private void ChangedLanguage(int index)
        {
            
            var settings=DatabaseManager.instance.GetDatabaseSettings();
            settings.language = languages[index];
            DatabaseManager.instance.SetSettings(settings);
            LanguageManager.instance.UpdateLanguage();
            
        }

        private void OnToggledVibration(bool arg0)
        {
            var settings = DatabaseManager.instance.GetDatabaseSettings();
            settings.useVibration = arg0;
            DatabaseManager.instance.SetSettings(settings);
        }
        private void OnChangedVolume(float volume)
        {
            var settings = DatabaseManager.instance.GetDatabaseSettings();
            settings.globalVolume = volume;
            DatabaseManager.instance.SetSettings(settings);
            AudioManager.instance.UpdateVolume(volume);
        }
        protected override void OnOpen()
        {
            var settings = DatabaseManager.instance.GetDatabaseSettings();
            vibrationToggle.isOn=settings.useVibration;

        }

    }
}