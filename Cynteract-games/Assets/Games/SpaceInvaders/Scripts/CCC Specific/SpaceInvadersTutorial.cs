using Cynteract.CTime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Lean.Localization;
namespace Cynteract.SpaceInvaders
{
    public class SpaceInvadersTutorial : MonoBehaviour, ITutorial
    {
        Coroutine tutorialRoutine;
        public TextMeshProUGUI tutorialText;
        IEnumerator TutorialRoutine()
        {
            LevelController.instance.DestroyOldWorld();
            LevelController.instance.RespawnPlayer();
            Backgroundcontroller.instance.HideStars();
            //LevelController.instance.HidePostProcessing();
            Backgroundcontroller.instance.GrayBackGround();
            tutorialText.text = "";
            yield return new WaitForSecondsPrioritized(3, TimeControl.Priority.Menu);

            if (GetMovement() > .5f)
            {
                yield return StartCoroutine(MoveLeftRoutine());
                yield return new WaitForSecondsPrioritized(1, TimeControl.Priority.Menu);
                yield return StartCoroutine(MoveRightRoutine());
            }
            else
            {
                yield return StartCoroutine(MoveRightRoutine());
                yield return new WaitForSecondsPrioritized(1, TimeControl.Priority.Menu);
                yield return StartCoroutine(MoveLeftRoutine());
            }
            LevelController.instance.CreateBrickWave();
            Backgroundcontroller.instance.BlueBackGround();
            Backgroundcontroller.instance.ShowStars();

            yield return new WaitForSecondsPrioritized(5, TimeControl.Priority.Menu);
            Backgroundcontroller.instance.StopMoving();
            LevelController.instance.StopBricks();

            tutorialText.text = LeanLocalization.GetTranslationText("Close your hand to shoot");
            yield return new WaitUntil(GetShooting);

            LevelController.instance.StartBricks();
            Backgroundcontroller.instance.StartMoving();
            tutorialText.text = "";
            yield return new WaitForSecondsPrioritized(10, TimeControl.Priority.Menu);

            StopTutorial();
        }
        public IEnumerator MoveLeftRoutine()
        {
            tutorialText.text = LeanLocalization.GetTranslationText("Tilt your hand to the left to fly left");
            yield return new WaitForSecondsPrioritized(3, TimeControl.Priority.Menu);
            yield return new WaitUntil(() => GetMovement() <.8f);
            tutorialText.text = "";
        }
        public IEnumerator MoveRightRoutine()
        {
            tutorialText.text = LeanLocalization.GetTranslationText("Tilt your hand to the right to fly right");
            yield return new WaitForSecondsPrioritized(3, TimeControl.Priority.Menu);
            yield return new WaitUntil(() => GetMovement() > .2f);
            tutorialText.text = "";
        }
        private static float GetMovement()
        {
            return SpaceInvaderInput.GetAxis(SpaceInvaderInput.move);
        }
        private static bool GetShooting()
        {
            return SpaceInvaderInput.GetAction(SpaceInvaderInput.shoot);
        }

        public void StartTutorial()
        {
            tutorialRoutine = StartCoroutine(TutorialRoutine());
        }

        public void StopTutorial()
        {
            LevelController.instance.SpawnWaves();
        }

    }
}