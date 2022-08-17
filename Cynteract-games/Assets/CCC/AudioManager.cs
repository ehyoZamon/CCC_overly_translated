using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Cynteract.CCC
{
    public class AudioManager : MonoBehaviour
    {
        public AudioMixer audioMixer;
        public static AudioManager instance;
        private void Awake()
        {
            instance = this;
        }
        private void Start()
        {
            LowPassOff();
        }
        public void UpdateVolume(float volume)
        {
            audioMixer.SetFloat("MasterVolume", ToVolume(volume));
        }
        public void LowPassMax()
        {
            audioMixer.SetFloat("MasterLowpassAmount", ToVolume(1));
        }
        public void LowPassOff()
        { 
            audioMixer.SetFloat("MasterLowpassAmount", ToVolume(0));
        }
        private static float ToVolume(float value)
        {
            return (1-value)*(-80);
        }
    }
}