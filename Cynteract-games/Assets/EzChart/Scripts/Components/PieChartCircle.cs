﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChartUtil
{
    public class PieChartCircle : ChartGraphic
    {
        public float angle;
        public float center;
        public float innerSize = 0.0f;
        public float separation = 0.0f;
        [HideInInspector] public Vector2 dir;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            if (angle < 0.0f) return;
            if (separation < 0.0f) separation = 0.0f;

            innerSize = Mathf.Clamp01(innerSize);
            float radius = rectTransform.rect.size.x < rectTransform.rect.size.y ? rectTransform.rect.size.x * 0.5f : rectTransform.rect.size.y * 0.5f;
            float radiusInner = radius * innerSize;
            if (separation > radius) return;

            int index = 0;
            Vector2[] points = new Vector2[4];
            Vector2[] uvs = new Vector2[2];
            uvs[0] = new Vector2(0.0f, 0.0f);
            uvs[1] = new Vector2(0.0f, 1.0f);

            //dir = new Vector2(Mathf.Cos(center * Mathf.Deg2Rad), Mathf.Sin(center * Mathf.Deg2Rad));

            //outer
            float angleSep = Mathf.Asin(separation / radius) * Mathf.Rad2Deg * 2;
            if (angle < angleSep) return;
            float anglePie = angle - angleSep;
            int side = Mathf.RoundToInt(anglePie / 360.0f * CosSin.Length);
            Vector2[] cossinPie = GetCosSin(side, 90.0f - anglePie * 0.5f, anglePie, true);

            //inner
            Vector2[] cossinPieInner = null;
            float radiusSep = angle > 180.0f ? separation : separation / Mathf.Sin(angle * 0.5f * Mathf.Deg2Rad);
            if (radiusInner > radiusSep)
            {
                float angleSepInner = Mathf.Asin(separation / radiusInner) * Mathf.Rad2Deg * 2;
                float anglePieInner = angle - angleSepInner;
                cossinPieInner = GetCosSin(side, 90.0f - anglePieInner * 0.5f, anglePieInner, true);
            }
            else
            {
                radiusInner = radiusSep;
                if (angle > 180.0f)
                {
                    float angleInner = angle - 180.0f;
                    cossinPieInner = GetCosSin(side, 90.0f - angleInner * 0.5f, angleInner, true);
                }
                else
                {
                    cossinPieInner = new Vector2[side + 1];
                    for (int j = 0; j < cossinPieInner.Length; ++j) cossinPieInner[j] = new Vector2(0.0f, 1.0f);
                }
            }

            for (int j = 0; j < cossinPie.Length - 1; ++j)
            {
                points[0] = cossinPieInner[j] * radiusInner;
                points[1] = cossinPie[j] * radius;
                points[2] = cossinPie[j + 1] * radius;
                points[3] = cossinPieInner[j + 1] * radiusInner;

                points[0] = RotateCW(points[0], dir);
                points[1] = RotateCW(points[1], dir);
                points[2] = RotateCW(points[2], dir);
                points[3] = RotateCW(points[3], dir);

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
}