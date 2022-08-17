using ChartUtil;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
namespace Cynteract.CCC.Charts
{
    using static TimespanExtensions;
    public class PointDisplayer : MonoBehaviour
    {
        public Chart chart;
        // Start is called before the first frame update
        public void DisplayData(TimestampedPointCollection timestampedPointCollection)
        {
            ChartData chartData = chart.chartData;
            chartData.series = new List<Series>();
            Series series = new Series();
            series.name = Lean.Localization.LeanLocalization.GetTranslationText("Points");
            series.show = true;
            series.data = new List<ChartUtil.Data>();
            int sum = 0;
            series.data = new List<ChartUtil.Data>();
            for (int i = 0; i < timestampedPointCollection.timestampedPoints.Count; i++)
            {
                int score = timestampedPointCollection.timestampedPoints[i].score;
                sum += score;
                series.data.Add(new ChartUtil.Data(sum, true));
            }
            //series.data = timestampedPointCollection.timestampedPoints.Select(x =>new ChartUtil.Data(x.score, true)).ToList();
            chartData.series.Add(series);
            chartData.categories= timestampedPointCollection.timestampedPoints.Select(x => x.dateTime.ToString("%d.MM")).ToList();
            chart.UpdateChart();
        }
        [Button]

        public void DisplayTestData()
        {
            var timestampedPointCollection = TimestampedPointCollection.Random(100, 100, DateTime.Now, TimeSpan.FromDays(1));
            DisplayData(timestampedPointCollection);
        }
        [Button]
        public void DisplayTrainingsData()
        {
            var timestampedPointCollection = TrainingsManager.instance.ToTimestampedPointCollection();
            DisplayData(timestampedPointCollection);
        }
        public void Display()
        {
            StopAllCoroutines();
            #warning hacky
            StartCoroutine(DisplayCoroutine());
        }
        IEnumerator DisplayCoroutine()
        {
            while (GetComponent<RectTransform>().rect.width==0)
            {
                yield return null;
            }
            DisplayTrainingsData();

        }
    }
    public class TimestampedPointCollection
    {
        public List<TimestampedPoint> timestampedPoints;

        public TimestampedPointCollection()
        {
            timestampedPoints = new List<TimestampedPoint>();
        }

        public TimestampedPointCollection(List<TimestampedPoint> values)
        {
            timestampedPoints = values;
        }


        public static TimestampedPointCollection Random(int numberOfPoints, int range, DateTime start, TimeSpan offsets)
        {
            TimestampedPointCollection timestampedPointCollection = new TimestampedPointCollection();
            for (int i = 0; i < numberOfPoints; i++)
            {
                timestampedPointCollection.timestampedPoints.Add(new TimestampedPoint(start + offsets.Multiply(i), UnityEngine.Random.Range(0, range)));
            }
            return timestampedPointCollection;
        }
    }
    [Serializable]
    public struct TimestampedPoint
    {
        public DateTime dateTime;
        public int score;

        public TimestampedPoint(DateTime dateTime, int score)
        {
            this.dateTime = dateTime;
            this.score = score;
        }
    }
}
