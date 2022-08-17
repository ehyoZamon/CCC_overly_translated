using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ChartUtil
{
    public class ChartGridGraphicRect : ChartGraphic
    {
        public int num = 4;
        public float width = 1.0f;
        public bool inverted = false;
        public bool mid = false;
        public int skip = 0;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            if (num < 1) num = 1;

            Vector2 size = rectTransform.rect.size;
            Vector2 offset = -size * 0.5f;
            int n = mid ? num - 1 : num;

            if (inverted)
            {
                offset.y -= width * 0.5f;
                float spacing = size.y / num;
                if (mid) offset.y += spacing * 0.5f;

                int index = 0;
                Vector2[] points = new Vector2[4];
                for (int i = 0; i <= n; i += skip + 1)
                {
                    float pos = offset.y + spacing * i;
                    points[0] = new Vector2(offset.x, pos);
                    points[1] = new Vector2(offset.x, pos + width);
                    points[2] = new Vector2(-offset.x, pos + width);
                    points[3] = new Vector2(-offset.x, pos);

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
                offset.x -= width * 0.5f;
                float spacing = size.x / num;
                if (mid) offset.x += spacing * 0.5f;

                int index = 0;
                Vector2[] points = new Vector2[4];
                for (int i = 0; i <= n; i += skip + 1)
                {
                    float pos = offset.x + spacing * i;
                    points[0] = new Vector2(pos, offset.y);
                    points[1] = new Vector2(pos, -offset.y);
                    points[2] = new Vector2(pos + width, -offset.y);
                    points[3] = new Vector2(pos + width, offset.y);

                    vh.AddVert(points[0], color, Vector2.zero);
                    vh.AddVert(points[1], color, Vector2.zero);
                    vh.AddVert(points[2], color, Vector2.zero);
                    vh.AddVert(points[3], color, Vector2.zero);

                    vh.AddTriangle(index, index + 1, index + 2);
                    vh.AddTriangle(index + 2, index + 3, index);
                    index += 4;
                }
            }
        }
    }
}