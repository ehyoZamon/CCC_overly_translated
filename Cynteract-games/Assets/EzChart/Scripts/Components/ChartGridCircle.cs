using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if CHART_TMPRO
using TMPro;
#endif

namespace ChartUtil
{
    public class ChartGridCircle : ChartGrid
    {
        [HideInInspector] public bool circularGrid = true;
        [HideInInspector] public float innerSize = 0.0f;
        [HideInInspector] public float outerSize = 1.0f;
        [HideInInspector] public float mouseAngle;
        [HideInInspector] public List<float> positiveSum = new List<float>();

        string format = "N0";
        Image highlight;
        Image background;
        MaskableGraphic innerRing, outerRing;

        public override int GetItemIndex(Vector2 pos)
        {
            if (pos.sqrMagnitude > 0.25f * chartSize.x * chartSize.x || pos.sqrMagnitude < 0.25f * chartSize.y * chartSize.y) return -1;
            mouseAngle = Vector3.Angle(new Vector2(0.0f, 1.0f), pos);
            if (pos.x < 0.0f) mouseAngle = 360.0f - mouseAngle;
            int index = Mathf.FloorToInt(mouseAngle / unitWidth) % chartData.categories.Count;
            if (chartOptions.plotOptions.reverseSeries) index = chartData.categories.Count - index - 1;
            return index;
        }

        public override void HighlightItem(int index)
        {
            if (chartOptions.plotOptions.reverseSeries) index = chartData.categories.Count - index - 1;
            highlight.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -unitWidth * index);
            highlight.gameObject.SetActive(true);
        }

        public override void UnhighlightItem(int index)
        {
            highlight.gameObject.SetActive(false);
        }

        public override void UpdateGrid()
        {
            float minValue = 0.0f, maxValue = 0.0f;
            for (int i = 0; i < chartData.categories.Count; ++i)
            {
                float pSum = ChartHelper.GetPositiveSumByCategory(chartData, i);
                if (pSum > maxValue) maxValue = pSum;
                positiveSum.Add(pSum);
            }

            switch (chartOptions.plotOptions.columnStacking)
            {
                case ColumnStacking.None:
                    if (chartOptions.yAxis.fixedRange)
                    {
                        rangeMin = chartOptions.yAxis.fixedMinRange > 0.0f ? chartOptions.yAxis.fixedMinRange : 0.0f;
                        rangeMax = chartOptions.yAxis.fixedMaxRange;
                        numberOfSteps = chartOptions.yAxis.fixedRangeDivision;
                        stepSize = (rangeMax - rangeMin) / numberOfSteps;
                    }
                    else
                    {
                        ChartHelper.FindMinMaxValue(chartData, out minValue, out maxValue);
                        if (minValue < 0.0f) minValue = 0.0f;
                        ChartHelper.FindRange(chartOptions.yAxis.startFromZero, chartOptions.yAxis.minRangeDivision, minValue, maxValue, out rangeMin, out rangeMax, out stepSize, out numberOfSteps);
                    }
                    break;
                case ColumnStacking.Normal:
                    ChartHelper.FindRange(chartOptions.yAxis.startFromZero, chartOptions.yAxis.minRangeDivision, minValue, maxValue, out rangeMin, out rangeMax, out stepSize, out numberOfSteps);
                    break;
                case ColumnStacking.Percent:
                    rangeMin = 0.0f;
                    rangeMax = 1.0f;
                    numberOfSteps = 5;
                    stepSize = 0.2f;
                    break;
                default:
                    break;
            }
            if (numberOfSteps <= 0)
            {
                numberOfSteps = 1;
                stepSize = 1.0f;
                rangeMin = 0.0f;
                rangeMax = 1.0f;
            }
            range = rangeMax - rangeMin;

            if (stepSize >= 1.0f) format = "N0";
            else format = "N" + ChartHelper.FindFloatDisplayPrecision(stepSize).ToString();

            unitWidth = 360.0f / chartData.categories.Count;
            chartSize.x = (chartSize.x < chartSize.y ? chartSize.x : chartSize.y) * Mathf.Clamp01(outerSize);
            chartSize.x -= chartOptions.xAxis.axisLineWidth + chartOptions.xAxis.tickSize.y * 2.0f;
            if (chartOptions.xAxis.enableLabel)
                chartSize.x -= chartOptions.xAxis.labelOption.customizedText == null ? 
                    chartOptions.xAxis.labelOption.fontSize * 2 : chartOptions.xAxis.labelOption.customizedText.fontSize * 2.0f;
            chartSize.y = chartSize.x * Mathf.Clamp(innerSize, 0.0f, outerSize);

            gridRect = ChartHelper.CreateEmptyRect("GridRect", transform, true);
            labelRect = ChartHelper.CreateEmptyRect("GridLabelRect", transform, true);

            background = ChartHelper.CreateImage("Background", gridRect, chartOptions.plotOptions.mouseTracking);
            background.sprite = Resources.Load<Sprite>("Images/Chart_Circle_512x512");
            background.color = chartOptions.plotOptions.backgroundColor;
            float bSize = chartSize.x + chartOptions.xAxis.tickSize.y * 2.0f + chartOptions.xAxis.axisLineWidth;
            background.rectTransform.sizeDelta = new Vector2(bSize, bSize);
            
            UpdateYAxis();
            UpdateXAxis();

            if (innerRing != null) innerRing.transform.SetSiblingIndex(1);
            if (outerRing != null) outerRing.transform.SetSiblingIndex(1);

            highlight = ChartHelper.CreateImage("Highlight", gridRect);
            highlight.color = chartOptions.plotOptions.itemHighlightColor;
            highlight.sprite = Resources.Load<Sprite>("Images/Chart_Circle_512x512");
            highlight.type = Image.Type.Filled;
            highlight.fillMethod = Image.FillMethod.Radial360;
            highlight.fillOrigin = (int)Image.Origin360.Top;
            highlight.fillAmount = Mathf.Clamp01(1.0f / chartData.categories.Count);
            highlight.rectTransform.sizeDelta = new Vector2(chartSize.x, chartSize.x);
            highlight.gameObject.SetActive(false);
        }
        
