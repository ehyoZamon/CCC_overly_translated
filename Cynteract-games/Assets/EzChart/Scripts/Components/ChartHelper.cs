using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if CHART_TMPRO
using TMPro;
#endif

namespace ChartUtil
{
    public class ChartHelper
    {
#if CHART_TMPRO
        static Dictionary<TextAnchor, TextAlignmentOptions> TMProAlignmentDict = new Dictionary<TextAnchor, TextAlignmentOptions>()
        {
            {TextAnchor.LowerLeft, TextAlignmentOptions.BottomLeft },
            {TextAnchor.LowerCenter, TextAlignmentOptions.Bottom },
            {TextAnchor.LowerRight, TextAlignmentOptions.BottomRight },
            {TextAnchor.MiddleLeft, TextAlignmentOptions.MidlineLeft },
            {TextAnchor.MiddleCenter, TextAlignmentOptions.Midline },
            {TextAnchor.MiddleRight, TextAlignmentOptions.MidlineRight },
            {TextAnchor.UpperLeft, TextAlignmentOptions.TopLeft },
            {TextAnchor.UpperCenter, TextAlignmentOptions.Top },
            {TextAnchor.UpperRight, TextAlignmentOptions.TopRight },
        };
#endif

        public static void Destroy(Object obj)
        {
#if UNITY_EDITOR
            GameObject.DestroyImmediate(obj);
#else
            GameObject.Destroy(obj);
#endif
        }

        public static void Clear(Transform trans)
        {
            for (int i = trans.childCount - 1; i >= 0; --i)
            {
                Destroy(trans.GetChild(i).gameObject);
            }
        }

        public static GameObject CreateDummyObject(Transform parent)
        {
            GameObject go = new GameObject("Dummy");
            go.SetActive(false);
            go.transform.SetParent(parent, false);
            return go;
        }

        public static Image CreateImage(string name, Transform parent, bool raycast = false, bool setMax = false)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.AddComponent<RectTransform>();
            Image img = go.AddComponent<Image>();
            img.raycastTarget = raycast;
            if (setMax) SetRectTransformMax(img.rectTransform);
            return img;
        }

        public static RectTransform CreateEmptyRect(string name, Transform parent, bool setMax = false)
        {
            GameObject go = new GameObject(name);
            go.transform.SetParent(parent, false);
            RectTransform t = go.AddComponent<RectTransform>();
            if (setMax) SetRectTransformMax(t);
            return t;
        }

        public static void SetRectTransformMax(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
        }

        public static HorizontalLayoutGroup AddHorizontalLayout(GameObject go, bool controlWidth = true, bool controlHeight = true, int alignment = 1)
        {
            HorizontalLayoutGroup horiLayout = go.gameObject.AddComponent<HorizontalLayoutGroup>();
            horiLayout.childControlWidth = controlWidth;
            horiLayout.childControlHeight = controlHeight;
            horiLayout.childForceExpandWidth = controlWidth;
            horiLayout.childForceExpandHeight = controlHeight;
            horiLayout.childAlignment = (TextAnchor)(alignment + 3);
            return horiLayout;
        }

