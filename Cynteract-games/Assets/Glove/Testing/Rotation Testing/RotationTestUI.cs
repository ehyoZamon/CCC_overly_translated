using Cynteract.CGlove;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RotationTestUI : MonoBehaviour
{
    public Button absoluteModeButton, resetModeButton, relativeModeButton;
    public Button resetButton, uploadButton;
    public Color buttonOnColor, buttonOffColor;
    public GloveDisplayer gloveDisplayer;
    private void Awake()
    {
        absoluteModeButton.onClick.AddListener(AbsoluteMode);
        resetModeButton.onClick.AddListener(ResetMode);
        relativeModeButton.onClick.AddListener(RealtiveMode);
        resetButton.onClick.AddListener(ResetAll);
        uploadButton.onClick.AddListener(SwitchToUploadScene);
    }

    private void SwitchToUploadScene()
    {
        SceneManager.LoadScene("ESPUploadScene");
    }

    private void Start()
    {
        AbsoluteMode();
    }
    public void AbsoluteMode()
    {
        absoluteModeButton.image.color = buttonOnColor;
        resetModeButton.image.color = buttonOffColor;
        relativeModeButton.image.color = buttonOffColor;
        gloveDisplayer.rotationType = GloveData.RotationSet.RotationType.Absolute;
    }
    public void ResetMode()
    {
        absoluteModeButton.image.color = buttonOffColor;
        resetModeButton.image.color = buttonOnColor;
        relativeModeButton.image.color = buttonOffColor;
        gloveDisplayer.rotationType = GloveData.RotationSet.RotationType.Reset;

    }
    public void RealtiveMode()
    {
        absoluteModeButton.image.color = buttonOffColor;
        resetModeButton.image.color = buttonOffColor;
        relativeModeButton.image.color = buttonOnColor;
        gloveDisplayer.rotationType = GloveData.RotationSet.RotationType.Relative;
    }
    public void ResetAll()
    {
        gloveDisplayer.ResetAll();
    }
}
