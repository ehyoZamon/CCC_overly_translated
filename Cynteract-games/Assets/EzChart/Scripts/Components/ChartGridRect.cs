using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if CHART_TMPRO
using TMPro;
#endif

namespace ChartUtil
{
    public class ChartGridRect : ChartGrid
    {
        [HideInInspector] public float zeroRatio = 0.0f;
        [HideInInspector] public float zeroValue = 0.0f;
        [HideInInspector] public float[] positiveSum = null;
        [HideInInspector] public float[] negativeSum = null;

        string yLabelFormat = "N0";
        Image highlight;
        Vector2 offsetMin = new Vector2();
#if CHART_TMPRO
        public TextMeshProUGUI xTitle, yTitle;
#else
        Text xTitle, yTitle;
#endif
        Image xAxis, yAxis;

        public override int GetItemIndex(Vector2 pos)
        {
            int index = 0;
            if (chartOptions.plotOptions.inverted) index = Mathf.FloorToInt(pos.y / unitWidth);
            else index = Mathf.FloorToInt(pos.x / unitWidth);
            index = Mathf.Clamp(index, 0, chartData.categories.Count - 1);
            if (chartOptions.plotOptions.reverseSeries ^ chartOptions.plotOptions.inverted) index = chartData.categories.Count - index - 1;
            return index;
        }

        public override void HighlightItem(int index)
        {
            if (chartOptions.plotOptions.inverted)
            {
                if (!chartOptions.plotOptions.reverseSeries) index = chartData.categories.Count - index - 1;
                highlight.transform.localPosition = new Vector2(chartSize.x * 0.5f, unitWidth * (index + 0.5f));
            }
            else
            {
                if (chartOptions.plotOptions.reverseSeries) index = chartData.categories.Count - index - 1;
                highlight.transform.localPosition = new Vector2(unitWidth * (index + 0.5f), chartSize.y * 0.5f);
            }
            highlight.gameObject.SetActive(true);
        }

        public override void UnhighlightItem(int index)
        {
            highlight.gameObject.SetActive(false);
        }

