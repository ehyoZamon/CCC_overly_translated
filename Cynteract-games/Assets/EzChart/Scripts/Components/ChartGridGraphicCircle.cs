using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChartUtil
{
    public class ChartGridGraphicCircle : ChartGraphic
    {
        public int side = 8;
        public int num = 4;
        public float width = 1.0f;
        public float innerSize = 0.0f;
        public bool isCircular = true;
        public bool mid = false;
        public bool drawLast = false;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            if (num < 1) num = 1;
            if (side < 3) side = 3;
            innerSize = Mathf.Clamp01(innerSize);
            float radius = rectTransform.rect.size.x < rectTransform.rect.size.y ? rectTransform.rect.size.x * 0.5f: rectTransform.rect.size.y * 0.5f;
            float offset = radius * innerSize;
            float spacing = (radius - offset) / num;
            float angleOffset = mid ? 360.0f / side * 0.5f : 0.0f;
            offset += width * 0.5f;

            int index = 0;
            Vector2[] points = new Vector2[4];
            Vector2[] cossin = isCircular ? CosSin : GetCosSin(side, angleOffset + 90.0f);
            Vector2[] uvs = new Vector2[2];
            uvs[0] = new Vector2(0.0f, 0.0f);
            uvs[1] = new Vector2(0.0f, 1.0f);

            int n = drawLast ? num : num - 1;
            for (int i = 1; i <= n; ++i)
            {
                float r = offset + spacing * i;
                for (int j = 0; j < cossin.Length - 1; ++j)
                {
                    points[0] = cossin[j] * (r - width);
                    points[1] = cossin[j] * (r);
                    points[2] = cossin[j + 1] * (r);
                    points[3] = cossin[j + 1] * (r - width);

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
}