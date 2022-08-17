using Cynteract.CTime;
using Cynteract.Tunnelrunner;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Lean.Localization;
public interface ITutorial
{
    void StartTutorial();
    void StopTutorial();
}
public class TunnelrunnerTutorial : MonoBehaviour, ITutorial
{
    public TextMeshProUGUI tutorialText;
    private bool textBlocked;
    public AnimationCurve curve;
    IEnumerator TutorialRoutine()
    {
        Pause();
        yield return new WaitForSecondsPrioritized(1, TimeControl.Priority.Menu);
        yield return new WaitUntil(() => !TunnelrunnerInput.GetAction(TunnelrunnerInput.boost));
        textBlocked = true;
        tutorialText.text = LeanLocalization.GetTranslationText("Try not to touch the walls");
        yield return new WaitForSecondsPrioritized(3, TimeControl.Priority.Menu);
        Unpause();
        tutorialText.text = "";
        textBlocked = false;
        yield return new WaitForSecondsPrioritized(30, TimeControl.Priority.Menu);
        textBlocked = true;
        tutorialText.text = LeanLocalization.GetTranslationText("Collecting stars provides additional points");
        yield return new WaitForSecondsPrioritized(3, TimeControl.Priority.Menu);
        tutorialText.text = "";
        textBlocked = false;
        yield return new WaitForSecondsPrioritized(5, TimeControl.Priority.Menu);
        StopTutorial();
    }


    IEnumerator SafetyRoutine()
    {
        while (true)
        {
            float ceillingDistance = Tunnelrunner.rocketControl.GetCeillingDistance();
            float floorDistance = Tunnelrunner.rocketControl.GetFloorDistance();

            float min = Mathf.Min(ceillingDistance, floorDistance);
            float sum = ceillingDistance + floorDistance;
            float percent = min / sum;
            print(percent);
            float scale = curve.Evaluate(percent);

            if (ceillingDistance < floorDistance)
            {
                if (!TunnelrunnerInput.GetAction(TunnelrunnerInput.boost))
                {
                    TimeControl.instance.ScaleTime(TimeControl.Priority.Standard, 1);
                }
                else
                {
                    TimeControl.instance.ScaleTime(TimeControl.Priority.Standard, scale);
                }
                if (!textBlocked)
                {
                    if (percent < .4)
                    {
                        tutorialText.text = LeanLocalization.GetTranslationText("Open your hand to fall");
                    }
                    else
                    {
                        tutorialText.text = "";
                    }
                }

            }
            else
            {
                if (TunnelrunnerInput.GetAction(TunnelrunnerInput.boost))
                {
                    TimeControl.instance.ScaleTime(TimeControl.Priority.Standard, 1);
                }
                else
                {
                    TimeControl.instance.ScaleTime(TimeControl.Priority.Standard, scale);
                }
                if (!textBlocked)
                {
                    if (percent < .4)
                    {
                        tutorialText.text = LeanLocalization.GetTranslationText("Close your hand to rise");
                    }
                    else
                    {
                        tutorialText.text = "";
                    }
                }
            }
            yield return null;
        }
    }

private static void Unpause()
{
    TimeControl.instance.ResetTime(TimeControl.Priority.High) ;
}

private static void Pause()
{
    TimeControl.instance.ScaleTime(TimeControl.Priority.High, 0);
}

public void StartTutorial()
{
    StartCoroutine(TutorialRoutine());
    StartCoroutine(SafetyRoutine());
    }
public void StopTutorial()
{
    StopAllCoroutines();
    Tunnelrunner.rocketControl.tempSettings.invincible = false;
    Tunnelrunner.instance.StartStandardLevel();
    gameObject.SetActive(false);
}
}