        public override void UpdateGrid()
        {
            if (chartOptions.yAxis.enableTitle)
            {
                yTitle = ChartHelper.CreateText("YTitle", transform, chartOptions.yAxis.titleOption, chartOptions.plotOptions.generalFont);
                yTitle.text = chartOptions.yAxis.title;
                float height = yTitle.fontSize * 1.2f;
                if (chartOptions.plotOptions.inverted) offsetMin.y = height;
                else offsetMin.x = height;
            }
            if (chartOptions.xAxis.enableTitle)
            {
                xTitle = ChartHelper.CreateText("XTitle", transform, chartOptions.xAxis.titleOption, chartOptions.plotOptions.generalFont);
                xTitle.text = chartOptions.xAxis.title;
                float height = xTitle.fontSize * 1.2f;
                if (chartOptions.plotOptions.inverted) offsetMin.x = height;
                else offsetMin.y = height;
            }

            gridRect = ChartHelper.CreateEmptyRect("GridRect", transform, true);
            gridRect.pivot = Vector2.zero;
            labelRect = ChartHelper.CreateEmptyRect("GridLabelRect", transform, true);
            labelRect.pivot = Vector2.zero;

            Image background = ChartHelper.CreateImage("Background", gridRect, chartOptions.plotOptions.mouseTracking, true);
            background.color = chartOptions.plotOptions.backgroundColor;

            float minValue = 0.0f, maxValue = 0.0f;
            positiveSum = new float[chartData.categories.Count];
            negativeSum = new float[chartData.categories.Count];
            for (int i = 0; i < chartData.categories.Count; ++i)
            {
                float pSum, nSum;
                ChartHelper.GetSumByCategory(chartData, i, out pSum, out nSum);
                if (pSum > maxValue) maxValue = pSum;
                if (nSum < minValue) minValue = nSum;
                positiveSum[i] = pSum;
                negativeSum[i] = -nSum;
            }

            switch (chartOptions.plotOptions.columnStacking)
            {
                case ColumnStacking.None:
                    if (chartOptions.yAxis.fixedRange)
                    {
                        rangeMin = chartOptions.yAxis.fixedMinRange;
                        rangeMax = chartOptions.yAxis.fixedMaxRange;
                        numberOfSteps = chartOptions.yAxis.fixedRangeDivision;
                        stepSize = (rangeMax - rangeMin) / numberOfSteps;
                    }
                    else
                    {
                        ChartHelper.FindMinMaxValue(chartData, out minValue, out maxValue);
                        ChartHelper.FindRange(chartOptions.yAxis.startFromZero, chartOptions.yAxis.minRangeDivision, minValue, maxValue, out rangeMin, out rangeMax, out stepSize, out numberOfSteps);
                    }
                    break;
                case ColumnStacking.Normal:
                    ChartHelper.FindRange(chartOptions.yAxis.startFromZero, chartOptions.yAxis.minRangeDivision, minValue, maxValue, out rangeMin, out rangeMax, out stepSize, out numberOfSteps);
                    break;
                case ColumnStacking.Percent:
                    rangeMin = minValue < 0.0f ? -1.0f : 0.0f;
                    rangeMax = maxValue > 0.0f ? 1.0f : 0.0f;
                    numberOfSteps = rangeMin < 0.0f && rangeMax > 0.0f ? 10 : 5;
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

            if (stepSize >= 1.0f) yLabelFormat = "N0";
            else yLabelFormat = "N" + ChartHelper.FindFloatDisplayPrecision(stepSize).ToString();
            zeroValue = 0.0f;
            if (rangeMin >= 0.0f) zeroValue = rangeMin;
            else if (rangeMax < 0.0f) zeroValue = rangeMax - stepSize;
            zeroRatio = (zeroValue - rangeMin) / (rangeMax - rangeMin);

            UpdateYAxis();
            UpdateXAxis();

            if (xAxis != null) xAxis.transform.SetAsFirstSibling();
            if (yAxis != null) yAxis.transform.SetAsLastSibling();

            GetComponent<RectTransform>().offsetMin += offsetMin;
            chartSize -= offsetMin;

            if (yTitle != null) UpdateYAxisTitle();
            if (xTitle != null) UpdateXAxisTitle();

            highlight = ChartHelper.CreateImage("Highlight", gridRect);
            highlight.color = chartOptions.plotOptions.itemHighlightColor;
            highlight.rectTransform.sizeDelta = chartOptions.plotOptions.inverted ? new Vector2(chartSize.x, unitWidth) : new Vector2(unitWidth, chartSize.y);
            highlight.gameObject.SetActive(false);
        }

        void UpdateYAxisTitle()
        {
            float height = yTitle.fontSize * 1.2f;
            float width = yTitle.preferredWidth;
            if (chartOptions.plotOptions.inverted)
            {
                yTitle.rectTransform.anchorMin = new Vector2(0.5f, 0.0f);
                yTitle.rectTransform.anchorMax = new Vector2(0.5f, 0.0f);
                yTitle.rectTransform.sizeDelta = new Vector2(width, height);
                yTitle.rectTransform.anchoredPosition = new Vector2(0.0f, -offsetMin.y + height * 0.5f);
            }
            else
            {
                yTitle.rectTransform.anchorMin = new Vector2(0.0f, 0.5f);
                yTitle.rectTransform.anchorMax = new Vector2(0.0f, 0.5f);
                yTitle.rectTransform.sizeDelta = new Vector2(width, height);
                yTitle.rectTransform.anchoredPosition = new Vector2(-offsetMin.x + height * 0.5f, 0.0f);
                yTitle.rectTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
            }
        }

        void UpdateXAxisTitle()
        {
            float height = xTitle.fontSize * 1.2f;
            float width = xTitle.preferredWidth;
            if (chartOptions.plotOptions.inverted)
            {
                xTitle.rectTransform.anchorMin = new Vector2(0.0f, 0.5f);
                xTitle.rectTransform.anchorMax = new Vector2(0.0f, 0.5f);
                xTitle.rectTransform.sizeDelta = new Vector2(width, height);
                xTitle.rectTransform.anchoredPosition = new Vector2(-offsetMin.x + height * 0.5f, 0.0f);
                xTitle.rectTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, 90.0f);
            }
            else
            {
                xTitle.rectTransform.anchorMin = new Vector2(0.5f, 0.0f);
                xTitle.rectTransform.anchorMax = new Vector2(0.5f, 0.0f);
                xTitle.rectTransform.sizeDelta = new Vector2(width, height);
                xTitle.rectTransform.anchoredPosition = new Vector2(0.0f, -offsetMin.y + height * 0.5f);
            }
        }

