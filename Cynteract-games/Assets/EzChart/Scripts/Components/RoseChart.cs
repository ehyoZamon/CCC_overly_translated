using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if CHART_TMPRO
using TMPro;
#endif

namespace ChartUtil
{
    public class RoseChart : ChartBase
    {
        ChartGridCircle chartGrid;
        float barWidth;
        float[] offset;
        RoseChartBar[] barList;
        Material barMat;

        private void OnDestroy()
        {
            if (barMat != null) ChartHelper.Destroy(barMat);
        }

        public override void UpdateChart()
        {
            chartGrid = gameObject.AddComponent<ChartGridCircle>();
            chartGrid.chartSize = chartSize;
            chartGrid.chartOptions = chartOptions;
            chartGrid.chartData = chartData;
            chartGrid.midGrid = false;
            chartGrid.circularGrid = true;
            chartGrid.innerSize = chartOptions.plotOptions.roseChartOption.innerSize;
            chartGrid.outerSize = chartOptions.plotOptions.roseChartOption.outerSize;
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
            Vector2 dir = localMousePosition - chartSize * 0.5f;
            float rInner = chartGrid.chartSize.y * 0.5f;
            float rRange = (chartGrid.chartSize.x - chartGrid.chartSize.y) * 0.5f;
            int posIndex = chartOptions.plotOptions.reverseSeries ? chartData.categories.Count - currCate - 1 : currCate;
            for (int i = 0; i < chartData.series.Count; ++i)
            {
                if (!chartData.series[i].show) continue;
                float pos = chartGrid.unitWidth * posIndex + chartGrid.unitWidth * 0.5f + offset[i] - barWidth * 0.5f;
                float distStart = rInner + rRange * barList[i].data[currCate].y;
                float dist = distStart + rRange * barList[i].data[currCate].x;
                if (chartGrid.mouseAngle > pos && chartGrid.mouseAngle < pos + barWidth &&
                    dir.sqrMagnitude > distStart * distStart && dir.sqrMagnitude < dist * dist)
                { index = i; break; }
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

        void UpdateItems()
        {
            //template
#if CHART_TMPRO
            TextMeshProUGUI labelTemp;
#else
            Text labelTemp;
#endif
            labelTemp = ChartHelper.CreateText("Label", transform, chartOptions.label.textOption, chartOptions.plotOptions.generalFont);
            labelTemp.rectTransform.sizeDelta = Vector2.zero;

            //material
            float smoothness = Mathf.Clamp01(4.0f / (chartGrid.chartSize.x - chartGrid.chartSize.y));
            barMat = new Material(Resources.Load<Material>("Materials/Chart_VBlur"));
            barMat.SetFloat("_Smoothness", smoothness);

            //item
            float maxBarWidth = chartOptions.plotOptions.columnStacking == ColumnStacking.None ? chartGrid.unitWidth / chartData.series.Count : chartGrid.unitWidth;
            barWidth = Mathf.Clamp(chartOptions.plotOptions.roseChartOption.barWidth, 0.0f, maxBarWidth);
            float barSpace = Mathf.Clamp(chartOptions.plotOptions.roseChartOption.itemSeparation, -barWidth * 0.5f, maxBarWidth - barWidth);
            float barUnit = barWidth + barSpace;

            offset = new float[chartData.series.Count];
            if (chartOptions.plotOptions.columnStacking == ColumnStacking.None)
            {
                float offsetMin = 0.0f;
                for (int i = 0; i < chartData.series.Count; ++i)
                {
                    if (skipSeries.Contains(i) || !chartData.series[i].show) continue;
                    offsetMin += barUnit;
                }
                offsetMin = -(offsetMin - barUnit) * 0.5f;
                int activeCount = 0;
                for (int i = 0; i < chartData.series.Count; ++i)
                {
                    if (skipSeries.Contains(i) || !chartData.series[i].show) continue;
                    offset[i] = offsetMin + barUnit * activeCount;
                    activeCount++;
                }
            }
            else
            {
                for (int i = 0; i < chartData.series.Count; ++i) offset[i] = 0.0f;
            }

            barList = new RoseChartBar[chartData.series.Count];
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

                //bar
                barList[i] = seriesRect.gameObject.AddComponent<RoseChartBar>();
                barList[i].material = barMat;
                barList[i].color = chartOptions.plotOptions.dataColor[i % chartOptions.plotOptions.dataColor.Length];
                barList[i].width = barWidth;
                barList[i].offset = offset[i];
                barList[i].reverse = chartOptions.plotOptions.reverseSeries ^ chartOptions.plotOptions.inverted;
                barList[i].innerSize = chartOptions.plotOptions.roseChartOption.innerSize;
                barList[i].seriesIndex = i;
                barList[i].innerExtend = 1.0f;
                barList[i].chart = this;
                barList[i].data = new Vector2[chartData.categories.Count];
                for (int j = 0; j < chartData.categories.Count; ++j) barList[i].data[j] = GetDataRatio(i, j, stackValueList);

                //label
                if (chartOptions.label.enable)
                {
                    float rInner = chartGrid.chartSize.y * 0.5f;
                    float rRange = (chartGrid.chartSize.x - chartGrid.chartSize.y) * 0.5f;
                    for (int j = 0; j < chartData.categories.Count; ++j)
                    {
                        if (!IsValid(i, j)) continue;
                        float dist = rInner + rRange * (barList[i].data[j].y + barList[i].data[j].x * chartOptions.label.anchoredPosition) + chartOptions.label.offset;
                        int posIndex = chartOptions.plotOptions.reverseSeries ? chartData.categories.Count - j - 1 : j;
                        float pos = chartGrid.unitWidth * posIndex + chartGrid.unitWidth * 0.5f + offset[i];
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