        void UpdateYAxis()
        {
            //inner ring
            if (chartOptions.yAxis.enableAxisLine)
            {
                float gridSize = chartSize.y + chartOptions.yAxis.axisLineWidth;
                float smoothness = Mathf.Clamp01(3.0f / gridSize);
                float gridWidth = Mathf.Clamp01(1.0f - chartOptions.yAxis.axisLineWidth * 2.0f / gridSize - smoothness);
                innerRing = ChartHelper.CreateImage("InnerRing", labelRect);
                innerRing.gameObject.AddComponent<ChartMaterialHandler>().Load("Materials/Chart_Ring");
                innerRing.rectTransform.sizeDelta = new Vector2(gridSize, gridSize);
                innerRing.color = chartOptions.yAxis.axisLineColor;
                innerRing.material.SetFloat("_InnerRadius", gridWidth);
                innerRing.material.SetFloat("_Smoothness", smoothness);
            }

            //tick
            float circularGridRatio = circularGrid ? 1.0f : Mathf.Sin((90.0f - 360.0f / chartData.categories.Count * 0.5f) * Mathf.Deg2Rad);
            if (chartOptions.yAxis.enableTick)
            {
                ChartGridGraphicRect yTicks = ChartHelper.CreateEmptyRect("YTicks", gridRect).gameObject.AddComponent<ChartGridGraphicRect>();
                yTicks.color = chartOptions.yAxis.tickColor;
                yTicks.width = chartOptions.yAxis.tickSize.y;
                yTicks.num = numberOfSteps;
                yTicks.inverted = true;
                yTicks.rectTransform.sizeDelta = new Vector2(chartOptions.yAxis.tickSize.x, (chartSize.x - chartSize.y) * 0.5f * circularGridRatio);
                yTicks.rectTransform.anchoredPosition = new Vector2(0.0f,
                    yTicks.rectTransform.sizeDelta.y * 0.5f + chartOptions.xAxis.gridLineWidth * 0.5f + chartOptions.yAxis.tickSize.y * 0.5f);
            }

            //grid
            if (chartOptions.xAxis.enableGridLine)
            {
                float smoothness = Mathf.Clamp01(3.0f / chartOptions.xAxis.gridLineWidth);
                float gridWidth = chartOptions.xAxis.gridLineWidth * (1 + smoothness);
                ChartGridGraphicCircle xGrid = ChartHelper.CreateEmptyRect("XGrid", gridRect).gameObject.AddComponent<ChartGridGraphicCircle>();
                xGrid.gameObject.AddComponent<ChartMaterialHandler>().Load("Materials/Chart_VBlur");
                xGrid.rectTransform.sizeDelta = new Vector2(chartSize.x, chartSize.x);
                xGrid.color = chartOptions.xAxis.gridLineColor;
                xGrid.width = gridWidth;
                xGrid.num = numberOfSteps;
                xGrid.mid = midGrid;
                xGrid.side = chartData.categories.Count;
                xGrid.innerSize = innerSize;
                xGrid.isCircular = circularGrid;
                xGrid.material.SetFloat("_Smoothness", smoothness);
            }

            //label
            if (chartOptions.yAxis.enableLabel)
            {
#if CHART_TMPRO
                TextMeshProUGUI labelTemp;
#else
                Text labelTemp;
#endif
                labelTemp = ChartHelper.CreateText("YGridLabel", transform, chartOptions.yAxis.labelOption, chartOptions.plotOptions.generalFont, TextAnchor.LowerCenter);
                labelTemp.rectTransform.sizeDelta = Vector2.zero;

                float spacing = (chartSize.x - chartSize.y) * circularGridRatio * 0.5f / numberOfSteps;
                float offset = chartSize.y * 0.5f + chartOptions.xAxis.gridLineWidth * 0.5f;
                for (int i = 0; i <= numberOfSteps; ++i)
                {
                    float h = offset + spacing * i;
                    var label = Instantiate(labelTemp, labelRect);
                    label.text = (rangeMin + stepSize * i).ToString(format);
                    label.transform.localPosition = new Vector2(0.0f, h);
                }
                ChartHelper.Destroy(labelTemp.gameObject);
            }
        }

