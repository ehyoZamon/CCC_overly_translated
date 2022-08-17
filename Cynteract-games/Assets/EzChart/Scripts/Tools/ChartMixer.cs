using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChartUtil
{
    public enum MixMethod
    {
        None, BarToLine, LineToBar
    }

    public class ChartMixer : MonoBehaviour
    {
        [SerializeField] MixMethod mixMethod;
        [SerializeField] List<int> seriesToMix;

        public void UpdateChart(ChartBase chart)
        {
            if (seriesToMix.Count == 0) { chart.UpdateChart(); return; }

            switch (mixMethod)
            {
                case MixMethod.BarToLine:
                    if (chart.GetComponent<BarChart>() != null)
                    {
                        LineChart lineChart = chart.gameObject.AddComponent<LineChart>();
                        lineChart.tooltip = chart.tooltip;
                        lineChart.chartRect = chart.chartRect;
                        lineChart.chartData = chart.chartData;
                        lineChart.chartOptions = chart.chartOptions;
                        for (int i = 0; i < chart.chartData.series.Count; ++i)
                        {
                            if (seriesToMix.Contains(i)) chart.skipSeries.Add(i);
                            else lineChart.skipSeries.Add(i);
                        }
                        chart.UpdateChart();
                        lineChart.UpdateChart();
                    }
                    else chart.UpdateChart();
                    break;
                case MixMethod.LineToBar:
                    if (chart.GetComponent<LineChart>() != null)
                    {
                        BarChart barChart = chart.gameObject.AddComponent<BarChart>();
                        barChart.tooltip = chart.tooltip;
                        barChart.chartRect = chart.chartRect;
                        barChart.chartData = chart.chartData;
                        barChart.chartOptions = chart.chartOptions;
                        for (int i = 0; i < chart.chartData.series.Count; ++i)
                        {
                            if (seriesToMix.Contains(i)) chart.skipSeries.Add(i);
                            else barChart.skipSeries.Add(i);
                        }
                        chart.UpdateChart();
                        barChart.UpdateChart();
                    }
                    else chart.UpdateChart();
                    break;
                default:
                    chart.UpdateChart();
                    break;
            }
        }
    }
}