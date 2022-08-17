using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChartUtil
{
    public class ChartGrid : MonoBehaviour
    {
        [HideInInspector] public Vector2 chartSize; //size and inner size for circle chart grid
        [HideInInspector] public ChartOptions chartOptions;
        [HideInInspector] public ChartData chartData;
        [HideInInspector] public bool midGrid = false;

        [HideInInspector] public int numberOfSteps = 1;
        [HideInInspector] public float stepSize = 0.0f;
        [HideInInspector] public float range = 0.0f;
        [HideInInspector] public float rangeMin = 0.0f;
        [HideInInspector] public float rangeMax = 0.0f;
        [HideInInspector] public float unitWidth = 0.0f;

        [HideInInspector] public RectTransform gridRect;
        [HideInInspector] public RectTransform labelRect;

        public virtual void UpdateGrid() { }
        public virtual int GetItemIndex(Vector2 mousePosition) { return -1; }
        public virtual void HighlightItem(int index) { }
        public virtual void UnhighlightItem(int index) { }
    }
}