        void UpdateYAxis()
        {
            //template
#if CHART_TMPRO
            TextMeshProUGUI labelTemp;
#else
            Text labelTemp;
#endif
            labelTemp = ChartHelper.CreateText("YGridLabel", transform, chartOptions.yAxis.labelOption, chartOptions.plotOptions.generalFont);
            labelTemp.rectTransform.sizeDelta = Vector2.zero;

            //grid
            if (chartOptions.xAxis.enableGridLine)
            {
                ChartGridGraphicRect xGrid = ChartHelper.CreateEmptyRect("XGrid", gridRect, true).gameObject.AddComponent<ChartGridGraphicRect>();
                xGrid.color = chartOptions.xAxis.gridLineColor;
                xGrid.width = chartOptions.xAxis.gridLineWidth;
                xGrid.num = numberOfSteps;
                xGrid.inverted = !chartOptions.plotOptions.inverted;
            }

            float spacing = 1.0f / numberOfSteps;
            if (chartOptions.plotOptions.inverted)
            {
                float height = chartOptions.yAxis.tickSize.y + chartOptions.yAxis.axisLineWidth * 0.5f;

                //tick
                if (chartOptions.yAxis.enableTick)
                {
                    ChartGridGraphicRect yTicks = ChartHelper.CreateEmptyRect("YTicks", gridRect).gameObject.AddComponent<ChartGridGraphicRect>();
                    yTicks.color = chartOptions.yAxis.tickColor;
                    yTicks.width = chartOptions.yAxis.tickSize.x;
                    yTicks.num = numberOfSteps;
                    yTicks.inverted = !chartOptions.plotOptions.inverted;

                    yTicks.rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
                    yTicks.rectTransform.anchorMax = new Vector2(1.0f, 0.0f);
                    yTicks.rectTransform.anchoredPosition = new Vector2(0.0f, -chartOptions.yAxis.tickSize.y * 0.5f - chartOptions.yAxis.axisLineWidth * 0.5f);
                    yTicks.rectTransform.sizeDelta = new Vector2(0.0f, chartOptions.yAxis.tickSize.y);
                }

                //axis
                if (chartOptions.yAxis.enableAxisLine)
                {
                    yAxis = ChartHelper.CreateImage("YAxis", gridRect);
                    yAxis.color = chartOptions.yAxis.axisLineColor;
                    yAxis.rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
                    yAxis.rectTransform.anchorMax = new Vector2(1.0f, 0.0f);
                    yAxis.rectTransform.sizeDelta = new Vector2(0.0f, chartOptions.yAxis.axisLineWidth);
                    yAxis.rectTransform.offsetMin -= new Vector2(chartOptions.xAxis.tickSize.y + chartOptions.yAxis.axisLineWidth * 0.5f, 0.0f);
                    yAxis.rectTransform.offsetMax += new Vector2(
                        chartOptions.yAxis.tickSize.x > chartOptions.xAxis.gridLineWidth ?
                        chartOptions.yAxis.tickSize.x * 0.5f : chartOptions.xAxis.gridLineWidth * 0.5f, 0.0f);
                }

                //label
                if (chartOptions.yAxis.enableLabel)
                {
                    labelTemp.rectTransform.anchoredPosition = new Vector2(0.0f, -chartOptions.yAxis.tickSize.y - chartOptions.yAxis.axisLineWidth * 0.5f - labelTemp.fontSize * 0.1f);
                    labelTemp.alignment = ChartHelper.ConvertAlignment(TextAnchor.UpperCenter);

                    for (int i = 0; i < numberOfSteps + 1; ++i)
                    {
                        var label = Instantiate(labelTemp, labelRect);
                        label.rectTransform.anchorMin = new Vector2(spacing * i, 0.0f);
                        label.rectTransform.anchorMax = new Vector2(spacing * i, 0.0f);
                        float value = rangeMin + stepSize * i;
                        if (chartOptions.yAxis.absoluteValue) value = Mathf.Abs(value);
                        if (chartOptions.plotOptions.columnStacking == ColumnStacking.Percent) label.text = (value * 100).ToString("f0") + "%";
                        else label.text = chartOptions.yAxis.labelFormat.Replace("{value}", value.ToString(yLabelFormat));
                    }

                    height += labelTemp.fontSize * 1.2f;
                }

                offsetMin.y += height;
                offsetMin.y = Mathf.Clamp(offsetMin.y, 0.0f, chartSize.y * 0.5f);
            }
            else
            {
                float width = chartOptions.yAxis.tickSize.y + chartOptions.yAxis.axisLineWidth * 0.5f;

                //tick
                if (chartOptions.yAxis.enableTick)
                {
                    ChartGridGraphicRect yTicks = ChartHelper.CreateEmptyRect("YTicks", gridRect).gameObject.AddComponent<ChartGridGraphicRect>();
                    yTicks.color = chartOptions.yAxis.tickColor;
                    yTicks.width = chartOptions.yAxis.tickSize.x;
                    yTicks.num = numberOfSteps;
                    yTicks.inverted = !chartOptions.plotOptions.inverted;

                    yTicks.rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
                    yTicks.rectTransform.anchorMax = new Vector2(0.0f, 1.0f);
                    yTicks.rectTransform.anchoredPosition = new Vector2(-chartOptions.yAxis.tickSize.y * 0.5f - chartOptions.yAxis.axisLineWidth * 0.5f, 0.0f);
                    yTicks.rectTransform.sizeDelta = new Vector2(chartOptions.yAxis.tickSize.y, 0.0f);
                }

                //axis
                if (chartOptions.yAxis.enableAxisLine)
                {
                    yAxis = ChartHelper.CreateImage("YAxis", gridRect);
                    yAxis.color = chartOptions.yAxis.axisLineColor;
                    yAxis.rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
                    yAxis.rectTransform.anchorMax = new Vector2(0.0f, 1.0f);
                    yAxis.rectTransform.sizeDelta = new Vector2(chartOptions.yAxis.axisLineWidth, 0.0f);
                    yAxis.rectTransform.offsetMin -= new Vector2(0.0f, chartOptions.xAxis.tickSize.y + chartOptions.yAxis.axisLineWidth * 0.5f);
                    yAxis.rectTransform.offsetMax += new Vector2(0.0f,
                        chartOptions.yAxis.tickSize.x > chartOptions.xAxis.gridLineWidth ?
                        chartOptions.yAxis.tickSize.x * 0.5f : chartOptions.xAxis.gridLineWidth * 0.5f);
                }

                //label
                if (chartOptions.yAxis.enableLabel)
                {
                    labelTemp.rectTransform.anchoredPosition = new Vector2(-chartOptions.yAxis.tickSize.y - chartOptions.yAxis.axisLineWidth * 0.5f - labelTemp.fontSize * 0.5f, 0.0f);
                    labelTemp.alignment = ChartHelper.ConvertAlignment(TextAnchor.MiddleRight);

                    for (int i = 0; i < numberOfSteps + 1; ++i)
                    {
                        var label = Instantiate(labelTemp, labelRect);
                        label.rectTransform.anchorMin = new Vector2(0.0f, spacing * i);
                        label.rectTransform.anchorMax = new Vector2(0.0f, spacing * i);
                        float value = rangeMin + stepSize * i;
                        if (chartOptions.yAxis.absoluteValue) value = Mathf.Abs(value);
                        if (chartOptions.plotOptions.columnStacking == ColumnStacking.Percent) label.text = (value * 100).ToString("f0") + "%";
                        else label.text = chartOptions.yAxis.labelFormat.Replace("{value}", value.ToString(yLabelFormat));
                    }

                    var firstLabel = ChartHelper.GetTextComponent(labelRect.GetChild(0).gameObject);
                    var lastLabel = ChartHelper.GetTextComponent(labelRect.GetChild(labelRect.childCount - 1).gameObject);
                    var temp = firstLabel.text.Length > lastLabel.text.Length ? firstLabel : lastLabel;

                    width += temp.preferredWidth + labelTemp.fontSize;
                }

                offsetMin.x += width;
                offsetMin.x = Mathf.Clamp(offsetMin.x, 0.0f, chartSize.x * 0.5f);
            }

            ChartHelper.Destroy(labelTemp.gameObject);
        }

