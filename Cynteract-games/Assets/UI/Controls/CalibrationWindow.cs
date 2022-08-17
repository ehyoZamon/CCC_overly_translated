using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Sirenix.OdinInspector;
using UnityEngine.Video;

namespace Cynteract.CynteractInput
{

    public class CalibrationWindow : SerializedMonoBehaviour
    {
        public Button button;
        public Dictionary<string, VideoClip> clips;
        public VideoPlayer videoPlayer;
        public TextMeshProUGUI buttonText, descriptionText;
        public ICalibration bindingUI;
        public static CalibrationWindow instance;
        private bool clipSet;

        public void SetCountdown(int time)
        {
            buttonText.text = Lean.Localization.LeanLocalization.GetTranslationText("Skip") + "(" + time + ")";
        }
        public CalibrationWindow()
        {
            instance = this;
        }
        public void StopCalibration()
        {
            bindingUI.StopCalibration();
        }
        public void SetDescription(string description)
        {
            descriptionText.text = Lean.Localization.LeanLocalization.GetTranslationText( description)??description;
        }
        private void Update()
        {

        }
        public void SetAnimation(string animation)
        {
            if (clips.ContainsKey(animation))
            {
                videoPlayer.clip = clips[animation];
                videoPlayer.Play();

            }

        }
        
    }
}