        void UpdateXAxis()
        {
            //grid
            if (chartOptions.yAxis.enableGridLine)
            {
                float smoothness = Mathf.Clamp01(3.0f / chartOptions.yAxis.gridLineWidth);
                float gridWidth = chartOptions.yAxis.gridLineWidth * (1 + smoothness);
                RectTransform tmp = midGrid ? gridRect : labelRect;
                ChartGridGraphicRadialLine yGrid = ChartHelper.CreateEmptyRect("YGrid", tmp).gameObject.AddComponent<ChartGridGraphicRadialLine>();
                if (!midGrid) yGrid.transform.SetAsFirstSibling();
                yGrid.gameObject.AddComponent<ChartMaterialHandler>().Load("Materials/Chart_UBlur");
                yGrid.rectTransform.sizeDelta = new Vector2(chartSize.x, chartSize.x);
                yGrid.color = chartOptions.yAxis.gridLineColor;
                yGrid.width = gridWidth;
                yGrid.innerSize = innerSize;
                yGrid.mid = midGrid;
                yGrid.side = chartData.categories.Count;
                yGrid.material.SetFloat("_Smoothness", smoothness);
            }

            //tick
            if (chartOptions.xAxis.enableTick)
            {
                float smoothness = Mathf.Clamp01(3.0f / chartOptions.xAxis.tickSize.x);
                float gridWidth = chartOptions.xAxis.tickSize.x * (1 + smoothness);
                ChartGridGraphicRadialLine xTicks = ChartHelper.CreateEmptyRect("XTicks", gridRect).gameObject.AddComponent<ChartGridGraphicRadialLine>();
                xTicks.gameObject.AddComponent<ChartMaterialHandler>().Load("Materials/Chart_UBlur");
                xTicks.rectTransform.sizeDelta = background.rectTransform.sizeDelta;
                xTicks.color = chartOptions.xAxis.tickColor;
                xTicks.width = gridWidth;
                xTicks.innerSize = chartSize.x / xTicks.rectTransform.sizeDelta.x;
                xTicks.mid = midGrid;
                xTicks.side = chartData.categories.Count;
                xTicks.material.SetFloat("_Smoothness", smoothness);
            }

            //outer ring
            if (chartOptions.xAxis.enableAxisLine)
            {
                float smoothness = Mathf.Clamp01(3.0f / chartOptions.xAxis.axisLineWidth);
                float gridWidth = chartOptions.xAxis.axisLineWidth * (1 + smoothness);
                ChartGridGraphicCircle oRing = ChartHelper.CreateEmptyRect("OuterRing", labelRect).gameObject.AddComponent<ChartGridGraphicCircle>();
                oRing.gameObject.AddComponent<ChartMaterialHandler>().Load("Materials/Chart_VBlur");
                oRing.rectTransform.sizeDelta = new Vector2(chartSize.x, chartSize.x);
                oRing.color = chartOptions.xAxis.axisLineColor;
                oRing.width = gridWidth;
                oRing.num = 1;
                oRing.drawLast = true;
                oRing.mid = midGrid;
                oRing.side = chartData.categories.Count;
                oRing.innerSize = innerSize;
                oRing.isCircular = circularGrid;
                oRing.material.SetFloat("_Smoothness", smoothness);
                outerRing = oRing;
            }

            //labels
            if (chartOptions.xAxis.enableLabel)
            {
#if CHART_TMPRO
                TextMeshProUGUI labelTemp;
#else
                Text labelTemp;
#endif
                labelTemp = ChartHelper.CreateText("XGridLabel", transform, chartOptions.xAxis.labelOption, chartOptions.plotOptions.generalFont);
                labelTemp.rectTransform.sizeDelta = Vector2.zero;

                float dist = chartSize.x * 0.5f + labelTemp.fontSize * 0.5f + chartOptions.xAxis.tickSize.y + chartOptions.xAxis.axisLineWidth * 0.5f;
                for (int i = 0; i < chartData.categories.Count; ++i)
                {
                    int posIndex = chartOptions.plotOptions.reverseSeries ? chartData.categories.Count - i - 1 : i;
                    var label = Instantiate(labelTemp, labelRect);
                    label.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -unitWidth * posIndex - unitWidth * 0.5f);
                    label.rectTransform.anchoredPosition = label.transform.up * dist;
                    label.text = chartData.categories[i];
                }
                ChartHelper.Destroy(labelTemp.gameObject);
            }
        }
    }
}