using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChartUtil
{
    public class ChartGridGraphicRadialLine : ChartGraphic
    {
        public int side = 8;
        public float width = 1.0f;
        public float innerSize = 0.0f;
        public bool mid = false;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            if (side < 1) side = 1;
            innerSize = Mathf.Clamp01(innerSize);
            float radius = rectTransform.rect.size.x < rectTransform.rect.size.y ? rectTransform.rect.size.x * 0.5f : rectTransform.rect.size.y * 0.5f;
            float offset = radius * innerSize;
            float angleOffset = mid ? 360.0f / side * 0.5f : 0.0f;

            int index = 0;
            Vector2[] rect = new Vector2[4];
            rect[0] = new Vector2(-width * 0.5f, offset);
            rect[1] = new Vector2(-width * 0.5f, radius);
            rect[2] = new Vector2(width * 0.5f, radius);
            rect[3] = new Vector2(width * 0.5f, offset);
            Vector2[] points = new Vector2[4];
            Vector2[] cossin = GetCosSin(side, angleOffset, 360.0f, false);
            Vector2[] uvs = new Vector2[2];
            uvs[0] = new Vector2(0.0f, 0.0f);
            uvs[1] = new Vector2(1.0f, 0.0f);

            for (int i = 0; i < cossin.Length; ++i)
            {
                points[0] = RotateCW(rect[0], cossin[i]);
                points[1] = RotateCW(rect[1], cossin[i]);
                points[2] = RotateCW(rect[2], cossin[i]);
                points[3] = RotateCW(rect[3], cossin[i]);

                vh.AddVert(points[0], color, uvs[0]);
                vh.AddVert(points[1], color, uvs[0]);
                vh.AddVert(points[2], color, uvs[1]);
                vh.AddVert(points[3], color, uvs[1]);

                vh.AddTriangle(index, index + 1, index + 2);
                vh.AddTriangle(index + 2, index + 3, index);
                index += 4;
            }
        }
    }
}