﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChartUtil
{
    public class RoseChartBar : ChartGraphic
    {
        public Vector2[] data = null; //ratio, starting
        public float offset = 0.0f;
        public float width = 5.0f;
        public float innerSize = 0.0f;
        public float innerExtend = 0.0f;
        public bool reverse = false;
        public int seriesIndex = 0;
        public RoseChart chart;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            if (data == null || data.Length == 0 || chart == null) return;

            innerSize = Mathf.Clamp01(innerSize);
            float radius = rectTransform.rect.size.x < rectTransform.rect.size.y ? rectTransform.rect.size.x * 0.5f : rectTransform.rect.size.y * 0.5f;
            float radiusInner = radius * innerSize;
            float range = radius - radiusInner;

            int barSide = Mathf.RoundToInt(width / 360.0f * CosSin.Length);
            Vector2[] cossinBar = GetCosSin(barSide, 90.0f - width * 0.5f, width, true);

            float direction = reverse ? -360.0f : 360.0f;
            float angleOffset = direction / data.Length * 0.5f + offset;
            int index = 0;
            Vector2[] cossin = GetCosSin(data.Length, angleOffset, direction, false);
            Vector2[] points = new Vector2[4];
            Vector2[] uvs = new Vector2[2];
            uvs[0] = new Vector2(0.0f, 0.0f);
            uvs[1] = new Vector2(0.0f, 1.0f);

            if (!chart.chartOptions.plotOptions.roseChartOption.colorByCategories)
            {
                for (int i = 0; i < data.Length; ++i)
                {
                    if (!chart.IsValid(seriesIndex, i)) continue;
                    float rStart = radiusInner + range * data[i].y;
                    float r = rStart + range * data[i].x;
                    rStart -= innerExtend;
                    if (rStart < 0.0f) rStart = 0.0f;

                    uvs[0].y = 1.0f - (data[i].y + data[i].x);
                    if (uvs[0].y < 0.5f) uvs[0].y = 0.5f;

                    for (int j = 0; j < cossinBar.Length - 1; ++j)
                    {
                        points[0] = cossinBar[j] * rStart;
                        points[1] = cossinBar[j] * r;
                        points[2] = cossinBar[j + 1] * r;
                        points[3] = cossinBar[j + 1] * rStart;

                        points[0] = RotateCW(points[0], cossin[i]);
                        points[1] = RotateCW(points[1], cossin[i]);
                        points[2] = RotateCW(points[2], cossin[i]);
                        points[3] = RotateCW(points[3], cossin[i]);

                        vh.AddVert(points[0], color, uvs[0]);
                        vh.AddVert(points[1], color, uvs[1]);
                        vh.AddVert(points[2], color, uvs[1]);
                        vh.AddVert(points[3], color, uvs[0]);

                        vh.AddTriangle(index, index + 1, index + 2);
                        vh.AddTriangle(index + 2, index + 3, index);
                        index += 4;
                    }
                }
            }
            else
            {
                Color[] colors = chart.chartOptions.plotOptions.dataColor;
                for (int i = 0; i < data.Length; ++i)
                {
                    if (!chart.IsValid(seriesIndex, i)) continue;
                    float rStart = radiusInner + range * data[i].y;
                    float r = rStart + range * data[i].x;
                    rStart -= innerExtend;
                    if (rStart < 0.0f) rStart = 0.0f;

                    uvs[0].y = 1.0f - (data[i].y + data[i].x);
                    if (uvs[0].y < 0.5f) uvs[0].y = 0.5f;

                    for (int j = 0; j < cossinBar.Length - 1; ++j)
                    {
                        points[0] = cossinBar[j] * rStart;
                        points[1] = cossinBar[j] * r;
                        points[2] = cossinBar[j + 1] * r;
                        points[3] = cossinBar[j + 1] * rStart;

                        points[0] = RotateCW(points[0], cossin[i]);
                        points[1] = RotateCW(points[1], cossin[i]);
                        points[2] = RotateCW(points[2], cossin[i]);
                        points[3] = RotateCW(points[3], cossin[i]);

                        vh.AddVert(points[0], colors[i % colors.Length], uvs[0]);
                        vh.AddVert(points[1], colors[i % colors.Length], uvs[1]);
                        vh.AddVert(points[2], colors[i % colors.Length], uvs[1]);
                        vh.AddVert(points[3], colors[i % colors.Length], uvs[0]);

                        vh.AddTriangle(index, index + 1, index + 2);
                        vh.AddTriangle(index + 2, index + 3, index);
                        index += 4;
                    }
                }
            }
        }
    }
}