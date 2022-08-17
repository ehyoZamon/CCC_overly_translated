using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChartUtil
{
    public class LineChartShade : ChartGraphic
    {
        public Vector2[] data = null; //ratio, starting
        public bool inverted = false;
        public bool reverse = false;
        public int seriesIndex = 0;
        public LineChart chart;

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
                m_offset.y += unit * 0.5f;

                Vector2 pStart = new Vector2(m_offset.x + size.x * data[0].y, m_offset.y);
                Vector2 p = pStart + new Vector2(size.x * data[0].x, 0.0f);
                for (int i = 1; i < data.Length; ++i)
                {
                    if (!chart.IsValid(seriesIndex, i)) continue;
                    Vector2 lastStart = pStart;
                    Vector2 lastPoint = p;
                    pStart = new Vector2(m_offset.x + size.x * data[i].y, m_offset.y + unit * i);
                    p = pStart + new Vector2(size.x * data[i].x, 0.0f);
                    if (!chart.IsValid(seriesIndex, i - 1)) continue;

                    points[0] = lastStart;
                    points[1] = lastPoint;
                    points[2] = p;
                    points[3] = pStart;

                    if (Vector2.Dot(points[1] - points[0], points[2] - points[3]) >= 0.0f)
                    {
                        vh.AddVert(points[0], color, Vector2.zero);
                        vh.AddVert(points[1], color, Vector2.zero);
                        vh.AddVert(points[2], color, Vector2.zero);
                        vh.AddVert(points[3], color, Vector2.zero);

                        vh.AddTriangle(index, index + 1, index + 2);
                        vh.AddTriangle(index + 2, index + 3, index);
                        index += 4;
                    }
                    else
                    {
                        float t = (points[1].x - points[0].x) * (points[0].y - points[3].y) - (points[0].x - points[3].x) * (points[1].y - points[0].y);
                        t /= (points[1].x - points[2].x) * (points[0].y - points[3].y) - (points[0].x - points[3].x) * (points[1].y - points[2].y);
                        Vector2 pZero = points[1] + (points[2] - points[1]) * t;

                        vh.AddVert(points[0], color, Vector2.zero);
                        vh.AddVert(points[1], color, Vector2.zero);
                        vh.AddVert(points[2], color, Vector2.zero);
                        vh.AddVert(points[3], color, Vector2.zero);
                        vh.AddVert(pZero, color, Vector2.zero);

                        vh.AddTriangle(index, index + 1, index + 4);
                        vh.AddTriangle(index + 4, index + 3, index + 2);
                        index += 5;
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
                m_offset.x += unit * 0.5f;

                Vector2 pStart = new Vector2(m_offset.x, m_offset.y + size.y * data[0].y);
                Vector2 p = pStart + new Vector2(0.0f, size.y * data[0].x);
                for (int i = 1; i < data.Length; ++i)
                {
                    if (!chart.IsValid(seriesIndex, i)) continue;
                    Vector2 lastStart = pStart;
                    Vector2 lastPoint = p;
                    pStart = new Vector2(m_offset.x + unit * i, m_offset.y + size.y * data[i].y);
                    p = pStart + new Vector2(0.0f, size.y * data[i].x);
                    if (!chart.IsValid(seriesIndex, i - 1)) continue;

                    points[0] = lastStart;
                    points[1] = lastPoint;
                    points[2] = p;
                    points[3] = pStart;

                    if (Vector2.Dot(points[1] - points[0], points[2] - points[3]) >= 0.0f)
                    {
                        vh.AddVert(points[0], color, Vector2.zero);
                        vh.AddVert(points[1], color, Vector2.zero);
                        vh.AddVert(points[2], color, Vector2.zero);
                        vh.AddVert(points[3], color, Vector2.zero);

                        vh.AddTriangle(index, index + 1, index + 2);
                        vh.AddTriangle(index + 2, index + 3, index);
                        index += 4;
                    }
                    else
                    {
                        float t = (points[1].x - points[0].x) * (points[0].y - points[3].y) - (points[0].x - points[3].x) * (points[1].y - points[0].y);
                        t /= (points[1].x - points[2].x) * (points[0].y - points[3].y) - (points[0].x - points[3].x) * (points[1].y - points[2].y);
                        Vector2 pZero = points[1] + (points[2] - points[1]) * t;

                        vh.AddVert(points[0], color, Vector2.zero);
                        vh.AddVert(points[1], color, Vector2.zero);
                        vh.AddVert(points[2], color, Vector2.zero);
                        vh.AddVert(points[3], color, Vector2.zero);
                        vh.AddVert(pZero, color, Vector2.zero);

                        vh.AddTriangle(index, index + 1, index + 4);
                        vh.AddTriangle(index + 4, index + 3, index + 2);
                        index += 5;
                    }
                }
            }
        }
    }
}