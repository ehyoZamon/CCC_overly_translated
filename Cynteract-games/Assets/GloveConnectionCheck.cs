using Cynteract.CTime;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace Cynteract.CGlove
{
    public class GloveConnectionCheck : MonoBehaviour
    {
        public static GloveConnectionCheck instance;
        public WaitForConnectionPanel waitForGlovePanel;
        [ReadOnly]
        public WaitForConnectionPanel waitForGlovePanelInstance;
        public enum Mode
        {
            Left, Right, Both, Any
        }

        public bool debug;

        public Mode mode;
        public  Transform canvasTransformOverwrite;
        private object checkConnectionLock=new object();
        private Coroutine waitForGloveRoutine;
        Action<bool> onConnected;

        [ShowInInspector]
        [ReadOnly]
        public bool IsActive
        {
            get; private set;
        }
        public bool Check
        {
            get; private set;
        }

        private void Awake()
        {
            instance = this;
        }
        private void Start()
        {
            Glove.Left.SubscribeOnDisconnected(OnDisconnect);
            Glove.Right.SubscribeOnDisconnected(OnDisconnect);
        }



        [Button]
        public void CheckForConnection()
        {
            print("Checking");
            lock (checkConnectionLock)
            {
                bool connected = IsGloveConnected();
                if (!IsActive)
                {
                    if (connected)
                    {
                        onConnected?.Invoke(true);
                    }
                    else
                    {
                        IsActive = true;
                        StartCoroutine(ConnectionCheckRoutine());
                    }
                }
                else if(connected)
                {
                    onConnected?.Invoke(true);
                }
            }
        }
        public void StopChecking()
        {
            if (waitForGlovePanelInstance)
            {
                Destroy(waitForGlovePanelInstance.gameObject);
            }
            if (waitForGloveRoutine!=null)
            {
                StopCoroutine(waitForGloveRoutine);
                waitForGloveRoutine = null;
            }
            IsActive = false;

        }
        public Task<bool> CheckForConnectionAsync()
        { 
            onConnected=null;
            var task = new TaskCompletionSource<bool>();
            onConnected = x =>
            {
                onConnected = null;
                task.SetResult(x);
            };
            CheckForConnection();
            return task.Task;
        }
        public void StartContinouslyChecking()
        {
            CheckForConnection();
            Check = true;
        }
        public Task<bool> StartContinouslyCheckingAsync()
        {
            Check = true;
            return CheckForConnectionAsync();
        }
        public void StopContinouslyChecking()
        {
            StopChecking();
            Check = false;
        }
        private void OnDisconnect(GloveInformation information)
        {
            if (Check)
            {
                MainThreadUtility.CallInMainThread(CheckForConnection);
                
            }
        }
        IEnumerator ConnectionCheckRoutine()
        {
            TimeControl.instance.MenuPause();
            waitForGlovePanelInstance = Instantiate(waitForGlovePanel, GetTransform());
            waitForGloveRoutine=StartCoroutine(WaitForGloveRoutine());
            yield return waitForGloveRoutine;
            print("ConnectionChecked");
            IsActive = false;
            TimeControl.instance.MenuUnpause();

        }
        IEnumerator WaitForGloveRoutine()
        {
            string dot = ".";
            int numberOfDots = 0;
            while (true)
            {
                if (IsGloveConnected())
                {
                    break;
                }
                string text =  $" {Lean.Localization.LeanLocalization.GetTranslationText("Waiting for Glove Connection")} \n";
                for (int i = 0; i < numberOfDots; i++)
                {
                    text += dot;
                }
                numberOfDots = (numberOfDots + 1) % 10 + 1;
                waitForGlovePanelInstance.textMesh.text = text;
                yield return new WaitForSecondsRealtime(.5f);
            }
            print("GloveConnected");

            onConnected?.Invoke(true);
            StopChecking();
        }
        public bool IsGloveConnected()
        {
            if (debug && Input.GetKey(KeyCode.Space))
            {
                return true;
            }
            bool right = Glove.Right.information.RecievedInformation;
            bool left = Glove.Left.information.RecievedInformation;
            switch (mode)
            {
                case Mode.Left:
                    if (left)
                    {
                        return true;
                    }
                    break;
                case Mode.Right:
                    if (right)
                    {
                        return true;
                    }
                    break;
                case Mode.Both:
                    if (right && left)
                    {
                        return true;
                    }
                    break;
                case Mode.Any:
                    if (right || left)
                    {
                        return true;
                    }
                    break;
                default:
                    break;
            }
            return false;
        }
        private static Transform GetTransform()
        {
            if (instance.canvasTransformOverwrite != null)
            {
                return instance.canvasTransformOverwrite;
            }
            if (MainCanvas.instance && MainCanvas.instance.gameObject.scene.isLoaded)
            {
                return MainCanvas.instance.transform;

            }
            else
            {
                return CCC.CynteractControlCenter.instance.popupWindowCanvas;
            }
        }
    }
}