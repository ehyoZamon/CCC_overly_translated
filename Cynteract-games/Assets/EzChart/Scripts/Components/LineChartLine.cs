using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChartUtil
{
    public class LineChartLine : ChartGraphic
    {
        public Vector2[] data = null; //ratio, starting
        public float width = 2.0f;
        public bool inverted = false;
        public bool reverse = false;
        public int seriesIndex = 0;
        public LineChart chart;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            if (data == null || data.Length == 0 || chart == null) return;

            float halfWidth = width * 0.5f;
            Vector2 size = rectTransform.rect.size;
            Vector2 m_offset = -size * 0.5f;

            int index = 0;
            Vector2[] points = new Vector2[4];
            Vector2[] uvs = new Vector2[2];
            uvs[0] = new Vector2(0.0f, 0.0f);
            uvs[1] = new Vector2(1.0f, 0.0f);

            if (inverted)
            {
                float unit = size.y / data.Length;
                if (reverse)
                {
                    unit *= -1;
                    m_offset.y *= -1;
                }
                m_offset.y += unit * 0.5f;

                Vector2 p = new Vector2(m_offset.x + size.x * (data[0].y + data[0].x), m_offset.y);
                for (int i = 1; i < data.Length; ++i)
                {
                    if (!chart.IsValid(seriesIndex, i)) continue;
                    Vector2 lastPoint = p;
                    p = new Vector2(m_offset.x + size.x * (data[i].y + data[i].x), m_offset.y + unit * i);
                    if (!chart.IsValid(seriesIndex, i - 1)) continue;

                    Vector2 dir = p - lastPoint;
                    points = CreateRect(dir, halfWidth);

                    vh.AddVert(points[0] + lastPoint, color, uvs[0]);
                    vh.AddVert(points[1] + lastPoint, color, uvs[0]);
                    vh.AddVert(points[2] + lastPoint, color, uvs[1]);
                    vh.AddVert(points[3] + lastPoint, color, uvs[1]);

                    vh.AddTriangle(index, index + 1, index + 2);
                    vh.AddTriangle(index + 2, index + 3, index);
                    index += 4;
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

                Vector2 p = new Vector2(m_offset.x, m_offset.y + size.y * (data[0].y + data[0].x));
                for (int i = 1; i < data.Length; ++i)
                {
                    if (!chart.IsValid(seriesIndex, i)) continue;
                    Vector2 lastPoint = p;
                    p = new Vector2(m_offset.x + unit * i, m_offset.y + size.y * (data[i].y + data[i].x));
                    if (!chart.IsValid(seriesIndex, i - 1)) continue;

                    Vector2 dir = p - lastPoint;
                    points = CreateRect(dir, halfWidth);

                    vh.AddVert(points[0] + lastPoint, color, uvs[0]);
                    vh.AddVert(points[1] + lastPoint, color, uvs[0]);
                    vh.AddVert(points[2] + lastPoint, color, uvs[1]);
                    vh.AddVert(points[3] + lastPoint, color, uvs[1]);

                    vh.AddTriangle(index, index + 1, index + 2);
                    vh.AddTriangle(index + 2, index + 3, index);
                    index += 4;
                }
            }
        }
    }
}