        public static VerticalLayoutGroup AddVerticalLayout(GameObject go, bool controlWidth = true, bool controlHeight = true, int alignment = 1)
        {
            VerticalLayoutGroup vertLayout = go.gameObject.AddComponent<VerticalLayoutGroup>();
            vertLayout.childControlWidth = controlWidth;
            vertLayout.childControlHeight = controlHeight;
            vertLayout.childForceExpandWidth = controlWidth;
            vertLayout.childForceExpandHeight = controlHeight;
            vertLayout.childAlignment = (TextAnchor)(alignment * 3 + 1);
            return vertLayout;
        }

#if CHART_TMPRO
        public static TextMeshProUGUI CreateText(string name, Transform parent, ChartOptions.ChartTextOptions option, TMP_FontAsset generalFont, TextAnchor anchor = TextAnchor.MiddleCenter, bool setMax = false)
        {
            TextMeshProUGUI t = null;
            if (option.customizedText == null)
            {
                GameObject go = new GameObject(name);
                go.transform.SetParent(parent, false);
                go.AddComponent<RectTransform>();
                t = go.AddComponent<TextMeshProUGUI>();
                t.raycastTarget = false;
                t.enableWordWrapping = false;
                t.overflowMode = TextOverflowModes.Overflow;
                t.alignment = TMProAlignmentDict[anchor];
                t.color = option.color;
                t.font = option.font == null ? generalFont : option.font;
                t.fontSize = option.fontSize;
                if (setMax) SetRectTransformMax(t.rectTransform);
            }
            else
            {
                t = GameObject.Instantiate(option.customizedText, parent);
                t.gameObject.name = name;
                t.gameObject.SetActive(true);
                t.raycastTarget = false;
                t.enableWordWrapping = false;
                t.overflowMode = TextOverflowModes.Overflow;
                t.alignment = TMProAlignmentDict[anchor];
                t.rectTransform.localPosition = Vector3.zero;
                t.rectTransform.localRotation = Quaternion.identity;
                t.rectTransform.localScale = Vector3.one;
                t.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                if (setMax) SetRectTransformMax(t.rectTransform);
                else
                {
                    t.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    t.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                }
            }
            return t;
        }

        public static void TruncateText(TextMeshProUGUI t, float maxWidth)
        {float w = t.fontSize * 2;
            var info = t.textInfo.characterInfo;
            for (int i = 0; i < info.Length; ++i)
            {
                if (t.GetPreferredValues(t.text.Substring(0, i)).x + w > maxWidth)
                {
                    t.text = t.text.Substring(0, i) + "...";
                    break;
                }
            }
        }

        public static TextAlignmentOptions ConvertAlignment(TextAnchor anchor)
        {
            return TMProAlignmentDict[anchor];
        }

        public static TextMeshProUGUI GetTextComponent(GameObject go)
        {
            return go.GetComponent<TextMeshProUGUI>();
        }
#else
        public static Text CreateText(string name, Transform parent, ChartOptions.ChartTextOptions option, Font generalFont, TextAnchor anchor = TextAnchor.MiddleCenter, bool setMax = false)
        {
            Text t = null;
            if (option.customizedText == null)
            {
                GameObject go = new GameObject(name);
                go.transform.SetParent(parent, false);
                go.AddComponent<RectTransform>();
                t = go.AddComponent<Text>();
                t.raycastTarget = false;
                t.horizontalOverflow = HorizontalWrapMode.Overflow;
                t.verticalOverflow = VerticalWrapMode.Overflow;
                t.alignment = anchor;
                t.color = option.color;
                t.font = option.font == null ? generalFont : option.font;
                t.fontSize = option.fontSize;
                if (setMax) SetRectTransformMax(t.rectTransform);
            }
            else
            {
                t = GameObject.Instantiate(option.customizedText, parent);
                t.gameObject.name = name;
                t.gameObject.SetActive(true);
                t.raycastTarget = false;
                t.horizontalOverflow = HorizontalWrapMode.Overflow;
                t.verticalOverflow = VerticalWrapMode.Overflow;
                t.alignment = anchor;
                t.rectTransform.localPosition = Vector3.zero;
                t.rectTransform.localRotation = Quaternion.identity;
                t.rectTransform.localScale = Vector3.one;
                t.rectTransform.pivot = new Vector2(0.5f, 0.5f);
                if (setMax) SetRectTransformMax(t.rectTransform);
                else
                {
                    t.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    t.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                }
            }
            return t;
        }

        public static float FindTextWidth(Text t)
        {
            float width = 0.0f;
            t.font.RequestCharactersInTexture(t.text, t.fontSize);
            CharacterInfo charInfo;
            if (t.font.dynamic)
            {
                for (int i = 0; i < t.text.Length; ++i)
                {
                    t.font.GetCharacterInfo(t.text[i], out charInfo, t.fontSize);
                    width += charInfo.advance;
                }
            }
            else
            {
                for (int i = 0; i < t.text.Length; ++i)
                {
                    t.font.GetCharacterInfo(t.text[i], out charInfo, t.font.fontSize);
                    width += charInfo.advance * ((float)t.fontSize / t.font.fontSize);
                }
            }
            return width;
        }

