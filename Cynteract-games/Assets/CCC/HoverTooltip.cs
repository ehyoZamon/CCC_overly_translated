using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.CCC
{
    public class HoverTooltip : UiHover
    {
        [ReadOnly]
        public Tooltip tooltip;
        public Tooltip tooltipPrefab;
        public string text;
        protected override void OnEnter()
        {
            if (tooltip==null)
            {
                tooltip = Instantiate(tooltipPrefab,OverlayCanvas.instance.transform);
                tooltip.SetText(text);
            }
            else
            {
                tooltip.SetActive(true);
            }
        }
        public void ForceLeave()
        {
            Destroy(tooltip.gameObject);
        }
        protected override void OnLeave()
        {
            if (tooltip!=null)
            {
                tooltip.SetActive(false);
            }

        }
        protected override void OnStay()
        {
            if (tooltip != null)
            {
                tooltip.transform.position = Input.mousePosition;
                if (tooltip.transform.position.x>Screen.width/2)
                {
                    tooltip.rectTransform.pivot = new Vector2(1, 1);

                }
                else
                {
                    tooltip.rectTransform.pivot = new Vector2(0, 1);

                }
            }
        }
    }
}
