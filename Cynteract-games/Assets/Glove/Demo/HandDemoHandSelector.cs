using Cynteract.CGlove;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandDemoHandSelector : MonoBehaviour
{
    public GameObject leftHand , leftHandAbsolute, rightHand,  rightHandAbsolute;
    public GloveData.RotationSet.RotationType rotationType;
    public Text modeTextMesh, buttonTextMesh;
    public Button modeSwitchButton;
    private void Awake()
    {
        modeTextMesh.text = "Modus: Absolute Werte";
        buttonTextMesh.text = "Handdemo";
        rotationType = GloveData.RotationSet.RotationType.Reset;
        modeSwitchButton.onClick.AddListener(SwitchMode);
    }
    void SwitchMode()
    {
        switch (rotationType)
        {
            case GloveData.RotationSet.RotationType.Absolute:
                break;
            case GloveData.RotationSet.RotationType.Reset:
                modeTextMesh.text = "Modus: Handdemo";
                buttonTextMesh.text = "Absolute Werte";
                rotationType = GloveData.RotationSet.RotationType.Relative;
                break;
            case GloveData.RotationSet.RotationType.Relative:
                modeTextMesh.text = "Modus: Absolute Werte";
                buttonTextMesh.text = "Handdemo";
                rotationType = GloveData.RotationSet.RotationType.Reset;
                break;
            case GloveData.RotationSet.RotationType.Calibrated:
                break;
        }
    }
    private void Update()
    {
        bool relative = rotationType == GloveData.RotationSet.RotationType.Relative;
        bool left = Glove.Left.information.side != Side.NotSet;
        bool right = Glove.Right.information.side != Side.NotSet;

        leftHand.SetActive(left&&relative);
        rightHand.SetActive(right&&relative);

        leftHandAbsolute.SetActive(left && !relative);
        rightHandAbsolute.SetActive(right && !relative);
    }

}
