using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Lean.Localization;
public class PasswordField : MonoBehaviour
{
    public Button showPasswordToggle;
    public TMP_InputField inputField;
    public TextMeshProUGUI buttonText;
    private bool passwordVisible;
    private void Awake()
    {
        showPasswordToggle.onClick.AddListener(TogglePassword);
    }
    // Start is called before the first frame update
    void Start()
    {
        HidePassword();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void TogglePassword()
    {
        if (passwordVisible)
        {
            HidePassword();
        }
        else
        {
            ShowPassword();
        }
    }
    public void ShowPassword()
    {
        passwordVisible = true;
        inputField.contentType = TMP_InputField.ContentType.Standard;
        buttonText.text=  LeanLocalization.GetTranslationText("Hide")?? "Verstecken";
        inputField.ForceLabelUpdate();
    }
    public void HidePassword()
    {
        passwordVisible = false;
        inputField.contentType = TMP_InputField.ContentType.Password;
        buttonText.text = LeanLocalization.GetTranslationText("Show") ?? "Anzeigen";
        inputField.ForceLabelUpdate();
    }
}
