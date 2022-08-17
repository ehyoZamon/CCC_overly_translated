using System;
using UnityEngine;
using UnityEngine.Video;

namespace Cynteract.CCC
{
    public class LogoVideoPopupWindow : PopupWindow
    {
        public VideoPlayer videoPlayer;
        public Action<bool> onClosed;
        protected override void OnClose()
        {
            onClosed(true);
        }

        protected override void OnInit()
        {
            videoPlayer.Play();
            videoPlayer.loopPointReached += EndReached;
        }
        void EndReached(VideoPlayer vp)
        {
            Close();
        }

        public void SubscribeOnClosed(Action<bool> p)
        {
            onClosed = p;
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Close();
            }
        }
    }
}