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
    public class Chart : MonoBehaviour
    {
        public ChartOptions chartOptions = null;
        public ChartData chartData = null;
        public ChartType chartType = ChartType.BarChart;
        [SerializeField] bool updateOnAwake = true;

        Vector2 chartSize;
        Vector2 offsetMin, offsetMax;
        ChartTooltip tooltip;
        ChartEvents chartEvents;
        ChartBase chart;
        RectTransform chartRect;
        RectTransform legendRect;

        public void Clear()
        {
            ChartHelper.Clear(transform);
        }

        private void Start()
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
#endif
            if (updateOnAwake) UpdateChart();
        }

        public void ToggleSeries(int index)
        {
            chartData.series[index].show = !chartData.series[index].show;
            UpdateChart();
            if (chartEvents != null) chartEvents.seriesToggleEvent.Invoke(index, chartData.series[index].show);
        }

        public void UpdateChart()
        {
            if (chartOptions == null || chartData == null) return;
            Clear();
#if CHART_TMPRO
            if (chartOptions.plotOptions.generalFont == null)
                chartOptions.plotOptions.generalFont = Resources.Load("Fonts & Materials/LiberationSans SDF", typeof(TMP_FontAsset)) as TMP_FontAsset;
#else
            if (chartOptions.plotOptions.generalFont == null)
                chartOptions.plotOptions.generalFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
#endif

            offsetMin = offsetMax = Vector2.zero;
            chartSize = GetComponent<RectTransform>().rect.size;
            chartEvents = GetComponent<ChartEvents>();
            chartRect = ChartHelper.CreateEmptyRect("Content", transform);
            legendRect = ChartHelper.CreateEmptyRect("Legends", transform);

            if (chartOptions.tooltip.enable) UpdateTooltip();
            if (chartOptions.title.enableMainTitle) UpdateMainTitle();
            if (chartOptions.title.enableSubTitle) UpdateSubTitle();
            if (chartOptions.legend.enable) UpdateLegend();
            UpdateContent();
        }

        void UpdateTooltip()
        {
            tooltip = ChartHelper.CreateEmptyRect("ChartTooltip", transform).gameObject.AddComponent<ChartTooltip>();

            tooltip.background = tooltip.gameObject.AddComponent<Image>();
            tooltip.background.rectTransform.anchorMin = Vector2.zero;
            tooltip.background.rectTransform.anchorMax = Vector2.zero;
            tooltip.background.rectTransform.pivot = new Vector2(0.5f, 0.0f);
            tooltip.background.sprite = Resources.Load<Sprite>("Images/Chart_Square");
            tooltip.background.color = chartOptions.tooltip.backgroundColor;
            tooltip.background.type = Image.Type.Sliced;
            tooltip.background.raycastTarget = false;

            Canvas c = tooltip.gameObject.AddComponent<Canvas>();
            c.overrideSorting = true;
            c.sortingOrder = 10000;
            tooltip.tooltipText = ChartHelper.CreateText("TooltipText", tooltip.transform, chartOptions.tooltip.textOption, chartOptions.plotOptions.generalFont, TextAnchor.UpperLeft, true);
            tooltip.tooltipText.rectTransform.offsetMin = new Vector2(8, 3);
            tooltip.tooltipText.rectTransform.offsetMax = new Vector2(-8, -3);

            tooltip.chartSize = chartSize;
            tooltip.parentPivot = GetComponent<RectTransform>().pivot;
            tooltip.gameObject.SetActive(false);
        }

        void UpdateMainTitle()
        {
            var mainTitle = ChartHelper.CreateText("MainTitle", transform, chartOptions.title.mainTitleOption, chartOptions.plotOptions.generalFont);
            mainTitle.text = chartOptions.title.mainTitle;
            if (mainTitle.preferredWidth > chartSize.x) ChartHelper.TruncateText(mainTitle, chartSize.x);

            float height = mainTitle.fontSize * 1.4f;
            mainTitle.rectTransform.anchorMin = new Vector2(0.0f, 1.0f);
            mainTitle.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            mainTitle.rectTransform.offsetMin = new Vector2(0.0f, -height);
            mainTitle.rectTransform.offsetMax = new Vector2(0.0f, 0.0f);
            offsetMax.y -= height;
        }

        void UpdateSubTitle()
        {
            var subTitle = ChartHelper.CreateText("SubTitle", transform, chartOptions.title.subTitleOption, chartOptions.plotOptions.generalFont);
            subTitle.text = chartOptions.title.subTitle;
            if (subTitle.preferredWidth > chartSize.x) ChartHelper.TruncateText(subTitle, chartSize.x);

            float height = subTitle.fontSize * 1.2f;
            subTitle.rectTransform.anchorMin = new Vector2(0.0f, 1.0f);
            subTitle.rectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            subTitle.rectTransform.offsetMin = new Vector2(0.0f, offsetMax.y - height);
            subTitle.rectTransform.offsetMax = new Vector2(0.0f, offsetMax.y);
            offsetMax.y -= height;
        }

        void UpdateLegend()
        {
            //legend template
            ChartLegend legendTemp = ChartHelper.CreateEmptyRect("ChartLegend", transform).gameObject.AddComponent<ChartLegend>();
            legendTemp.background = legendTemp.gameObject.AddComponent<Image>();
            legendTemp.background.sprite = Resources.Load<Sprite>("Images/Chart_Square");
            legendTemp.background.color = chartOptions.legend.backgroundColor;
            legendTemp.background.type = Image.Type.Sliced;
            legendTemp.text = ChartHelper.CreateText("LegendLabel", legendTemp.transform, chartOptions.legend.textOption, chartOptions.plotOptions.generalFont, TextAnchor.MiddleLeft, true);
            legendTemp.text.rectTransform.offsetMin = new Vector2(legendTemp.text.fontSize * 1.5f, 0.0f);
            legendTemp.icon = ChartHelper.CreateImage("Icon", legendTemp.transform);
            legendTemp.icon.sprite = chartOptions.legend.iconImage;
            legendTemp.icon.rectTransform.anchorMin = new Vector2(0.0f, 0.5f);
            legendTemp.icon.rectTransform.anchorMax = new Vector2(0.0f, 0.5f);
            legendTemp.icon.rectTransform.sizeDelta = new Vector2(legendTemp.text.fontSize * 0.75f, legendTemp.text.fontSize * 0.75f);
            legendTemp.icon.rectTransform.anchoredPosition = new Vector2(legendTemp.text.fontSize * 0.75f, 0.0f);
            if (chartType == ChartType.BarChart) legendTemp.icon.gameObject.SetActive(!chartOptions.plotOptions.barChartOption.colorByCategories);
            else if (chartType == ChartType.RoseChart) legendTemp.icon.gameObject.SetActive(!chartOptions.plotOptions.roseChartOption.colorByCategories);

            //update items
            float itemMaxWidth = 0.0f;
            Vector2 itemSumSize = Vector2.zero;
            List<ChartLegend> legendList = new List<ChartLegend>();
            for (int i = 0; i < chartData.series.Count; ++i)
            {
                ChartLegend legend = Instantiate(legendTemp, legendRect);
                legend.gameObject.name = chartData.series[i].name;
                legend.text.text = chartData.series[i].name;

                float width = legend.text.preferredWidth + legendTemp.text.fontSize * 2.0f;
                if (width > itemMaxWidth) itemMaxWidth = width;
                legend.background.rectTransform.sizeDelta = new Vector2(width, legendTemp.text.fontSize * 1.5f);
                legend.Init(i, this);
                legend.SetStatus(chartData.series[i].show);
                itemSumSize += legend.background.rectTransform.sizeDelta;
                legendList.Add(legend);
            }

            //update rect
            int alignment = 0;
            float limitW = 0.0f, limitH = 0.0f;
            bool controlW = false, controlH = false;
            float offset = 0.0f;
            if (chartOptions.legend.itemLayout == RectTransform.Axis.Horizontal)
            {
                int rows = chartOptions.legend.horizontalRows < 1 ? 1 : chartOptions.legend.horizontalRows;
                switch (chartOptions.legend.alignment)
                {
                    case TextAnchor.LowerCenter:
                    case TextAnchor.LowerLeft:
                    case TextAnchor.LowerRight:
                        alignment = (int)chartOptions.legend.alignment % 3;
                        limitW = chartSize.x;
                        limitH = chartSize.y - chartSize.x > chartSize.x ? chartSize.y - chartSize.x : chartSize.y * 0.4f;
                        offset = Mathf.Clamp(legendTemp.text.fontSize * 1.5f * rows, 0.0f, limitH);
                        legendRect.anchorMin = new Vector2(0.0f, 0.0f);
                        legendRect.anchorMax = new Vector2(1.0f, 0.0f);
                        legendRect.offsetMin = new Vector2(0.0f, 0.0f);
                        legendRect.offsetMax = new Vector2(0.0f, offset);
                        offsetMin.y += offset;
                        break;
                    case TextAnchor.UpperCenter:
                    case TextAnchor.UpperLeft:
                    case TextAnchor.UpperRight:
                        alignment = (int)chartOptions.legend.alignment % 3;
                        limitW = chartSize.x;
                        limitH = chartSize.y - chartSize.x > chartSize.x ? chartSize.y - chartSize.x : chartSize.y * 0.4f;
                        offset = Mathf.Clamp(legendTemp.text.fontSize * 1.5f * rows, 0.0f, limitH);
                        legendRect.anchorMin = new Vector2(0.0f, 1.0f);
                        legendRect.anchorMax = new Vector2(1.0f, 1.0f);
                        legendRect.offsetMin = new Vector2(0.0f, offsetMax.y - offset);
                        legendRect.offsetMax = new Vector2(0.0f, offsetMax.y);
                        offsetMax.y -= offset;
                        break;
                    case TextAnchor.MiddleLeft:
                        alignment = 1;
                        limitW = chartSize.x - chartSize.y > chartSize.y ? chartSize.x - chartSize.y : chartSize.x * 0.4f;
                        limitH = chartSize.y;
                        offset = Mathf.Clamp(itemSumSize.x, 0.0f, limitW);
                        legendRect.anchorMin = new Vector2(0.0f, 0.0f);
                        legendRect.anchorMax = new Vector2(0.0f, 1.0f);
                        legendRect.offsetMin = new Vector2(0.0f, 0.0f);
                        legendRect.offsetMax = new Vector2(offset, offsetMax.y);
                        offsetMin.x += offset;
                        break;
                    case TextAnchor.MiddleRight:
                        alignment = 1;
                        limitW = chartSize.x - chartSize.y > chartSize.y ? chartSize.x - chartSize.y : chartSize.x * 0.4f;
                        limitH = chartSize.y;
                        offset = Mathf.Clamp(itemSumSize.x, 0.0f, limitW);
                        legendRect.anchorMin = new Vector2(1.0f, 0.0f);
                        legendRect.anchorMax = new Vector2(1.0f, 1.0f);
                        legendRect.offsetMin = new Vector2(-offset, 0.0f);
                        legendRect.offsetMax = new Vector2(0.0f, offsetMax.y);
                        offsetMax.x -= offset;
                        break;
                    default:
                        alignment = 1;
                        limitW = chartSize.x;
                        limitH = chartSize.y;
                        legendRect.anchorMin = new Vector2(0.0f, 0.0f);
                        legendRect.anchorMax = new Vector2(1.0f, 1.0f);
                        legendRect.offsetMin = new Vector2(0.0f, 0.0f);
                        legendRect.offsetMax = new Vector2(0.0f, offsetMax.y);
                        break;
                }

                if (rows > 1)
                {
                    ChartHelper.AddVerticalLayout(legendRect.gameObject, true, false, alignment);
                    List<RectTransform> legendRow = new List<RectTransform>();
                    List<ChartLegend>[] legends = new List<ChartLegend>[rows];
                    float[] sumWidth = new float[rows];
                    for (int i = 0; i < rows; ++i)
                    {
                        RectTransform row = ChartHelper.CreateEmptyRect("Legends", legendRect);
                        row.sizeDelta = new Vector2(0.0f, legendTemp.text.fontSize * 1.5f);
                        legendRow.Add(row);
                        legends[i] = new List<ChartLegend>();
                    }

                    int num = Mathf.CeilToInt(legendList.Count / (float)rows);
                    for (int i = 0; i < legendList.Count; ++i)
                    {
                        int index = i / num;
                        legendList[i].transform.SetParent(legendRow[index]);
                        legends[index].Add(legendList[i]);
                        sumWidth[index] += legendList[i].background.rectTransform.sizeDelta.x;
                    }

                    for (int i = 0; i < rows; ++i)
                    {
                        if (sumWidth[i] > limitW)
                        {
                            controlW = true;
                            float wLimit = limitW / legends[i].Count - legendTemp.text.fontSize * 1.5f;
                            for (int j = 0; j < legends[i].Count; ++j)
                            {
                                if (legends[i][j].text.preferredWidth > wLimit) ChartHelper.TruncateText(legends[i][j].text, wLimit);
                            }
                        }
                        else
                        {
                            controlW = false;
                        }
                        ChartHelper.AddHorizontalLayout(legendRow[i].gameObject, controlW, controlH, alignment);
                    }
                }
                else
                {
                    if (itemSumSize.x > limitW)
                    {
                        controlW = true;
                        float wLimit = limitW / legendList.Count - legendTemp.text.fontSize * 1.5f;
                        foreach (ChartLegend l in legendList) if (l.text.preferredWidth > wLimit) ChartHelper.TruncateText(l.text, wLimit);
                    }
                    ChartHelper.AddHorizontalLayout(legendRect.gameObject, controlW, controlH, alignment);
                }
            }
            else
            {
                limitH = Mathf.Clamp(itemSumSize.y, 0.0f, chartSize.y * 0.4f);
                switch (chartOptions.legend.alignment)
                {
                    case TextAnchor.MiddleLeft:
                    case TextAnchor.UpperLeft:
                    case TextAnchor.LowerLeft:
                        alignment = (int)chartOptions.legend.alignment / 3;
                        limitW = chartSize.x - chartSize.y > chartSize.y ? chartSize.x - chartSize.y : chartSize.x * 0.4f;
                        limitH = chartSize.y;
                        offset = Mathf.Clamp(itemMaxWidth, 0.0f, limitW);
                        legendRect.anchorMin = new Vector2(0.0f, 0.0f);
                        legendRect.anchorMax = new Vector2(0.0f, 1.0f);
                        legendRect.offsetMin = new Vector2(0.0f, 0.0f);
                        legendRect.offsetMax = new Vector2(offset, offsetMax.y);
                        offsetMin.x += offset;
                        break;
                    case TextAnchor.MiddleRight:
                    case TextAnchor.UpperRight:
                    case TextAnchor.LowerRight:
                        alignment = (int)chartOptions.legend.alignment / 3;
                        limitW = chartSize.x - chartSize.y > chartSize.y ? chartSize.x - chartSize.y : chartSize.x * 0.4f;
                        limitH = chartSize.y;
                        offset = Mathf.Clamp(itemMaxWidth, 0.0f, limitW);
                        legendRect.anchorMin = new Vector2(1.0f, 0.0f);
                        legendRect.anchorMax = new Vector2(1.0f, 1.0f);
                        legendRect.offsetMin = new Vector2(-offset, 0.0f);
                        legendRect.offsetMax = new Vector2(0.0f, offsetMax.y);
                        offsetMax.x -= offset;
                        break;
                    case TextAnchor.LowerCenter:
                        alignment = 1;
                        limitW = chartSize.x;
                        limitH = chartSize.y - chartSize.x > chartSize.x ? chartSize.y - chartSize.x : chartSize.y * 0.4f;
                        offset = Mathf.Clamp(itemSumSize.y, 0.0f, limitH);
                        legendRect.anchorMin = new Vector2(0.0f, 0.0f);
                        legendRect.anchorMax = new Vector2(1.0f, 0.0f);
                        legendRect.offsetMin = new Vector2(0.0f, 0.0f);
                        legendRect.offsetMax = new Vector2(0.0f, offset);
                        offsetMin.y += offset;
                        break;
                    case TextAnchor.UpperCenter:
                        alignment = 1;
                        limitW = chartSize.x;
                        limitH = chartSize.y - chartSize.x > chartSize.x ? chartSize.y - chartSize.x : chartSize.y * 0.4f;
                        offset = Mathf.Clamp(itemSumSize.y, 0.0f, limitH);
                        legendRect.anchorMin = new Vector2(0.0f, 1.0f);
                        legendRect.anchorMax = new Vector2(1.0f, 1.0f);
                        legendRect.offsetMin = new Vector2(0.0f, offsetMax.y - offset);
                        legendRect.offsetMax = new Vector2(0.0f, offsetMax.y);
                        offsetMax.y -= offset;
                        break;
                    default:
                        alignment = 1;
                        limitW = chartSize.x;
                        limitH = chartSize.y;
                        legendRect.anchorMin = new Vector2(0.0f, 0.0f);
                        legendRect.anchorMax = new Vector2(1.0f, 1.0f);
                        legendRect.offsetMin = new Vector2(0.0f, 0.0f);
                        legendRect.offsetMax = new Vector2(0.0f, offsetMax.y);
                        break;
                }
                if (itemMaxWidth > limitW)
                {
                    controlW = true;
                    foreach (ChartLegend l in legendList) if (l.text.preferredWidth > limitW) ChartHelper.TruncateText(l.text, limitW);
                }
                if (itemSumSize.y > limitH) controlH = true;
                ChartHelper.AddVerticalLayout(legendRect.gameObject, controlW, controlH, alignment);
            }

            ChartHelper.Destroy(legendTemp.gameObject);
        }

        void UpdateContent()
        {
            if (chartType == ChartType.BarChart || chartType == ChartType.LineChart)
            {
                offsetMax -= new Vector2(chartOptions.yAxis.tickSize.y, chartOptions.xAxis.tickSize.y);
            }
            offsetMin.x = Mathf.Clamp(offsetMin.x, 0.0f, chartSize.x * 0.4f);
            offsetMin.y = Mathf.Clamp(offsetMin.y, 0.0f, chartSize.y * 0.4f);
            offsetMax.x = Mathf.Clamp(offsetMax.x, -chartSize.x * 0.4f, 0.0f);
            offsetMax.y = Mathf.Clamp(offsetMax.y, -chartSize.y * 0.4f, 0.0f);
            chartSize -= offsetMin - offsetMax;

            chartRect.anchorMin = Vector2.zero;
            chartRect.anchorMax = Vector2.one;
            chartRect.offsetMin = offsetMin;
            chartRect.offsetMax = offsetMax;

            switch (chartType)
            {
                case ChartType.BarChart:
                    chart = chartRect.gameObject.AddComponent<BarChart>();
                    break;
                case ChartType.LineChart:
                    chart = chartRect.gameObject.AddComponent<LineChart>();
                    break;
                case ChartType.PieChart:
                    chart = chartRect.gameObject.AddComponent<PieChart>();
                    break;
                case ChartType.RoseChart:
                    chart = chartRect.gameObject.AddComponent<RoseChart>();
                    break;
                case ChartType.RadarChart:
                    chart = chartRect.gameObject.AddComponent<RadarChart>();
                    break;
            }
            chart.chartSize = chartSize;
            chart.tooltip = tooltip;
            chart.chartRect = chartRect;
            chart.chartData = chartData;
            chart.chartOptions = chartOptions;
            chart.chartEvents = chartEvents;
            ChartMixer mixer = GetComponent<ChartMixer>();
            if (mixer == null) chart.UpdateChart();
            else mixer.UpdateChart(chart);
        }
    }
}
