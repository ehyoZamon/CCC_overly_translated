using Cynteract.CTime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Lean.Localization;
namespace Cynteract.Sphererunner {
    public class SphererunnerTutorial : MonoBehaviour, ITutorial
    {
        public TextMeshProUGUI tutorialText;
        public static SphererunnerTutorial instance;
        Coroutine tutorialRoutine;
        Coroutine hintRoutine;
        IEnumerator TutorialRoutine()
        {
            SphereMovement.instance.autoDirection = .5f;
            yield return new WaitForSecondsPrioritized(3, TimeControl.Priority.Menu);
            SphereMovement.instance.autoDirection = .2f;

            if (SphereMovement.instance.Scale01 > .5f)
            {
                yield return StartCoroutine(CloseHandRoutine());
                yield return new WaitForSecondsPrioritized(1, TimeControl.Priority.Menu);
                yield return StartCoroutine(OpenHandRoutine());
            }
            else
            {
                yield return StartCoroutine(OpenHandRoutine());
                yield return new WaitForSecondsPrioritized(1, TimeControl.Priority.Menu);
                yield return StartCoroutine(CloseHandRoutine());
            }





            StopTutorial();
            SphereMovement.instance.autoDirection = 1f;

            yield return null;
        }

        public void ShowHintAfterTime(SphereHelper.Type type, float time)
        {
            hintRoutine=StartCoroutine(ShowHintAfterTimeRoutine(type, time));
        }
        public void AborthowHintAfterTime()
        {
            StopCoroutine(hintRoutine);
            tutorialText.text = "";
        }

        private IEnumerator ShowHintAfterTimeRoutine(SphereHelper.Type type, float time)
        {
            yield return new WaitForSeconds(time);
            switch (type)
            {
                case SphereHelper.Type.Big:
                    tutorialText.text =  LeanLocalization.GetTranslationText("Open your hand to enlarge the ball");
                    break;
                case SphereHelper.Type.Small:
                    tutorialText.text = LeanLocalization.GetTranslationText("Close your hand to reduce the size of the ball");
                    break;
                default:
                    break;
            }
        }

        public IEnumerator OpenHandRoutine()
        {
            SphereMovement.instance.autoDirection = .2f;
            tutorialText.text = LeanLocalization.GetTranslationText("Open your hand to enlarge the ball");
            yield return new WaitForSecondsPrioritized(3, TimeControl.Priority.Menu);
            SphereMovement.instance.autoDirection = 0f;
            yield return new WaitUntil(() => SphereMovement.instance.Scale01 > .8f);
            tutorialText.text = "";
        }
        public IEnumerator CloseHandRoutine()
        {
            SphereMovement.instance.autoDirection = .2f;
            tutorialText.text = LeanLocalization.GetTranslationText("Close your hand to reduce the size of the ball");
            yield return new WaitForSecondsPrioritized(3, TimeControl.Priority.Menu);
            SphereMovement.instance.autoDirection = 0f;
            yield return new WaitUntil(() => SphereMovement.instance.Scale01 < .2f);
            tutorialText.text = "";
        }
        void Awake()
        {
            instance = this;
        }
        private static void Unpause()
        {
            TimeControl.instance.ResetTime(TimeControl.Priority.High);
        }

        private static void Pause()
        {
            TimeControl.instance.ScaleTime(TimeControl.Priority.High, 0);
        }

        public void StartTutorial()
        {
            tutorialRoutine=StartCoroutine(TutorialRoutine());
        }

        public void StopTutorial()
        {
            StopCoroutine(tutorialRoutine);
        }
    }
}