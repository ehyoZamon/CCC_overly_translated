using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if CHART_TMPRO
using TMPro;
#endif

namespace ChartUtil
{
    public class RadarChart : ChartBase
    {
        ChartGridCircle chartGrid;
        RadarChartPoint[] pointList;
        Material pointMat;
        Material lineMat;

        private void OnDestroy()
        {
            if (pointMat != null) ChartHelper.Destroy(pointMat);
            if (lineMat != null) ChartHelper.Destroy(lineMat);
        }

        public override void UpdateChart()
        {
            chartGrid = gameObject.AddComponent<ChartGridCircle>();
            chartGrid.chartSize = chartSize;
            chartGrid.chartOptions = chartOptions;
            chartGrid.chartData = chartData;
            chartGrid.midGrid = true;
            chartGrid.circularGrid = chartOptions.plotOptions.radarChartOption.circularGrid;
            chartGrid.innerSize = 0.0f;
            chartGrid.outerSize = chartOptions.plotOptions.radarChartOption.outerSize;
            chartGrid.UpdateGrid();

            dataRect = ChartHelper.CreateEmptyRect("DataRect", transform, true);
            labelRect = ChartHelper.CreateEmptyRect("LabelRect", transform, true);
            UpdateItems();
            chartGrid.labelRect.SetSiblingIndex(labelRect.GetSiblingIndex() - 1);
        }

        protected override int FindCategory()
        {
            return chartGrid.GetItemIndex(localMousePosition - chartSize * 0.5f);
        }

