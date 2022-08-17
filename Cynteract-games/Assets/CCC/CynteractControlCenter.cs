using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cynteract.CCC
{
    public class CynteractControlCenter : MonoBehaviour
    {
        #region PublicFields

        public bool showSplashScreen;
        public bool shutDown;
        public bool selfRegister;
        public bool ukaP;

        [ValueDropdown("Windows")]
        public CCCWindow startWindow;
        #region Windows
        public ConnectionWindow connectionWindow;
        public LoginWindow loginWindow;
        public HomeWindow homeWindow;
        public TrainingWindow trainingWindow;
        public EndOfTrainingWindow endOfTrainingWindow;
        public GameSelectionWindow gameSelectionWindow;
        public FinishWindow finishWindow;
        public AchievementsWindow achievementsWindow;
        public StatisticsWindow statisticsWindow;
        public ProfileWindow profileWindow;
        public UKAPWindow ukaPWindow;
        public CCCSettings settings;
        public HelpWindow helpWindow;
        public DiaryWIndow diaryWindow;
        public WelcomeScreenWindow welcomeScreenWindow;
        #endregion
        public TextMeshProUGUI versionText;
        #region Popups
        public TutorialPopupWindow tutorialPopupWindow;
        //public DiagnosisPopupWindow diagnosisPopupWindow;
        public LogoVideoPopupWindow logoVideoPopupWindow;
        public ExitConfirmationPopupWindow exitConfirmationPopupWindow;
        public YesNoPopupWindow playtimeWarningPopupWindow;
        public MessagePopupWindow messagePopupWindow, deprecatedVersionPopupWindow, oldVersionPopupWindow;
        public GameObject savePopupWindow;
        #endregion
        public CCCWindow[] instantiatedWindows;

        public GameObject instantiatedSaveWindow;

        public Transform windowsPanel, popupWindowCanvas;

        public CGame[] cGames;
        public static CynteractControlCenter instance;
        #endregion
        #region Private Fields
        List<CCCWindow> overlays = new List<CCCWindow>();
        public static CCCWindow ActiveWindow
        {
            get;
            private set;
        }
        #endregion
        #region Private Properties
        [ShowInInspector]
        CCCWindow[] Windows
        {
            get
            {
                Type selfType = GetType();
                var properties = selfType.GetFields();
                List<CCCWindow> windows = new List<CCCWindow>();
                foreach (var item in properties)
                {
                    if (item.Name != nameof(startWindow))
                    {
                        object value = item.GetValue(this);
                        if (value is CCCWindow)
                        {
                            windows.Add(value as CCCWindow);
                        }
                    }
                }
                return windows.ToArray();
            }
        }


        #endregion
        #region Unity Messages
        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
            }
            DontDestroyOnLoad(gameObject);
            instance = this;
        }


        private async void Start()
        {

            instantiatedWindows = new CCCWindow[Windows.Length];
            for (int i = 0; i < Windows.Length; i++)
            {
                instantiatedWindows[i] = Instantiate(Windows[i], windowsPanel);

                instantiatedWindows[i].gameObject.SetActive(false);
            }
            if (showSplashScreen)
            {
                await LogoVideo();
            }
            versionText.text =  Lean.Localization.LeanLocalization.GetTranslationText("Version")+" " + Application.version;

            Action action =  () => MainThreadUtility.CallInMainThread(async ()=> await DisplayDeprecatedVersionPopup());
            Database.DatabaseManager.instance.onDeprecatedVersionCallbacks.Add(new Callback(action, CallbackType.Once));
            Action versionErrorAction =  () => MainThreadUtility.CallInMainThread(async ()=> await DisplayOldVersionPopup());
            Database.DatabaseManager.instance.onOldVersionCallbacks.Add(new Callback(versionErrorAction, CallbackType.Once));

            Database.DatabaseManager.instance.Init();
            OpenWindow(startWindow);
        }
        #endregion
        #region Private Methods
        private void OpenWindowNonStatic(CCCWindow window)
        {
            foreach (var item in instantiatedWindows)
            {
                if (item.isOpen)
                {
                    if (item.GetType() != window.GetType())
                    {
                        item.Close();
                    }
                }
                else
                {
                    if (item.GetType() == window.GetType())
                    {
                        ActiveWindow = item;
                        item.Open();
                    }
                }
            }
        }
        private void OverlayWindow(CCCWindow window)
        {
            foreach (var item in instantiatedWindows)
            {

                if (item.GetType() == window.GetType())
                {
                    item.Open();
                    overlays.Add(item);

                }
            }
        }
        #endregion
        #region Public Methods
        public static void CloseOverlays()
        {
            foreach (var item in instance.overlays)
            {
                item.Close();
            }
            instance.overlays = new List<CCCWindow>();
        }

        public static void OpenWindow(CCCWindow window)
        {
            instance.OpenWindowNonStatic(window);
        }
        public static void EndOfTraining()
        {
            instance.OpenWindowNonStatic(instance.endOfTrainingWindow);
        }
        public static void ConnectionWindow()
        {
            instance.OpenWindowNonStatic(instance.connectionWindow);
        }
        public static void LoginWindow()
        {
            instance.OpenWindowNonStatic(instance.loginWindow);
        }
        public static void Welcome()
        {
            
            if (Database.DatabaseManager.instance.GetOrAddConfig().ShowWelcomeScreen)
            {
                WelcomeScreen();

            }
            else
            {
                Home();
            }
        }
        public static void WelcomeScreen()
        {
            instance.OpenWindowNonStatic(instance.welcomeScreenWindow);

        }
        public static void Home()
        {
            foreach (var item in instance.overlays)
            {
                item.isOpen = false;
            }
            CCCStatusBar.instance.ShowSettings();
            if (instance.ukaP)
            {
                instance.OpenWindowNonStatic(instance.ukaPWindow);
            }
            else
            {
            instance.OpenWindowNonStatic(instance.homeWindow);

            }
        }
        public static void Training()
        {
            instance.OpenWindowNonStatic(instance.trainingWindow);
        }
        public static void SelectGameWindow()
        {
            instance.OpenWindowNonStatic(instance.gameSelectionWindow);
        }
        public static async void Quit()
        {
            var close = await ExitCCCPopup();
            if (close)
            {
                if (instance.shutDown)
                {
                    ShutDownPc.ShutDown();
                }
                else
                {
                    GameQuitting.QuitGame();
                }
            }
        }
        public static void OpenSettings()
        {
            instance.OverlayWindow(instance.settings);
        }
        public static void OpenHelpWindow()
        {
            instance.OverlayWindow(instance.helpWindow);
        }
        public static void OpenProfileWindow()
        {
            instance.OverlayWindow(instance.profileWindow);
        }
        public static void FinishWindow()
        {
            instance.OpenWindowNonStatic(instance.finishWindow);

        }
        public static void AchievementsWindow()
        {
            instance.OpenWindowNonStatic(instance.achievementsWindow);
        }
        public static void StatisticsWindow()
        {
            instance.OpenWindowNonStatic(instance.statisticsWindow);
        }
        public static void DiaryWindow()
        {
            instance.OpenWindowNonStatic(instance.diaryWindow);
        }
        public static Task<bool> TutorialPopup()
        {
            var task = new TaskCompletionSource<bool>();
            var tutorialPopup = Instantiate(instance.tutorialPopupWindow, instance.popupWindowCanvas);
            tutorialPopup.Init();
            tutorialPopup.SubscribeOnClosed(x => task.SetResult(x));
            return task.Task;
        }
        public static Task<bool> ExitCCCPopup()
        {
            var task = new TaskCompletionSource<bool>();
            var logoPopup = Instantiate(instance.exitConfirmationPopupWindow, instance.popupWindowCanvas);
            logoPopup.Init();
            logoPopup.SubscribeOnClosed(x => task.SetResult(x));
            return task.Task;
        }
        public static Task<bool> LogoVideo()
        {
            var task = new TaskCompletionSource<bool>();
            var logoPopup = Instantiate(instance.logoVideoPopupWindow, instance.popupWindowCanvas);
            logoPopup.Init();
            logoPopup.SubscribeOnClosed(x => task.SetResult(x));
            return task.Task;
        }
        public static Task<bool> PlaytimeWarningPopup()
        {
            var task = new TaskCompletionSource<bool>();
            var logoPopup = Instantiate(instance.playtimeWarningPopupWindow, instance.popupWindowCanvas);
            logoPopup.Init();
            logoPopup.SubscribeOnClosed(x => task.SetResult(x));
            return task.Task;
        }
        public static Task<bool> DisplayMessagePopup(string message)
        {
            var task = new TaskCompletionSource<bool>();
            var popup = Instantiate(instance.messagePopupWindow, instance.popupWindowCanvas);
            popup.Init();
            popup.SubscribeOnClosed(x => task.SetResult(x));
            popup.SetText(message);
            return task.Task;
        }
        public static Task<bool> DisplayDeprecatedVersionPopup()
        {
            var task = new TaskCompletionSource<bool>();
            var popup = Instantiate(instance.deprecatedVersionPopupWindow, instance.popupWindowCanvas);
            popup.Init();
            popup.SubscribeOnClosed(x => {
                if (instance.shutDown)
                {
                    ShutDownPc.ShutDown();
                }
                else
                {
                    GameQuitting.QuitGame();
                }
                task.SetResult(x);
                });
            return task.Task;
        }
        public static Task<bool> DisplayOldVersionPopup()
        {
            var task = new TaskCompletionSource<bool>();
            var popup = Instantiate(instance.oldVersionPopupWindow, instance.popupWindowCanvas);
            popup.Init();
            popup.SubscribeOnClosed(x => {
                if (instance.shutDown)
                {
                    ShutDownPc.ShutDown();
                }
                else
                {
                    GameQuitting.QuitGame();
                }
                task.SetResult(x);
            });
            return task.Task;
        }
        public static void Saving()
        {
            instance.instantiatedSaveWindow = Instantiate(instance.savePopupWindow, instance.popupWindowCanvas);
        }
        public static void StopSaving()
        {
            if (instance.instantiatedSaveWindow)
            {
                Destroy(instance.instantiatedSaveWindow);
            }
        }
        public static void Diagnose()
        {
            ActiveWindow.Close();
            CloseOverlays();
            SceneManager.LoadScene("DiagnoseScene");
            CCCStatusBar.instance.gameObject.SetActive(false);
        }

        #endregion
    }

}
