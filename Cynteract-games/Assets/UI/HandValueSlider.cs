using Cynteract.CynteractInput;
using Lean.Localization;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandValueSlider : MonoBehaviour
{
    public Type type;
    public string key;
    public Slider slider;
    public TextMeshProUGUI textMesh;
    public GameObject actionHandles;
    public Image fillColor;
    public Color trueColor, falseColor;
    private void Update()
    {
        switch (type)
        {
            case Type.Axis:
                actionHandles.SetActive(false);
                slider.value = CInput.GetAxis(key);
                textMesh.text = LeanLocalization.GetTranslationText( CInput.GetInputAxis(key).name);

                break;
            case Type.Action:
                actionHandles.SetActive(true);
                if (CInput.GetAction(key))
                {
                    fillColor.color = trueColor;
                }
                else
                {
                    fillColor.color = falseColor;
                }
                textMesh.text = LeanLocalization.GetTranslationText(CInput.GetInputAction(key).name);
                slider.value = CInput.GetActionValue(key);
                break;
            default:
                break;
        }
    }
}
