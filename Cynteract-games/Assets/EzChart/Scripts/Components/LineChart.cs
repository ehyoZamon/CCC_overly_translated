using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if CHART_TMPRO
using TMPro;
#endif

namespace ChartUtil
{
    [ExecuteInEditMode]
    public class LineChart : ChartBase
    {
        ChartGridRect chartGrid;
        LineChartPoint[] pointList;
        Material pointMat;
        Material lineMat;

        private void OnDestroy()
        {
            if (pointMat != null) ChartHelper.Destroy(pointMat);
            if (lineMat != null) ChartHelper.Destroy(lineMat);
        }

        public override void UpdateChart()
        {
            chartGrid = GetComponent<ChartGridRect>();
            if (chartGrid == null)
            {
                chartGrid = gameObject.AddComponent<ChartGridRect>();
                chartGrid.chartSize = chartSize;
                chartGrid.chartOptions = chartOptions;
                chartGrid.chartData = chartData;
                chartGrid.midGrid = true;
                chartGrid.UpdateGrid();
            }
            dataRect = ChartHelper.CreateEmptyRect("DataRect", transform, true);
            labelRect = ChartHelper.CreateEmptyRect("LabelRect", transform, true);
            dataRect.pivot = Vector2.zero;
            UpdateItems();
            chartGrid.labelRect.SetSiblingIndex(labelRect.GetSiblingIndex() - 1);
        }

        protected override int FindCategory()
        {
            return chartGrid.GetItemIndex(localMousePosition);
        }

        protected override int FindSeries()
        {
            int index = -1;
            float radius = chartOptions.plotOptions.lineChartOption.pointSize * 0.5f * 1.1f;
            float sqrRadius = radius * radius;
            if (chartOptions.plotOptions.inverted)
            {
                for (int i = 0; i < chartData.series.Count; ++i)
                {
                    if (!chartData.series[i].show) continue;
                    Vector2 pos = new Vector2((pointList[i].data[currCate].y + pointList[i].data[currCate].x) * chartGrid.chartSize.x, chartGrid.unitWidth * (currCate + 0.5f));
                    if (!chartOptions.plotOptions.reverseSeries) pos.y = chartGrid.chartSize.y - pos.y;
                    Vector2 dir = localMousePosition - pos;
                    if (dir.sqrMagnitude < sqrRadius) { index = i; break; }
                }
            }
            else
            {
                for (int i = 0; i < chartData.series.Count; ++i)
                {
                    if (!chartData.series[i].show) continue;
                    Vector2 pos = new Vector2(chartGrid.unitWidth * (currCate + 0.5f), (pointList[i].data[currCate].y + pointList[i].data[currCate].x) * chartGrid.chartSize.y);
                    if (chartOptions.plotOptions.reverseSeries) pos.x = chartGrid.chartSize.x - pos.x;
                    Vector2 dir = localMousePosition - pos;
                    if (dir.sqrMagnitude < sqrRadius) { index = i; break; }
                }
            }
            return index;
        }

        protected override void HighlightCurrentItem()
        {
            chartGrid.HighlightItem(currCate);
        }

        protected override void UnhighlightCurrentItem()
        {
            chartGrid.UnhighlightItem(currCate);
        }

        protected override void UpdateTooltip()
        {
            tooltip.tooltipText.text = GetFormattedHeaderText(currCate, chartOptions.tooltip.headerFormat);
            if (chartOptions.tooltip.share)
            {
                for (int i = 0; i < chartData.series.Count; ++i)
                {
                    if (!chartData.series[i].show || !IsValid(i, currCate)) continue;
                    tooltip.tooltipText.text += "\n" + GetFormattedPointText(i, currCate, chartOptions.tooltip.pointFormat, 0);
                }
            }
            else
            {
                if (chartData.series[currSeries].show || !IsValid(currSeries, currCate))
                    tooltip.tooltipText.text += "\n" + GetFormattedPointText(currSeries, currCate, chartOptions.tooltip.pointFormat, 0);
            }
            tooltip.background.rectTransform.sizeDelta = new Vector2(tooltip.tooltipText.preferredWidth + 16, tooltip.tooltipText.preferredHeight + 6);
        }

        public bool IsValid(int seriesIndex, int cateIndex)
        {
            return cateIndex < chartData.series[seriesIndex].data.Count && chartData.series[seriesIndex].data[cateIndex].show;
        }

