using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.CCC
{
    public abstract class CCCWindow : MonoBehaviour
    {
        [ReadOnly]
        public bool isOpen=false;
        bool initialized = false;
        void Start()
        {
            Init();
        }
        public void Open()
        {
            Init();
            isOpen = true;
            gameObject.SetActive(true);
            OnOpen ();
        }
        public void Close()
        {
            isOpen = false;
            gameObject.SetActive(false);
            OnClose(false);
        }
        protected virtual void OnDestroy()
        {
            OnClose(true);
        }
        protected abstract void OnOpen();
        protected abstract void OnClose(bool wasDestroyed);
        protected abstract void OnInit();
        void Init()
        {
            if (!initialized)
            {
                OnInit();
            }
            initialized = true;
        }

    }
}