        public static void TruncateText(Text t, float maxWidth)
        {
            float width = 0.0f;
            t.font.RequestCharactersInTexture(t.text, t.fontSize);
            CharacterInfo charInfo;
            float w = t.fontSize * 2;
            if (t.font.dynamic)
            {
                for (int i = 0; i < t.text.Length - 3; ++i)
                {
                    if (width + w > maxWidth)
                    {
                        t.text = t.text.Substring(0, i) + "...";
                        break;
                    }
                    t.font.GetCharacterInfo(t.text[i], out charInfo, t.fontSize);
                    width += charInfo.advance;
                }
            }
            else
            {
                for (int i = 0; i < t.text.Length - 3; ++i)
                {
                    if (width + w > maxWidth)
                    {
                        t.text = t.text.Substring(0, i) + "...";
                        break;
                    }
                    t.font.GetCharacterInfo(t.text[i], out charInfo, t.font.fontSize);
                    width += charInfo.advance * ((float)t.fontSize / t.font.fontSize);
                }
            }
        }

        public static TextAnchor ConvertAlignment(TextAnchor anchor)
        {
            return anchor;
        }

        public static Text GetTextComponent(GameObject go)
        {
            return go.GetComponent<Text>();
        }
#endif

        public static string FindFloatDisplayFormat(float value)
        {
            string format;
            value = Mathf.Abs(value);
            if (value >= 100.0f) format = "N0";
            else if (value >= 10.0f) format = "N1";
            else if (value >= 1.0f) format = "N2";
            else format = "N" + (FindFloatDisplayPrecision(value) + 2).ToString();
            return format;
        }

        public static string FloatToString(float value)
        {
            return value.ToString(FindFloatDisplayFormat(value), System.Globalization.CultureInfo.InvariantCulture);
        }

        public static int FindFloatDisplayPrecision(float f)
        {
            if (f == 0.0f) return 0;

            string s = f.ToString("f5");
            int i;
            for (i = 2; i < s.Length; ++i)
            {
                if (s[i] != '0') break;
            }

            return i - 1;
        }

        public static int FindDoubleDisplayPrecision(double f)
        {
            if (f == 0.0f) return 0;

            string s = f.ToString("f10");
            int i;
            for (i = 2; i < s.Length; ++i)
            {
                if (s[i] != '0') break;
            }

            return i - 1;
        }

        public static int FindIntegerLength(int i)
        {
            return i.ToString().Length;
        }

        public static float FindMaxValue(ChartData data)
        {
            float max = float.MinValue;
            for (int i = 0; i < data.series.Count; ++i)
            {
                if (data.series[i].show)
                {
                    foreach (var v in data.series[i].data)
                    {
                        if (v.value > max) max = v.value;
                    }
                }
            }
            return max;
        }

        public static float FindMinValue(ChartData data)
        {
            float min = float.MaxValue;
            for (int i = 0; i < data.series.Count; ++i)
            {
                if (data.series[i].show)
                {
                    foreach (var v in data.series[i].data)
                    {
                        if (v.value < min) min = v.value;
                    }
                }
            }
            return min;
        }

        public static void FindMinMaxValue(ChartData data, out float minValue, out float maxValue)
        {
            minValue = float.MaxValue;
            maxValue = float.MinValue;
            for (int i = 0; i < data.series.Count; ++i)
            {
                if (data.series[i].show)
                {
                    foreach (var v in data.series[i].data)
                    {
                        if (v.show)
                        {
                            if (v.value < minValue) minValue = v.value;
                            if (v.value > maxValue) maxValue = v.value;
                        }
                    }
                }
            }
        }

