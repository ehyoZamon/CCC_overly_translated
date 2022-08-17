using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace Cynteract.Sphererunner
{
    public class LevelSwitchTrigger : MonoBehaviour
    {
        AudioSource audioSource;
        float lowValue = 5.25f;
        float highValue = 10.5f;
        public Transform backblocker;
        public Transform[] nextBlocker;
        public float delay;
        bool triggered = false;
        public Transform spawnPoint;
        // Use this for initialization
        private void OnTriggerEnter2D(Collider2D collision)
        {
            triggerNextLevel();

        }

        public void triggerNextLevel()
        {
            if (!triggered)
            {
                triggered = true;
               WorldGenerator.instance.NextLevel();
                block();
                WorldGenerator.instance.lastLevelSwitcher = spawnPoint.position;
            }
        }
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }
        private void block()
        {
            Vector3 pos = backblocker.localPosition;
            pos.y = lowValue;
            backblocker.localPosition = pos;
          //  audioSource.Play();
            StartCoroutine(pullUpNextBlockers());
        }

        private IEnumerator pullUpNextBlockers()
        {
            for (int i = 0; i < nextBlocker.Length; i++)
            {
                StartCoroutine(pullUp(nextBlocker[i]));

                yield return new WaitForSeconds(delay);
            }
        }

        private IEnumerator pullUp(Transform thingtoPullUp)
        {
            while (thingtoPullUp.localPosition.y < highValue)
            {
                Vector3 pos = thingtoPullUp.localPosition;
                pos.y = Mathf.Min(pos.y + Time.deltaTime * 10f, highValue);
                thingtoPullUp.localPosition = pos;

                yield return null;
            }
        }
    }
#if UNITY_EDITOR
    [CustomEditor(typeof(LevelSwitchTrigger))]
    public class LevelSwitcherEditor : Editor
    {
        public override void OnInspectorGUI()
        {

            LevelSwitchTrigger levelSwitcher = (LevelSwitchTrigger)(target);
            base.OnInspectorGUI();
            if (Application.isPlaying)
            {
                if (GUILayout.Button("triggerNextLevel"))
                {
                    levelSwitcher.triggerNextLevel();
                }
            }

        }
    }
#endif
}