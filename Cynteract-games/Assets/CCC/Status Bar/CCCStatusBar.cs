using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cynteract.CGlove;
using UnityEngine.UI;
using TMPro;
using System;

namespace Cynteract.CCC {
    public class CCCStatusBar : MonoBehaviour
    {
        public GameObject leftGlove,rightGlove; 
        public Image background, leftConnectionDot, rightConnectionDot, onlineDot;
        public TextMeshProUGUI userNameText;
        public Transform userNamePanel, userNameDropdown;
        public HoverTooltip userNameHoverToolTip;
        public Button settingsButton, helpButton, exitButton;
        public Button userNamePanelButton, profileButton, logoutButton;
        public static CCCStatusBar instance;
        bool settingsActive, helpActive, profileActive;
        CCCWindow lastWindow;
        public Color patientColor, therapistColor;
        private void Awake()
        {
            instance = this;
        }



        private void Start()
        {
            settingsButton.onClick.AddListener(()=>ToggleSettings());
            helpButton.onClick.AddListener(()=>ToggleHelp());
            exitButton.onClick.AddListener(() => CynteractControlCenter.Quit());
            userNamePanelButton.onClick.AddListener(ToggleDropdown);
            profileButton.onClick.AddListener(OpenProfile);
            logoutButton.onClick.AddListener(Logout);
        }
        public void ToggleSettings()
        {
            if (settingsActive)
            {
                CloseAllOverlays();
            }
            else
            {
                CloseAllOverlays();
                settingsActive = true;
                CynteractControlCenter.OpenSettings();
            }
        }
        public void ToggleHelp()
        {
            if (helpActive)
            {
                CloseAllOverlays();
            }
            else
            {
                CloseAllOverlays();
                helpActive = true;
                CynteractControlCenter.OpenHelpWindow();
            }
        }
        public void OpenProfile()
        {
             CloseAllOverlays();
             profileActive = true;
             CynteractControlCenter.OpenProfileWindow();
        }
        public void CloseAllOverlays()
        {
            settingsActive = false;
            helpActive = false;
            profileActive = false;
            HideDropdown();
            CynteractControlCenter.CloseOverlays();
        }
        public void ToggleDropdown()
        {
            if (userNameDropdown.gameObject.activeSelf)
            {
                HideDropdown();
            }
            else
            {
                ShowDropdown();
            }
        }
        public void ShowDropdown()
        {
            userNameDropdown.gameObject.SetActive(true);
        }
        public void HideDropdown()
        {
            userNameDropdown.gameObject.SetActive(false);

        }
        public void Hide()
        {
            HideDropdown();
            gameObject.SetActive(false);
        }
        public void Show()
        {
            HideDropdown();
            gameObject.SetActive(true);
        }


        public  void Therapist()
        {
            background.color = therapistColor;
        }
        public  void StandardColor()
        {
            background.color = patientColor;
        }
        private void Update()
        {
            bool left = Glove.Left.information.RecievedInformation;
            bool right = Glove.Right.information.RecievedInformation;
            bool online = Cynteract.Database.DatabaseManager.instance.IsOnline;
            if (!left && !right)
            {
                leftGlove.SetActive(false);
                rightGlove.SetActive(true);
                rightConnectionDot.color = Color.red;
            }
            else if (left && right)
            {
                leftGlove.SetActive(false);
                rightGlove.SetActive(true);
                rightConnectionDot.color = Color.green;
            }
            else
            {
                if (left)
                {
                    leftConnectionDot.color = Color.green;
                    leftGlove.SetActive(true);
                }
                else
                {
                    leftGlove.SetActive(false);
                }
                if (right)
                {
                    rightConnectionDot.color = Color.green;
                    rightGlove.SetActive(true);
                }
                else
                {
                    rightGlove.SetActive(false);
                }
            }

            
            if (online)
            {
                onlineDot.color = Color.green;
            }
            else
            {
                onlineDot.color = Color.red;

            }
        }



        public static void SetUserName(string userName)
        {
            instance.userNameText.text = userName;
            if (instance.userNameText.text!="")
            {
                instance.ShowUserNamePanel();
            }
        }
        public void HideSettings()
        {
            settingsButton.gameObject.SetActive(false);
        }
        public void ShowSettings()
        {
            settingsButton.gameObject.SetActive(true);
        }
        public async void Logout()
        {
            HideUserNamePanel();
            HideSettings();
            userNameHoverToolTip.ForceLeave();

            CloseAllOverlays();
            await Database.DatabaseManager.instance.Logout();
            CynteractControlCenter.ConnectionWindow();
        }
        private void ShowUserNamePanel()
        {
            HideDropdown();
            instance.userNamePanel.gameObject.SetActive(true);
        }
        private void HideUserNamePanel()
        {
            HideDropdown();
            instance.userNamePanel.gameObject.SetActive(false);
        }
    }
}