        public static float GetPositiveSumBySeries(ChartData data, int index)
        {
            if (index >= data.series.Count) return 0;
            float sum = 0;
            for (int i = 0; i < data.series[index].data.Count; ++i)
            {
                if (data.series[index].data[i].show && data.series[index].data[i].value >= 0.0f) sum += data.series[index].data[i].value;
            }
            return sum;
        }

        public static float GetPositiveSumByCategory(ChartData data, int index)
        {
            float sum = 0;
            for (int i = 0; i < data.series.Count; ++i)
            {
                if (index >= data.series[i].data.Count || !data.series[i].show) continue;
                if (data.series[i].data[index].show && data.series[i].data[index].value >= 0.0f) sum += data.series[i].data[index].value;
            }
            return sum;
        }

        public static float GetNegativeSumBySeries(ChartData data, int index)
        {
            if (index >= data.series.Count) return 0;
            float sum = 0;
            for (int i = 0; i < data.series[index].data.Count; ++i)
            {
                if (data.series[index].data[i].show && data.series[index].data[i].value < 0.0f) sum += data.series[index].data[i].value;
            }
            return sum;
        }

        public static float GetNegativeSumByCategory(ChartData data, int index)
        {
            float sum = 0;
            for (int i = 0; i < data.series.Count; ++i)
            {
                if (index >= data.series[i].data.Count || !data.series[i].show) continue;
                if (data.series[i].data[index].show && data.series[i].data[index].value < 0.0f) sum += data.series[i].data[index].value;
            }
            return sum;
        }

        public static void GetSumBySeries(ChartData data, int index, out float pSum, out float nSum)
        {
            pSum = 0.0f;
            nSum = 0.0f;
            if (index < data.series.Count) return;
            for (int i = 0; i < data.series[index].data.Count; ++i)
            {
                if (data.series[index].data[i].show)
                {
                    if (data.series[index].data[i].value >= 0.0f) pSum += data.series[index].data[i].value;
                    else nSum += data.series[index].data[i].value;
                }
            }
        }

        public static void GetSumByCategory(ChartData data, int index, out float pSum, out float nSum)
        {
            pSum = 0.0f;
            nSum = 0.0f;
            for (int i = 0; i < data.series.Count; ++i)
            {
                if (index >= data.series[i].data.Count || !data.series[i].show) continue;
                if (data.series[i].data[index].show)
                {
                    if (data.series[i].data[index].value >= 0.0f) pSum += data.series[i].data[index].value;
                    else nSum += data.series[i].data[index].value;
                }
            }
        }

        public static void FindRange(bool zeroBased, int minDivision, float minValue, float maxValue, out float rangeMin, out float rangeMax, out float stepSize, out int numberOfSteps)
        {
            if (minValue == 0.0f && maxValue == 0.0f) { rangeMin = 0.0f; rangeMax = 1.0f; stepSize = 1.0f; numberOfSteps = 1; return; }
            if (zeroBased) { if (minValue > 0.0f) minValue = 0.0f; if (maxValue < 0.0f) maxValue = 0.0f; }

            numberOfSteps = minDivision >= 1 ? minDivision : 1;
            stepSize = (maxValue - minValue) / numberOfSteps;
            if (stepSize >= 1.0f)
            {
                int i = (int)stepSize;
                int l = FindIntegerLength(i);
                int unit = (int)Mathf.Pow(10, l - 1);
                int r = i % unit;
                i = i - r;
                if (r > (unit / 2)) i += unit;
                stepSize = i;
            }
            else
            {
                float l = Mathf.Pow(10, FindFloatDisplayPrecision(stepSize));
                stepSize = Mathf.Floor(stepSize * l) / l;
            }

            int minStep = Mathf.FloorToInt(minValue / stepSize);
            int maxStep = Mathf.CeilToInt(maxValue / stepSize);
            rangeMin = minStep * stepSize;
            rangeMax = maxStep * stepSize;
            numberOfSteps = maxStep - minStep;
        }
    }
}
