using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChartUtil
{
    public class RadarChartPoint : ChartGraphic
    {
        public Vector2[] data = null; //ratio, starting
        public float diameter = 2.0f;
        public bool reverse = false;
        public int seriesIndex = 0;
        public RadarChart chart;

        Vector2[] pBuffer = null;
        public Vector2[] PointsBuffer
        {
            get
            {
                if (pBuffer == null) CaculateBuffer();
                return pBuffer;
            }
        }

        public void CaculateBuffer()
        {
            if (data == null || data.Length == 0 || chart == null) return;
            pBuffer = new Vector2[data.Length];
            float angleOffset = 360.0f / data.Length * 0.5f;
            Vector2[] cossin = GetCosSin(data.Length, angleOffset, 360.0f, false);
            if (reverse)
            {
                for (int i = 0; i < data.Length; ++i)
                {
                    if (!chart.IsValid(seriesIndex, i)) continue;
                    pBuffer[i] = RotateCCW(Vector2.up, cossin[i]);
                }
            }
            else
            {
                for (int i = 0; i < data.Length; ++i)
                {
                    if (!chart.IsValid(seriesIndex, i)) continue;
                    pBuffer[i] = RotateCW(Vector2.up, cossin[i]);
                }
            }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            if (data == null || data.Length == 0 || chart == null || pBuffer == null) return;

            float radiusP = diameter * 0.5f;
            float radius = rectTransform.rect.size.x < rectTransform.rect.size.y ? rectTransform.rect.size.x * 0.5f : rectTransform.rect.size.y * 0.5f;

            int index = 0;
            Vector2[] points = new Vector2[4];
            Vector2[] uvs = new Vector2[4];
            uvs[0] = new Vector2(0.0f, 0.0f);
            uvs[1] = new Vector2(0.0f, 1.0f);
            uvs[2] = new Vector2(1.0f, 1.0f);
            uvs[3] = new Vector2(1.0f, 0.0f);

            for (int i = 0; i < data.Length; ++i)
            {
                if (!chart.IsValid(seriesIndex, i)) continue;
                Vector2 p = pBuffer[i] * radius * (data[i].y + data[i].x);

                points[0] = new Vector2(p.x - radiusP, p.y - radiusP);
                points[1] = new Vector2(p.x - radiusP, p.y + radiusP);
                points[2] = new Vector2(p.x + radiusP, p.y + radiusP);
                points[3] = new Vector2(p.x + radiusP, p.y - radiusP);

                vh.AddVert(points[0], color, uvs[0]);
                vh.AddVert(points[1], color, uvs[1]);
                vh.AddVert(points[2], color, uvs[2]);
                vh.AddVert(points[3], color, uvs[3]);

                vh.AddTriangle(index, index + 1, index + 2);
                vh.AddTriangle(index + 2, index + 3, index);
                index += 4;
            }
        }
    }
}