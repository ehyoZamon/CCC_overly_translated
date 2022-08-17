
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Lean.Localization;
using Cynteract.Database;
using UnityEngine.Events;
using SimpleFileBrowser;
using System.IO;
using System.Linq;
using NPOI.HSSF.UserModel;
using Newtonsoft.Json.Linq;

namespace Cynteract.CCC
{
    public class ProfileWindow : CCCWindow
    {
        public TextMeshProUGUI emailTextMesh, userNameTextMesh;
        public Button backButton,changeEmailButton, changePasswordButton, deleteDataButton, deleteAccountButton, exportDataButton, allowAccessButton;
        public ChangePasswordPopupWindow changePasswordPopupWindow;
        public ChangeEmailPopupWindow changeEmailPopupwindow;
        public DeleteDataPopupWindow deleteDataPopupWindow;
        public DeleteAccountPopupWindow deleteAccountPopupWindow;
        public ExportDataPopupwindow exportDataPopupwindow;
        public AllowDataAccessPopupWIndow allowAccessPopupWindow;
        public GameObject onlineScreen,offlineScreen;
        public DataSharedPanel dataSharedPanel;

        protected override void OnClose(bool wasDestroyed)
        {
        }

        protected override void OnInit()
        {
            offlineScreen.SetActive(false);
            offlineScreen.SetActive(true);

            changeEmailButton.onClick.AddListener(ChangeEmail);
            changePasswordButton.onClick.AddListener(ChangePassword);
            deleteDataButton.onClick.AddListener(DeleteData);
            deleteAccountButton.onClick.AddListener(DeleteAccount);
            exportDataButton.onClick.AddListener(ExportData);
            backButton.onClick.AddListener( () => { CCCStatusBar.instance.CloseAllOverlays(); CynteractControlCenter.ActiveWindow.Open(); });
            allowAccessButton.onClick.AddListener(AllowAccess);
            dataSharedPanel.revokeButton.onClick.AddListener(RevokeAccess);
            DatabaseManager.instance.SubscribeOnConnected(new Callback(() => {
                if (isOpen) { MainThreadUtility.CallInMainThread(OnOpen); } 
            }, CallbackType.Once));
            DatabaseManager.instance.SubscribeOnDisconnected(new Callback(() => { if (isOpen) { MainThreadUtility.CallInMainThread(OnOpen); } }, CallbackType.Forever));

        }



        protected override void OnOpen()
        {

            var profileOf = LeanLocalization.GetTranslationText("Profile of") ?? "Profil von";
            userNameTextMesh.text = profileOf + " " + DatabaseManager.instance.User.Username;
            if (DatabaseManager.instance.IsOnline)
            {
                Init();
            }
            else
            {
                InitOffline();
            }
        }
        private void InitOffline()
        {
            offlineScreen.SetActive(true);
            onlineScreen.SetActive(false);
        }



        private async void Init()
        {
            offlineScreen.SetActive(false);
            onlineScreen.SetActive(true);
            string mail = DatabaseManager.instance.GetEmail();

            if (mail!=null&&mail != "")
            {
                emailTextMesh.text = mail;
            }
            else
            {
                emailTextMesh.text = LeanLocalization.GetTranslationText("No email set") ?? "Keine Email-Adresse angegeben";
            }
            if (await DatabaseManager.instance.AllowedAccess())
            {
                var therapistName = await DatabaseManager.instance.GetTherapistName();

                allowAccessButton.gameObject.SetActive(false);
                dataSharedPanel.gameObject.SetActive(true);
                dataSharedPanel.textMesh.text = LeanLocalization.GetTranslationText("Data shared wtih therapist:")+"\n" +therapistName;
            }
            else
            {
                allowAccessButton.gameObject.SetActive(true);
                dataSharedPanel.gameObject.SetActive(false);
            }

        }
        private void Update()
        {
            
            bool loggedIn = DatabaseManager.instance.IsOnline;
            changeEmailButton.interactable = loggedIn;
            changePasswordButton.interactable = loggedIn;
        }
        public async void ChangeEmail()
        {
            var changed = await ChangeEmailTask();
            if (changed)
            {
                await CynteractControlCenter.DisplayMessagePopup(LeanLocalization.GetTranslationText("E mail adress changed")??"Email wurde geändert");
                Init();
            }
        }
        public async void ChangePassword()
        {
            var changed=await ChangePasswordTask();
            if (changed)
            {
                await CynteractControlCenter.DisplayMessagePopup(LeanLocalization.GetTranslationText("Password was changed")??"Passwort wurde geändert");
            }
        }
        public async void DeleteData()
        {
            var deleted=await DeleteDataTask();
            if (deleted)
            {
                await CynteractControlCenter.DisplayMessagePopup(LeanLocalization.GetTranslationText("Data was deleted") ?? "Daten wurden gelöscht");
                Init();
            }
        }
        public async void DeleteAccount()
        {
            var deleted = await DeleteAccountTask();
        }
        public async void ExportData()
        {
            var exported = await ExportDataTask();
        }
        public async void AllowAccess()
        {
            var allowed = await AllowAccessTask();
            if (allowed)
            {
                Init();
            }
        }
        public async void RevokeAccess()
        {
            var revoked = await RevokeAccessTask();
            if (revoked)
            {
                Init();
            }
        }
        
        private Task<bool> ChangeEmailTask()
        {
            var task = new TaskCompletionSource<bool>();
            var logoPopup = Instantiate(changeEmailPopupwindow, CynteractControlCenter.instance.popupWindowCanvas);
            logoPopup.Init();
            logoPopup.SubscribeOnClosed(x => task.SetResult(x));
            return task.Task;
        }
        private Task<bool> ChangePasswordTask()
        {
            var task = new TaskCompletionSource<bool>();
            var logoPopup = Instantiate(changePasswordPopupWindow, CynteractControlCenter.instance.popupWindowCanvas);
            logoPopup.Init();
            logoPopup.SubscribeOnClosed(x => task.SetResult(x));
            return task.Task;
        }
        private Task<bool> DeleteDataTask()
        {
            var task = new TaskCompletionSource<bool>();
            var logoPopup = Instantiate(deleteDataPopupWindow, CynteractControlCenter.instance.popupWindowCanvas);
            logoPopup.Init();
            logoPopup.SubscribeOnClosed(x => task.SetResult(x));
            return task.Task;
        }
        private Task<bool> DeleteAccountTask()
        {
            var task = new TaskCompletionSource<bool>();
            var logoPopup = Instantiate(deleteAccountPopupWindow, CynteractControlCenter.instance.popupWindowCanvas);
            logoPopup.Init();
            logoPopup.SubscribeOnClosed(x => task.SetResult(x));
            return task.Task;
        }
        private Task<bool> ExportDataTask()
        {
            var task = new TaskCompletionSource<bool>();
            var logoPopup = Instantiate(exportDataPopupwindow, CynteractControlCenter.instance.popupWindowCanvas);
            logoPopup.Init();
            logoPopup.SubscribeOnClosed(x => task.SetResult(x));
            return task.Task;
        }
        private Task<bool> AllowAccessTask()
        {
            var task = new TaskCompletionSource<bool>();
            var logoPopup = Instantiate(allowAccessPopupWindow, CynteractControlCenter.instance.popupWindowCanvas);
            logoPopup.Init();
            logoPopup.SubscribeOnClosed(x => task.SetResult(x));
            return task.Task;
        }
        private async Task<bool> RevokeAccessTask()
        {
            var response = await DatabaseManager.instance.RevokeTherapistAccess();
            return true;
        }
    }
}