        void UpdateXAxis()
        {
            //template
#if CHART_TMPRO
            TextMeshProUGUI labelTemp;
#else
            Text labelTemp;
#endif
            labelTemp = ChartHelper.CreateText("XGridLabel", transform, chartOptions.xAxis.labelOption, chartOptions.plotOptions.generalFont);
            labelTemp.rectTransform.sizeDelta = Vector2.zero;

            //grid
            ChartGridGraphicRect yGrid = null;
            if (chartOptions.yAxis.enableGridLine)
            {
                yGrid = ChartHelper.CreateEmptyRect("YGrid", gridRect, true).gameObject.AddComponent<ChartGridGraphicRect>();
                yGrid.color = chartOptions.yAxis.gridLineColor;
                yGrid.width = chartOptions.yAxis.gridLineWidth;
                yGrid.num = chartData.categories.Count;
                yGrid.mid = midGrid;
                yGrid.inverted = chartOptions.plotOptions.inverted;
            }

            float spacing = 1.0f / chartData.categories.Count;
            float maxWidth = 0.0f;
            if (chartOptions.xAxis.enableLabel)
            {
                string maxStr = "";
                for (int i = 0; i < chartData.categories.Count; ++i)
                {
                    if (chartData.categories[i].Length > maxStr.Length) maxStr = chartData.categories[i];
                }
                var label = Instantiate(labelTemp, transform);
                label.text = maxStr;
                maxWidth = label.preferredWidth;
                ChartHelper.Destroy(label.gameObject);
            }

            if (chartOptions.plotOptions.inverted)
            {
                float width = chartOptions.xAxis.tickSize.y + chartOptions.xAxis.axisLineWidth * 0.5f;
                unitWidth = (chartSize.y - offsetMin.y) / chartData.categories.Count;

                //tick
                ChartGridGraphicRect xTicks = null;
                if (chartOptions.xAxis.enableTick)
                {
                    xTicks = ChartHelper.CreateEmptyRect("XTicks", gridRect).gameObject.AddComponent<ChartGridGraphicRect>();
                    xTicks.color = chartOptions.xAxis.tickColor;
                    xTicks.width = chartOptions.xAxis.tickSize.x;
                    xTicks.num = chartData.categories.Count;
                    xTicks.mid = midGrid;
                    xTicks.inverted = chartOptions.plotOptions.inverted;

                    xTicks.rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
                    xTicks.rectTransform.anchorMax = new Vector2(0.0f, 1.0f);
                    xTicks.rectTransform.anchoredPosition = new Vector2(-chartOptions.xAxis.tickSize.y * 0.5f - chartOptions.xAxis.axisLineWidth * 0.5f, 0.0f);
                    xTicks.rectTransform.sizeDelta = new Vector2(chartOptions.xAxis.tickSize.y, 0.0f);
                }

                //axis
                if (chartOptions.xAxis.enableAxisLine)
                {
                    float pos = chartOptions.xAxis.autoAxisLinePosition ? zeroRatio : 0.0f;
                    xAxis = ChartHelper.CreateImage("XAxis", labelRect);
                    xAxis.rectTransform.anchorMin = new Vector2(pos, 0.0f);
                    xAxis.rectTransform.anchorMax = new Vector2(pos, 1.0f);
                    xAxis.gameObject.name = "XAxis";
                    xAxis.color = chartOptions.xAxis.axisLineColor;
                    xAxis.rectTransform.sizeDelta = new Vector2(chartOptions.xAxis.axisLineWidth, 0.0f);
                    xAxis.rectTransform.offsetMin -= new Vector2(0.0f, chartOptions.yAxis.tickSize.y + chartOptions.yAxis.axisLineWidth * 0.5f);
                    xAxis.rectTransform.offsetMax += new Vector2(0.0f,
                        chartOptions.xAxis.tickSize.x > chartOptions.yAxis.gridLineWidth ?
                        chartOptions.xAxis.tickSize.x * 0.5f : chartOptions.yAxis.gridLineWidth * 0.5f);
                }

                //label
                if (chartOptions.xAxis.enableLabel)
                {
                    labelTemp.rectTransform.anchoredPosition = new Vector2(-chartOptions.xAxis.tickSize.y - chartOptions.yAxis.axisLineWidth * 0.5f - labelTemp.fontSize * 0.5f, 0.0f);
                    labelTemp.alignment = ChartHelper.ConvertAlignment(TextAnchor.MiddleRight);

                    width += maxWidth + labelTemp.fontSize;
                    width = Mathf.Clamp(width, 0.0f, chartSize.x * 0.5f - offsetMin.x);
                    maxWidth = width;

                    int skip = chartOptions.xAxis.skipLabel;
                    if (skip < 0)
                    {
                        float minWidth = chartOptions.xAxis.maxLabels > 0 ? (chartSize.y - offsetMin.y) / chartOptions.xAxis.maxLabels : 0.0f;
                        minWidth -= 0.1f;
                        float skipWidth = labelTemp.fontSize * 1.25f > minWidth ? labelTemp.fontSize * 1.25f : minWidth;
                        skip = Mathf.FloorToInt(skipWidth / unitWidth);
                    }
                    if (xTicks != null) xTicks.skip = skip;
                    if (yGrid != null) yGrid.skip = skip;

                    for (int i = 0; i < chartData.categories.Count; i += skip + 1)
                    {
                        int posIndex = !chartOptions.plotOptions.reverseSeries ? chartData.categories.Count - i - 1 : i;
                        var label = Instantiate(labelTemp, labelRect);
                        label.text = chartData.categories[i];
                        label.rectTransform.anchorMin = new Vector2(0.0f, spacing * (posIndex + 0.5f));
                        label.rectTransform.anchorMax = new Vector2(0.0f, spacing * (posIndex + 0.5f));
                        if (label.preferredWidth > maxWidth) ChartHelper.TruncateText(label, maxWidth);
                    }
                }

                offsetMin.x += width;
                offsetMin.x = Mathf.Clamp(offsetMin.x, 0.0f, chartSize.x * 0.5f);
            }
            else
            {
                float height = chartOptions.xAxis.tickSize.y + chartOptions.xAxis.axisLineWidth * 0.5f;
                unitWidth = (chartSize.x - offsetMin.x) / chartData.categories.Count;

                //tick
                ChartGridGraphicRect xTicks = null;
                if (chartOptions.xAxis.enableTick)
                {
                    xTicks = ChartHelper.CreateEmptyRect("XTicks", gridRect).gameObject.AddComponent<ChartGridGraphicRect>();
                    xTicks.color = chartOptions.xAxis.tickColor;
                    xTicks.width = chartOptions.xAxis.tickSize.x;
                    xTicks.num = chartData.categories.Count;
                    xTicks.mid = midGrid;
                    xTicks.inverted = chartOptions.plotOptions.inverted;

                    xTicks.rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
                    xTicks.rectTransform.anchorMax = new Vector2(1.0f, 0.0f);
                    xTicks.rectTransform.anchoredPosition = new Vector2(0.0f, -chartOptions.xAxis.tickSize.y * 0.5f - chartOptions.xAxis.axisLineWidth * 0.5f);
                    xTicks.rectTransform.sizeDelta = new Vector2(0.0f, chartOptions.xAxis.tickSize.y);
                }

                //axis
                if (chartOptions.xAxis.enableAxisLine)
                {
                    float pos = chartOptions.xAxis.autoAxisLinePosition ? zeroRatio : 0.0f;
                    xAxis = ChartHelper.CreateImage("XAxis", labelRect);
                    xAxis.rectTransform.anchorMin = new Vector2(0.0f, pos);
                    xAxis.rectTransform.anchorMax = new Vector2(1.0f, pos);
                    xAxis.gameObject.name = "XAxis";
                    xAxis.color = chartOptions.xAxis.axisLineColor;
                    xAxis.rectTransform.sizeDelta = new Vector2(0.0f, chartOptions.xAxis.axisLineWidth);
                    xAxis.rectTransform.offsetMin -= new Vector2(chartOptions.yAxis.tickSize.y + chartOptions.yAxis.axisLineWidth * 0.5f, 0.0f);
                    xAxis.rectTransform.offsetMax += new Vector2(
                        chartOptions.xAxis.tickSize.x > chartOptions.yAxis.gridLineWidth ?
                        chartOptions.xAxis.tickSize.x * 0.5f : chartOptions.yAxis.gridLineWidth * 0.5f, 0.0f);
                }

                //label
                if (chartOptions.xAxis.enableLabel)
                {
                    bool useLongLabel = maxWidth > unitWidth * 0.8f && chartOptions.xAxis.autoRotateLabel;

                    labelTemp.rectTransform.anchoredPosition = new Vector2(0.0f, -chartOptions.xAxis.tickSize.y - chartOptions.yAxis.axisLineWidth * 0.5f - labelTemp.fontSize * 0.1f);
                    if (useLongLabel)
                    {
                        labelTemp.alignment = ChartHelper.ConvertAlignment(TextAnchor.MiddleRight);
                        labelTemp.rectTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, 45.0f);
                    }
                    else
                    {
                        labelTemp.alignment = ChartHelper.ConvertAlignment(TextAnchor.UpperCenter);
                    }

                    height += useLongLabel ? maxWidth * 0.8f : labelTemp.fontSize * 1.2f;
                    height = Mathf.Clamp(height, 0.0f, chartSize.y * 0.5f - offsetMin.y);
                    if (useLongLabel) maxWidth = height * 1.414f;
                    else maxWidth = chartOptions.xAxis.skipLabel < 0 ? unitWidth * 0.9f : unitWidth * (chartOptions.xAxis.skipLabel + 1) * 0.9f;

                    int skip = chartOptions.xAxis.skipLabel;
                    if (skip < 0)
                    {
                        float minWidth = chartOptions.xAxis.maxLabels > 0 ? (chartSize.x - offsetMin.x) / chartOptions.xAxis.maxLabels : 0.0f;
                        minWidth -= 0.1f;
                        float skipWidth = labelTemp.fontSize * 1.25f > minWidth ? labelTemp.fontSize * 1.25f : minWidth;
                        skip = Mathf.FloorToInt(skipWidth / unitWidth);
                    }
                    if (xTicks != null) xTicks.skip = skip;
                    if (yGrid != null) yGrid.skip = skip;

                    for (int i = 0; i < chartData.categories.Count; i += skip + 1)
                    {
                        int posIndex = chartOptions.plotOptions.reverseSeries ? chartData.categories.Count - i - 1 : i;
                        var label = Instantiate(labelTemp, labelRect);
                        label.text = chartData.categories[i];
                        label.rectTransform.anchorMin = new Vector2(spacing * (posIndex + 0.5f), 0.0f);
                        label.rectTransform.anchorMax = new Vector2(spacing * (posIndex + 0.5f), 0.0f);
                        if (label.preferredWidth > maxWidth) ChartHelper.TruncateText(label, maxWidth);
                    }
                }

                offsetMin.y += height;
                offsetMin.y = Mathf.Clamp(offsetMin.y, 0.0f, chartSize.y * 0.5f);
            }
            ChartHelper.Destroy(labelTemp.gameObject);
        }
    }
}