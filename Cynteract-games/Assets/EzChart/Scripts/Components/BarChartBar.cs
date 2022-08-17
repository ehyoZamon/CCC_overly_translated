using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChartUtil
{
    public class BarChartBar : ChartGraphic
    {
        public Vector2[] data = null; //ratio, starting
        public float offset = 0.0f;
        public float width = 5.0f;
        public bool inverted = false;
        public bool reverse = false;
        public int seriesIndex = 0;
        public BarChart chart;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            if (data == null || data.Length == 0 || chart == null) return;

            Vector2 size = rectTransform.rect.size;
            Vector2 m_offset = -size * 0.5f;

            int index = 0;
            Vector2[] points = new Vector2[4];

            if (inverted)
            {
                float unit = size.y / data.Length;
                if (reverse)
                {
                    unit *= -1;
                    m_offset.y *= -1;
                }
                m_offset.y += offset + unit * 0.5f - width * 0.5f;

                if (!chart.chartOptions.plotOptions.barChartOption.colorByCategories)
                {
                    for (int i = 0; i < data.Length; ++i)
                    {
                        if (!chart.IsValid(seriesIndex, i)) continue;
                        float pos = m_offset.y + unit * i;
                        float h = size.x * data[i].x;
                        float hStart = m_offset.x + size.x * data[i].y;

                        points[0] = new Vector2(hStart, pos);
                        points[1] = new Vector2(hStart + h, pos);
                        points[2] = new Vector2(hStart + h, pos + width);
                        points[3] = new Vector2(hStart, pos + width);

                        vh.AddVert(points[0], color, Vector2.zero);
                        vh.AddVert(points[1], color, Vector2.zero);
                        vh.AddVert(points[2], color, Vector2.zero);
                        vh.AddVert(points[3], color, Vector2.zero);

                        vh.AddTriangle(index, index + 1, index + 2);
                        vh.AddTriangle(index + 2, index + 3, index);
                        index += 4;
                    }
                }
                else
                {
                    Color[] colors = chart.chartOptions.plotOptions.dataColor;
                    for (int i = 0; i < data.Length; ++i)
                    {
                        if (!chart.IsValid(seriesIndex, i)) continue;
                        float pos = m_offset.y + unit * i;
                        float h = size.x * data[i].x;
                        float hStart = m_offset.x + size.x * data[i].y;

                        points[0] = new Vector2(hStart, pos);
                        points[1] = new Vector2(hStart + h, pos);
                        points[2] = new Vector2(hStart + h, pos + width);
                        points[3] = new Vector2(hStart, pos + width);

                        vh.AddVert(points[0], colors[i % colors.Length], Vector2.zero);
                        vh.AddVert(points[1], colors[i % colors.Length], Vector2.zero);
                        vh.AddVert(points[2], colors[i % colors.Length], Vector2.zero);
                        vh.AddVert(points[3], colors[i % colors.Length], Vector2.zero);

                        vh.AddTriangle(index, index + 1, index + 2);
                        vh.AddTriangle(index + 2, index + 3, index);
                        index += 4;
                    }
                }
            }
            else
            {
                float unit = size.x / data.Length;
                if (reverse)
                {
                    unit *= -1;
                    m_offset.x *= -1;
                }
                m_offset.x += offset + unit * 0.5f - width * 0.5f;

                if (!chart.chartOptions.plotOptions.barChartOption.colorByCategories)
                {
                    for (int i = 0; i < data.Length; ++i)
                    {
                        if (!chart.IsValid(seriesIndex, i)) continue;
                        float pos = m_offset.x + unit * i;
                        float h = size.y * data[i].x;
                        float hStart = m_offset.y + size.y * data[i].y;

                        points[0] = new Vector2(pos, hStart);
                        points[1] = new Vector2(pos, hStart + h);
                        points[2] = new Vector2(pos + width, hStart + h);
                        points[3] = new Vector2(pos + width, hStart);

                        vh.AddVert(points[0], color, Vector2.zero);
                        vh.AddVert(points[1], color, Vector2.zero);
                        vh.AddVert(points[2], color, Vector2.zero);
                        vh.AddVert(points[3], color, Vector2.zero);

                        vh.AddTriangle(index, index + 1, index + 2);
                        vh.AddTriangle(index + 2, index + 3, index);
                        index += 4;
                    }
                }
                else
                {
                    Color[] colors = chart.chartOptions.plotOptions.dataColor;
                    for (int i = 0; i < data.Length; ++i)
                    {
                        if (!chart.IsValid(seriesIndex, i)) continue;
                        float pos = m_offset.x + unit * i;
                        float h = size.y * data[i].x;
                        float hStart = m_offset.y + size.y * data[i].y;

                        points[0] = new Vector2(pos, hStart);
                        points[1] = new Vector2(pos, hStart + h);
                        points[2] = new Vector2(pos + width, hStart + h);
                        points[3] = new Vector2(pos + width, hStart);

                        vh.AddVert(points[0], colors[i % colors.Length], Vector2.zero);
                        vh.AddVert(points[1], colors[i % colors.Length], Vector2.zero);
                        vh.AddVert(points[2], colors[i % colors.Length], Vector2.zero);
                        vh.AddVert(points[3], colors[i % colors.Length], Vector2.zero);

                        vh.AddTriangle(index, index + 1, index + 2);
                        vh.AddTriangle(index + 2, index + 3, index);
                        index += 4;
                    }
                }
            }
        }
    }
}