        protected override int FindSeries()
        {
            int index = -1;
            Vector2 pos = localMousePosition - chartSize * 0.5f;
            float radius = chartOptions.plotOptions.radarChartOption.pointSize * 0.5f * 1.1f;
            float r = chartGrid.chartSize.x * 0.5f;
            float sqrRadius = radius * radius;
            for (int i = 0; i < chartData.series.Count; ++i)
            {
                if (!chartData.series[i].show) continue;
                Vector2 dir = pos - pointList[i].PointsBuffer[currCate] * (pointList[i].data[currCate].y + pointList[i].data[currCate].x) * r;
                if (dir.sqrMagnitude < sqrRadius) { index = i; break; }
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
            return cateIndex < chartData.series[seriesIndex].data.Count && chartData.series[seriesIndex].data[cateIndex].show && chartData.series[seriesIndex].data[cateIndex].value >= 0.0f;
        }

        Vector2 GetDataRatio(int seriesIndex, int cateIndex, float[] stackValueList)
        {
            Vector2 value = new Vector2();
            if (!IsValid(seriesIndex, cateIndex)) return value;
            switch (chartOptions.plotOptions.columnStacking)
            {
                case ColumnStacking.None:
                    value.x = chartData.series[seriesIndex].data[cateIndex].value / chartGrid.range;
                    value.y = 0.0f;
                    break;
                case ColumnStacking.Normal:
                    value.x = chartData.series[seriesIndex].data[cateIndex].value / chartGrid.range;
                    value.y = stackValueList[cateIndex];
                    stackValueList[cateIndex] += value.x;
                    break;
                case ColumnStacking.Percent:
                    value.x = chartData.series[seriesIndex].data[cateIndex].value / chartGrid.positiveSum[cateIndex] / chartGrid.range;
                    value.y = stackValueList[cateIndex];
                    stackValueList[cateIndex] += value.x;
                    break;
                default:
                    break;
            }
            value.x = Mathf.Clamp01(value.x);
            value.y = Mathf.Clamp01(value.y);
            return value;
        }

        string GetFormattedHeaderText(int cateIndex, string format)
        {
            format = format.Replace("{category}", chartData.categories[cateIndex]);
            return format;
        }

        string GetFormattedPointText(int seriesIndex, int cateIndex, string format, int type)
        {
            string f = type == 0 ? chartOptions.tooltip.pointNumericFormat : chartOptions.label.numericFormat;
            format = format.Replace("{category}", chartData.categories[cateIndex]);
            format = format.Replace("{series.name}", chartData.series[seriesIndex].name);
            format = format.Replace("{data.value}", GetValueString(chartData.series[seriesIndex].data[cateIndex].value, f));
            format = format.Replace("{data.percentage}", GetPercentageString(chartData.series[seriesIndex].data[cateIndex].value / chartGrid.positiveSum[cateIndex] * 100, f));
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
#if CHART_TMPRO
            TextMeshProUGUI labelTemp;
#else
            Text labelTemp;
#endif
            labelTemp = ChartHelper.CreateText("Label", transform, chartOptions.label.textOption, chartOptions.plotOptions.generalFont);
            labelTemp.rectTransform.sizeDelta = Vector2.zero;

            //material
            float smoothness = Mathf.Clamp01(3.0f / chartOptions.plotOptions.radarChartOption.pointSize);
            float outlineWidth = Mathf.Clamp01(1.0f - chartOptions.plotOptions.radarChartOption.pointOutlineWidth * 2.0f / chartOptions.plotOptions.radarChartOption.pointSize - smoothness);
            float lineSmoothness = Mathf.Clamp01(3.0f / chartOptions.plotOptions.radarChartOption.lineWidth);
            if (chartOptions.plotOptions.radarChartOption.enablePointOutline)
            {
                pointMat = new Material(Resources.Load<Material>("Materials/Chart_OutlineCircle"));
                pointMat.SetFloat("_Smoothness", smoothness);
                pointMat.SetFloat("_InnerRadius", outlineWidth);
                pointMat.SetColor("_OutlineColor", chartOptions.plotOptions.radarChartOption.pointOutlineColor);
            }
            else
            {
                pointMat = new Material(Resources.Load<Material>("Materials/Chart_Circle"));
                pointMat.SetFloat("_Smoothness", smoothness);
            }
            if (chartOptions.plotOptions.radarChartOption.enableLine)
            {
                lineMat = new Material(Resources.Load<Material>("Materials/Chart_UBlur"));
                lineMat.SetFloat("_Smoothness", lineSmoothness);
            }

            //item
            pointList = new RadarChartPoint[chartData.series.Count];
            float[] stackValueList = chartOptions.plotOptions.columnStacking == ColumnStacking.None ? null : new float[chartData.categories.Count];
            for (int i = 0; i < chartData.series.Count; ++i)
            {
                RectTransform seriesRect = ChartHelper.CreateEmptyRect(chartData.series[i].name, dataRect);
                seriesRect.sizeDelta = new Vector2(chartGrid.chartSize.x, chartGrid.chartSize.x);
                seriesRect.SetAsFirstSibling();
                RectTransform seriesLabelRect = ChartHelper.CreateEmptyRect(chartData.series[i].name, labelRect);
                seriesLabelRect.sizeDelta = new Vector2(chartGrid.chartSize.x, chartGrid.chartSize.x);
                seriesLabelRect.SetAsFirstSibling();
                if (skipSeries.Contains(i) || !chartData.series[i].show) continue;

                //point
                pointList[i] = ChartHelper.CreateEmptyRect("Point", seriesRect, true).gameObject.AddComponent<RadarChartPoint>();
                pointList[i].material = pointMat;
                pointList[i].color = chartOptions.plotOptions.dataColor[i % chartOptions.plotOptions.dataColor.Length];
                pointList[i].diameter = chartOptions.plotOptions.radarChartOption.pointSize;
                pointList[i].reverse = chartOptions.plotOptions.reverseSeries;
                pointList[i].data = new Vector2[chartData.categories.Count];
                pointList[i].seriesIndex = i;
                pointList[i].chart = this;
                pointList[i].CaculateBuffer();
                for (int j = 0; j < chartData.categories.Count; ++j) pointList[i].data[j] = GetDataRatio(i, j, stackValueList);

                //line
                if (chartOptions.plotOptions.radarChartOption.enableLine)
                {
                    RadarChartLine line = ChartHelper.CreateEmptyRect("Line", seriesRect, true).gameObject.AddComponent<RadarChartLine>();
                    line.transform.SetAsFirstSibling();
                    line.material = lineMat;
                    line.color = pointList[i].color;
                    line.width = chartOptions.plotOptions.radarChartOption.lineWidth;
                    line.reverse = pointList[i].reverse;
                    line.data = pointList[i].data;
                    line.point = pointList[i];
                    line.seriesIndex = i;
                    line.chart = this;
                }

                //shade
                if (chartOptions.plotOptions.radarChartOption.enableShade)
                {
                    RadarChartShade shade = ChartHelper.CreateEmptyRect("Shade", seriesRect, true).gameObject.AddComponent<RadarChartShade>();
                    shade.transform.SetAsFirstSibling();
                    shade.color = new Color(pointList[i].color.r, pointList[i].color.g, pointList[i].color.b, chartOptions.plotOptions.radarChartOption.shadeTransparency);
                    shade.reverse = pointList[i].reverse;
                    shade.data = pointList[i].data;
                    shade.point = pointList[i];
                    shade.seriesIndex = i;
                    shade.chart = this;
                }

                //label
                if (chartOptions.label.enable)
                {
                    for (int j = 0; j < chartData.categories.Count; ++j)
                    {
                        if (!IsValid(i, j)) continue;
                        float dist = chartGrid.chartSize.x * 0.5f * (pointList[i].data[j].y + pointList[i].data[j].x * chartOptions.label.anchoredPosition) + chartOptions.label.offset;
                        int posIndex = chartOptions.plotOptions.reverseSeries ? chartData.categories.Count - j - 1 : j;
                        float pos = chartGrid.unitWidth * posIndex + chartGrid.unitWidth * 0.5f;
                        var label = Instantiate(labelTemp, seriesLabelRect);
                        label.text = GetFormattedPointText(i, j, chartOptions.label.format, 1);
                        label.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -pos);
                        label.rectTransform.anchoredPosition = label.transform.up * dist;
                        label.transform.localEulerAngles += new Vector3(0.0f, 0.0f, chartOptions.label.rotation);
                    }
                }
            }
            ChartHelper.Destroy(labelTemp.gameObject);
        }
    }
}

