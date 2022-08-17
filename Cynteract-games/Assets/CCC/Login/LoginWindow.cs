using Cynteract.Database;
using Lean.Localization;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Cynteract.CCC
{
    public class LoginWindow : CCCWindow
    {
        private const string standardAdminUserName = "superduperuserino";
        private const string standardAdminPassword = "youllneverguessit";
        public TMP_InputField userNameInputField, passwordInputField;
        public Button loginButton, showPasswordButton, resetPasswordButton;
        public TextMeshProUGUI showPasswordButtonText;
        public GameObject loginFailedMessage;

        public ResetPasswordPopupWindow resetPasswordPopup;
        public YesNoPopupWindow confirmDSGVOPopup;
        public HomeLoadingScreen loadingScreen;
        private bool canPressEnter;

        protected override void OnInit()
        {
            showPasswordButton.onClick.AddListener(TogglePasswordVisibility);
            EventSystem.current.SetSelectedGameObject(userNameInputField.gameObject, null);

            loginButton.onClick.AddListener(Login);
            userNameInputField.ActivateInputField();
            resetPasswordButton.onClick.AddListener(ResetPassword);
        }

        protected override void OnOpen()
        {
            canPressEnter = true;
            loginFailedMessage.SetActive(false);

            CCCStatusBar.instance.StandardColor();
            LoadPreviousLoginData();
            showPasswordButtonText.text = LeanLocalization.GetTranslationText("Show password") ?? "Passwort anzeigen";
            passwordInputField.contentType = TMP_InputField.ContentType.Password;
        }
        protected override void OnClose(bool wasDestroyed)
        {
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {

                if (EventSystem.current.currentSelectedGameObject == userNameInputField.gameObject)
                {
                    EventSystem.current.SetSelectedGameObject(passwordInputField.gameObject, null);
                    passwordInputField.ActivateInputField();
                }
                else
                {

                    EventSystem.current.SetSelectedGameObject(userNameInputField.gameObject, null);
                    userNameInputField.ActivateInputField();
                }

            }
            if (canPressEnter&&Input.GetKeyDown(KeyCode.Return))
            {
                Login();
            }
        }
        private void LoadPreviousLoginData()
        {
            /*
            if (LoginManager.TryGetLastUserdata(out LoginManager.UnhashedLoginData data))
            {
                userNameInputField.text = data.username;
                passwordInputField.text = data.password;
            }*/
        }
        private async void Login()
        {
            SetInputFields(false);
            var result=await DatabaseManager.instance.Login(userNameInputField.text, passwordInputField.text);
            if (result==LoginResponse.Success)
            {
                OnSuccess();
            }
            else
            {
                OnFail();
            }
        }
        private void TogglePasswordVisibility()
        {
            if (passwordInputField.contentType == TMP_InputField.ContentType.Password)
            {
                showPasswordButtonText.text = LeanLocalization.GetTranslationText("Hide password") ?? "Passwort verstecken";
                passwordInputField.contentType = TMP_InputField.ContentType.Standard;
            }
            else
            {
                showPasswordButtonText.text = LeanLocalization.GetTranslationText("Show password") ?? "Passwort anzeigen";
                passwordInputField.contentType = TMP_InputField.ContentType.Password;
            }
            passwordInputField.ForceLabelUpdate();
        }
        private void SetInputFields(bool value)
        {
            userNameInputField.interactable = value;
            passwordInputField.interactable = value;
            loginButton.interactable = value;
        }
        private async void OnSuccess()
        {
            canPressEnter = false;
            SetInputFields(true);
            bool consented = DatabaseManager.instance.GetOrAddConfig()?.Consented??false;
            if (!consented)
            {
                consented = await ConfirmDSGVOTask();
                Debug.Log("setting Consented");
                DatabaseManager.instance.SetConsent(consented);
            }
            if (!consented)
            {
                await DatabaseManager.instance.Logout();
                canPressEnter = true;
                return;
            }
            loginFailedMessage.SetActive(false);
            loadingScreen.gameObject.SetActive(true);

            LanguageManager.instance.UpdateLanguage();
            loadingScreen.gameObject.SetActive(false);
            CCCStatusBar.SetUserName(DatabaseManager.instance.User.username);
            CynteractControlCenter.Welcome();
            CConsole.Log("success", CSubConsoleType.CCC);
        }
        private void OnAdminSuccess()
        {
            loginFailedMessage.SetActive(false);
            CCCStatusBar.instance.Therapist();
        }
        private void OnFail()
        {
            loginFailedMessage.SetActive(true);
            SetInputFields(true);
            EventSystem.current.SetSelectedGameObject(userNameInputField.gameObject, null);
            userNameInputField.ActivateInputField();
            CConsole.Log("Fail", CSubConsoleType.CCC);
        }

        public async void ResetPassword()
        {
            canPressEnter = false;
            var changed = await ResetPasswordTask();
            if (changed)
            {
                await CynteractControlCenter.DisplayMessagePopup(LeanLocalization.GetTranslationText("Reset link sent"));
            }
            canPressEnter = true;
        }
        private Task<bool> ResetPasswordTask()
        {
            var task = new TaskCompletionSource<bool>();
            var logoPopup = Instantiate(resetPasswordPopup, CynteractControlCenter.instance.popupWindowCanvas);
            logoPopup.Init();
            logoPopup.SubscribeOnClosed(x => task.SetResult(x));
            return task.Task;
        }
        private Task<bool> ConfirmDSGVOTask()
        {

            var task = new TaskCompletionSource<bool>();
            var logoPopup = Instantiate(confirmDSGVOPopup, CynteractControlCenter.instance.popupWindowCanvas);
            logoPopup.Init();
            logoPopup.SubscribeOnClosed(x => task.SetResult(x));
            return task.Task;
        }
    }
}