        Vector2 GetDataRatio(int seriesIndex, int cateIndex, float[] stackValueList, float[] stackValueListNeg)
        {
            Vector2 value = new Vector2();
            if (!IsValid(seriesIndex, cateIndex)) return value;
            switch (chartOptions.plotOptions.columnStacking)
            {
                case ColumnStacking.None:
                    value.x = (chartData.series[seriesIndex].data[cateIndex].value - chartGrid.zeroValue) / chartGrid.range;
                    value.y = chartGrid.zeroRatio;
                    break;
                case ColumnStacking.Normal:
                    value.x = (chartData.series[seriesIndex].data[cateIndex].value - chartGrid.zeroValue) / chartGrid.range;
                    if (chartData.series[seriesIndex].data[cateIndex].value >= 0.0f)
                    {
                        value.y = chartGrid.zeroRatio + stackValueList[cateIndex];
                        stackValueList[cateIndex] += value.x;
                    }
                    else
                    {
                        value.y = chartGrid.zeroRatio + stackValueListNeg[cateIndex];
                        stackValueListNeg[cateIndex] += value.x;
                    }
                    break;
                case ColumnStacking.Percent:
                    if (chartData.series[seriesIndex].data[cateIndex].value >= 0.0f)
                    {
                        value.x = (chartData.series[seriesIndex].data[cateIndex].value / chartGrid.positiveSum[cateIndex] - chartGrid.zeroValue) / chartGrid.range;
                        value.y = chartGrid.zeroRatio + stackValueList[cateIndex];
                        stackValueList[cateIndex] += value.x;
                    }
                    else
                    {
                        value.x = (chartData.series[seriesIndex].data[cateIndex].value / chartGrid.negativeSum[cateIndex] - chartGrid.zeroValue) / chartGrid.range;
                        value.y = chartGrid.zeroRatio + stackValueListNeg[cateIndex];
                        stackValueListNeg[cateIndex] += value.x;
                    }
                    break;
                default:
                    break;
            }
            value.x = Mathf.Clamp(value.x, -1.0f, 1.0f);
            value.y = Mathf.Clamp(value.y, -1.0f, 1.0f);
            return value;
        }

        string GetFormattedHeaderText(int cateIndex, string format)
        {
            format = format.Replace("{category}", chartData.categories[cateIndex]);
            return format;
        }

        string GetFormattedPointText(int seriesIndex, int cateIndex, string format, int type)
        {
            float value = chartData.series[seriesIndex].data[cateIndex].value;
            string f;
            if (type == 0)
            {
                f = chartOptions.tooltip.pointNumericFormat;
                if (chartOptions.tooltip.absoluteValue) value = Mathf.Abs(value);
            }
            else
            {
                f = chartOptions.label.numericFormat;
                if (chartOptions.label.absoluteValue) value = Mathf.Abs(value);
            }
            float sum = value >= 0.0f ? chartGrid.positiveSum[cateIndex] : chartGrid.negativeSum[cateIndex];
            format = format.Replace("{category}", chartData.categories[cateIndex]);
            format = format.Replace("{series.name}", chartData.series[seriesIndex].name);
            format = format.Replace("{data.value}", GetValueString(value, f));
            format = format.Replace("{data.percentage}", GetPercentageString(value / sum * 100, f));
            return format;
        }

        string GetValueString(float value, string format)
        {
            if (format == "") return ChartHelper.FloatToString(value);
            else return value.ToString(format);
        }

        string GetPercentageString(float value, string format)
        {
            if (format == "") return value.ToString("f0") + "%";
            else return value.ToString(format) + "%";
        }

        void UpdateItems()
        {
            //template
#if CHART_TMPRO
            TextMeshProUGUI labelTemp;
#else
            Text labelTemp;
#endif
            labelTemp = ChartHelper.CreateText("Label", transform, chartOptions.label.textOption, chartOptions.plotOptions.generalFont);
            labelTemp.rectTransform.anchorMin = Vector2.zero;
            labelTemp.rectTransform.anchorMax = Vector2.zero;
            labelTemp.rectTransform.sizeDelta = Vector2.zero;
            labelTemp.rectTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, chartOptions.label.rotation);

            //material
            float smoothness = Mathf.Clamp01(3.0f / chartOptions.plotOptions.lineChartOption.pointSize);
            float outlineWidth = Mathf.Clamp01(1.0f - chartOptions.plotOptions.lineChartOption.pointOutlineWidth * 2.0f / chartOptions.plotOptions.lineChartOption.pointSize - smoothness);
            float lineSmoothness = Mathf.Clamp01(3.0f / chartOptions.plotOptions.lineChartOption.lineWidth);
            if (chartOptions.plotOptions.lineChartOption.enablePointOutline)
            {
                pointMat = new Material(Resources.Load<Material>("Materials/Chart_OutlineCircle"));
                pointMat.SetFloat("_Smoothness", smoothness);
                pointMat.SetFloat("_InnerRadius", outlineWidth);
                pointMat.SetColor("_OutlineColor", chartOptions.plotOptions.lineChartOption.pointOutlineColor);
            }
            else
            {
                pointMat = new Material(Resources.Load<Material>("Materials/Chart_Circle"));
                pointMat.SetFloat("_Smoothness", smoothness);
            }
            if (chartOptions.plotOptions.lineChartOption.enableLine)
            {
                lineMat = new Material(Resources.Load<Material>("Materials/Chart_UBlur"));
                lineMat.SetFloat("_Smoothness", lineSmoothness);
            }

