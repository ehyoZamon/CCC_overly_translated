﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChartUtil
{
    public class RadarChartLine : ChartGraphic
    {
        public Vector2[] data = null; //ratio, starting
        public float width = 2.0f;
        public bool reverse = false;
        public int seriesIndex = 0;
        public RadarChart chart;
        public RadarChartPoint point;

        Vector2[] pBuffer = null;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            if (data == null || data.Length == 0 || chart == null) return;

            float halfWidth = width * 0.5f;
            float radius = rectTransform.rect.size.x < rectTransform.rect.size.y ? rectTransform.rect.size.x * 0.5f : rectTransform.rect.size.y * 0.5f;

            int index = 0;
            //float angleOffset = 360.0f / data.Length * 0.5f;
            //Vector2[] cossin = GetCosSin(data.Length, angleOffset, 360.0f, false);
            pBuffer = point.PointsBuffer;
            Vector2[] points = new Vector2[4];
            Vector2[] uvs = new Vector2[2];
            uvs[0] = new Vector2(0.0f, 0.0f);
            uvs[1] = new Vector2(1.0f, 0.0f);

            //Vector2 p = (reverse ? RotateCCW(Vector2.up, cossin[0]) : RotateCW(Vector2.up, cossin[0])) * radius * data[0];
            Vector2 p = pBuffer[0] * radius * (data[0].y + data[0].x);
            for (int i = 1; i < data.Length; ++i)
            {
                if (!chart.IsValid(seriesIndex, i)) continue;
                Vector2 lastPoint = p;
                //p = reverse ? RotateCCW(Vector2.up, cossin[i]) : RotateCW(Vector2.up, cossin[i]) * radius * data[i];
                p = pBuffer[i] * radius * (data[i].y + data[i].x);
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

            {
                if (!chart.IsValid(seriesIndex, 0)) return;
                Vector2 lastPoint = p;
                //p = (reverse ? RotateCCW(Vector2.up, cossin[0]) : RotateCW(Vector2.up, cossin[0])) * radius * data[0];
                p = pBuffer[0] * radius * (data[0].y + data[0].x);
                if (!chart.IsValid(seriesIndex, data.Length - 1)) return;

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

            pBuffer = null;
        }
    }
}