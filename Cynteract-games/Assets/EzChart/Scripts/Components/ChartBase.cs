using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ChartUtil
{
    public class ChartBase : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [HideInInspector] public Vector2 chartSize;
        [HideInInspector] public RectTransform chartRect;
        [HideInInspector] public ChartOptions chartOptions;
        [HideInInspector] public ChartData chartData;
        [HideInInspector] public ChartEvents chartEvents;
        [HideInInspector] public ChartTooltip tooltip;
        [HideInInspector] public List<int> skipSeries = new List<int>();

        bool mouseOver = false;
        protected int currCate = -1;
        protected int currSeries = -1;
        protected Vector2 localMousePosition;
        protected RectTransform dataRect;
        protected RectTransform labelRect;
        Camera eventCam;

        public virtual void UpdateChart() { }

        protected virtual int FindCategory() { return -1; }

        protected virtual int FindSeries() { return -1; }

        protected virtual void HighlightCurrentItem() { }

        protected virtual void UnhighlightCurrentItem() { }

        protected virtual void UpdateTooltip() { }

        void Update()
        {
            if (mouseOver) CheckTooltip();
            if (tooltip != null && tooltip.gameObject.activeSelf)
            {
                tooltip.transform.localPosition = localMousePosition + chartRect.rect.position + (Vector2)chartRect.localPosition;
                tooltip.ValidatePosition();
            }
        }

        void ShowTooltip()
        {
            if (tooltip == null) return;
            tooltip.gameObject.SetActive(true);
            UpdateTooltip();
        }

        void HideTooltip()
        {
            if (tooltip == null) return;
            tooltip.gameObject.SetActive(false);
        }

        void CheckTooltip()
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(chartRect, Input.mousePosition, eventCam, out localMousePosition)) return;
            localMousePosition -= chartRect.rect.position;

            int cate = FindCategory();
            if (chartOptions.tooltip.share)
            {
                if (cate != currCate)
                {
                    if (currCate >= 0) UnhighlightCurrentItem();
                    currCate = cate;
                    if (currCate >= 0)
                    {
                        HighlightCurrentItem();
                        ShowTooltip();
                    }
                    else
                    {
                        HideTooltip();
                    }
                }
            }
            else
            {
                if (cate != currCate)
                {
                    if (currCate >= 0) UnhighlightCurrentItem();
                    currCate = cate;
                    if (currCate >= 0) HighlightCurrentItem();
                }
                if (cate >= 0)
                {
                    int seri = FindSeries();
                    if (seri != currSeries)
                    {
                        currSeries = seri;
                        if (currSeries >= 0) ShowTooltip();
                        else HideTooltip();
                    }
                }
                else
                {
                    currSeries = -1;
                    HideTooltip();
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!chartOptions.plotOptions.mouseTracking) return;
            mouseOver = true;
            eventCam = eventData.enterEventCamera;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!chartOptions.plotOptions.mouseTracking) return;
            mouseOver = false;
            if (currCate >= 0)
            {
                UnhighlightCurrentItem();
                currCate = -1;
                HideTooltip();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (chartEvents == null) return;
            currCate = FindCategory();
            currSeries = FindSeries();
            chartEvents.itemClickEvent.Invoke(currCate, currSeries);
        }
    }
}