            //item
            pointList = new LineChartPoint[chartData.series.Count];
            float[] stackValueList = chartOptions.plotOptions.columnStacking == ColumnStacking.None ? null : new float[chartData.categories.Count];
            float[] stackValueListNeg = chartOptions.plotOptions.columnStacking == ColumnStacking.None ? null : new float[chartData.categories.Count];
            for (int i = 0; i < chartData.series.Count; ++i)
            {
                RectTransform seriesRect = ChartHelper.CreateEmptyRect(chartData.series[i].name, dataRect, true);
                seriesRect.SetAsFirstSibling();
                RectTransform seriesLabelRect = ChartHelper.CreateEmptyRect(chartData.series[i].name, labelRect, true);
                seriesLabelRect.SetAsFirstSibling();
                if (skipSeries.Contains(i) || !chartData.series[i].show) continue;

                //point
                pointList[i] = ChartHelper.CreateEmptyRect("Point", seriesRect, true).gameObject.AddComponent<LineChartPoint>();
                pointList[i].material = pointMat;
                pointList[i].color = chartOptions.plotOptions.dataColor[i % chartOptions.plotOptions.dataColor.Length];
                pointList[i].diameter = chartOptions.plotOptions.lineChartOption.pointSize;
                pointList[i].inverted = chartOptions.plotOptions.inverted;
                pointList[i].reverse = chartOptions.plotOptions.reverseSeries ^ chartOptions.plotOptions.inverted;
                pointList[i].data = new Vector2[chartData.categories.Count];
                pointList[i].seriesIndex = i;
                pointList[i].chart = this;
                for (int j = 0; j < chartData.categories.Count; ++j) pointList[i].data[j] = GetDataRatio(i, j, stackValueList, stackValueListNeg);

                //line
                if (chartOptions.plotOptions.lineChartOption.enableLine)
                {
                    LineChartLine line = ChartHelper.CreateEmptyRect("Line", seriesRect, true).gameObject.AddComponent<LineChartLine>();
                    line.transform.SetAsFirstSibling();
                    line.material = lineMat;
                    line.color = pointList[i].color;
                    line.width = chartOptions.plotOptions.lineChartOption.lineWidth;
                    line.inverted = pointList[i].inverted;
                    line.reverse = pointList[i].reverse;
                    line.data = pointList[i].data;
                    line.seriesIndex = i;
                    line.chart = this;
                }

                //shade
                if (chartOptions.plotOptions.lineChartOption.enableShade)
                {
                    LineChartShade shade = ChartHelper.CreateEmptyRect("Shade", seriesRect, true).gameObject.AddComponent<LineChartShade>();
                    shade.transform.SetAsFirstSibling();
                    shade.color = new Color(pointList[i].color.r, pointList[i].color.g, pointList[i].color.b, chartOptions.plotOptions.lineChartOption.shadeTransparency);
                    shade.inverted = pointList[i].inverted;
                    shade.reverse = pointList[i].reverse;
                    shade.data = pointList[i].data;
                    shade.seriesIndex = i;
                    shade.chart = this;
                }

                //label
                if (chartOptions.label.enable)
                {
                    if (chartOptions.plotOptions.inverted)
                    {
                        float offsetPos = 0.0f;
                        float unit = chartGrid.unitWidth;
                        if (!chartOptions.plotOptions.reverseSeries)
                        {
                            unit *= -1;
                            offsetPos = chartGrid.chartSize.y;
                        }
                        offsetPos += unit * 0.5f;

                        for (int j = 0; j < chartData.categories.Count; ++j)
                        {
                            if (!IsValid(i, j)) continue;
                            float pos = offsetPos + unit * j;
                            float h = chartGrid.chartSize.x * (pointList[i].data[j].y + pointList[i].data[j].x * chartOptions.label.anchoredPosition) +
                                chartOptions.label.offset * Mathf.Sign(pointList[i].data[j].x);
                            var label = Instantiate(labelTemp, seriesLabelRect);
                            label.text = GetFormattedPointText(i, j, chartOptions.label.format, 1);
                            label.rectTransform.anchoredPosition = new Vector2(h, pos);
                        }
                    }
                    else
                    {
                        float offsetPos = 0.0f;
                        float unit = chartGrid.unitWidth;
                        if (chartOptions.plotOptions.reverseSeries)
                        {
                            unit *= -1;
                            offsetPos = chartGrid.chartSize.x;
                        }
                        offsetPos += unit * 0.5f;

                        for (int j = 0; j < chartData.categories.Count; ++j)
                        {
                            if (!IsValid(i, j)) continue;
                            float pos = offsetPos + unit * j;
                            float h = chartGrid.chartSize.y * (pointList[i].data[j].y + pointList[i].data[j].x * chartOptions.label.anchoredPosition) +
                                chartOptions.label.offset * Mathf.Sign(pointList[i].data[j].x);
                            var label = Instantiate(labelTemp, seriesLabelRect);
                            label.text = GetFormattedPointText(i, j, chartOptions.label.format, 1);
                            label.rectTransform.anchoredPosition = new Vector2(pos, h);
                        }
                    }
                }
            }
            ChartHelper.Destroy(labelTemp.gameObject);
        }
    }
}
