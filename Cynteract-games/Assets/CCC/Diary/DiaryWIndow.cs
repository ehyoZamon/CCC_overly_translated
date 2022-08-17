using Cynteract.CCC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cynteract.Database;
using UnityEngine.UI;

namespace Cynteract.CCC
{
    public class DiaryWIndow : CCCWindow
    {
        public FeedbackPanel feedbackPanelPrefab;
        public List<FeedbackPanel> feedbackPanels;
        public RectTransform contentRect;
        public Button backButton;
        protected override void OnClose(bool wasDestroyed)
        {
        }

        protected override void OnInit()
        {
            backButton.onClick.AddListener(CynteractControlCenter.Home);
        }

        protected override void OnOpen()
        {
            var feedback=DatabaseManager.instance.GetFeedback();
            DrawFeedback(feedback);
        }
        void DrawFeedback(List<Feedback> feedbacks)
        {
            if (feedbackPanels!=null)
            {
                foreach (var item in feedbackPanels)
                {
                    Destroy(item.gameObject);
                }
            }
            var height = feedbacks.Count * 200;
            contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, height);
            feedbackPanels = new List<FeedbackPanel>();
            foreach (var item in feedbacks)
            {
                var feedbackPanel = Instantiate(feedbackPanelPrefab, contentRect);
                feedbackPanel.Init(item);
                feedbackPanels.Add(feedbackPanel);
            }
        }
    }
}