using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if CHART_TMPRO
using TMPro;
#endif

namespace ChartUtil
{
    public class ChartTooltip : MonoBehaviour
    {
#if CHART_TMPRO
        public TextMeshProUGUI tooltipText;
#else
        public Text tooltipText;
#endif
        public Image background;
        public Vector2 chartSize;
        public Vector2 parentPivot;

        public void ValidatePosition()
        {
            Vector3 pos = transform.localPosition;
            pos.x = Mathf.Clamp(pos.x, -chartSize.x * parentPivot.x + background.rectTransform.sizeDelta.x * 0.5f, 
                chartSize.x * (1.0f - parentPivot.x) - background.rectTransform.sizeDelta.x * 0.5f);
            pos.y = Mathf.Clamp(pos.y, -chartSize.y * parentPivot.y, 
                chartSize.y * (1.0f - parentPivot.y) - background.rectTransform.sizeDelta.y);
            transform.localPosition = pos;
        }
    }
}
