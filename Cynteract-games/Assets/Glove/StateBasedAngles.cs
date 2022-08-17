using Sirenix.OdinInspector;
using System;
using UnityEngine;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities;
#endif

namespace Cynteract.CGlove
{
    [Serializable] 
    public class StateBasedAngles
    {
        public Vector2 
            runBootingMinMax=new Vector2(float.MaxValue, float.MinValue),
            running2MinMax = new Vector2(float.MaxValue, float.MinValue),
            running1MinMax = new Vector2(float.MaxValue, float.MinValue),
            runningMinMax = new Vector2(float.MaxValue, float.MinValue);
        public float angle;
        bool runBootingSet = false, running2Set = false, running1Set = false, runningSet = false;
        [Button]
        public void Reset()
        {
            runBootingMinMax = new Vector2(float.MaxValue, float.MinValue);
            running2MinMax = new Vector2(float.MaxValue, float.MinValue);
            running1MinMax = new Vector2(float.MaxValue, float.MinValue);
            runningMinMax = new Vector2(float.MaxValue, float.MinValue);

            runBootingSet = false;
            running2Set = false;
            running1Set = false;
            runningSet = false;
        }
        public void Evaluate(float xAngle, ImuState state)
        {
            angle = xAngle;
            switch (state)
            {
                case ImuState.Running:
                    runningMinMax.x = Mathf.Min(runningMinMax.x, xAngle);
                    runningMinMax.y = Mathf.Max(runningMinMax.y, xAngle);
                    runningSet = true;
                    goto case ImuState.Running1;
                case ImuState.Running1:
                    running1MinMax.x = Mathf.Min(running1MinMax.x, xAngle);
                    running1MinMax.y = Mathf.Max(running1MinMax.y, xAngle);
                    running1Set = true;
                    goto case ImuState.Running2;
                case ImuState.Running2:
                    running2MinMax.x = Mathf.Min(running2MinMax.x, xAngle);
                    running2MinMax.y = Mathf.Max(running2MinMax.y, xAngle);
                    running2Set = true;
                    goto case ImuState.RunBooting;
                case ImuState.RunBooting:
                    runBootingMinMax.x = Mathf.Min(runBootingMinMax.x, xAngle);
                    runBootingMinMax.y = Mathf.Max(runBootingMinMax.y, xAngle);
                    runBootingSet = true;

                    break;
                    
                default:
                    break;
            }
        }
        public float GetMin()
        {
            if (runningSet)
            {
                return runningMinMax.x;
            }
            else if (running1Set)
            {
                return running1MinMax.x;
            }
            else if (running2Set)
            {
                return running2MinMax.x;
            }
            else if (runBootingSet)
            {
                return runBootingMinMax.x;
            }
            return -1;
        }
        public float GetMax()
        {
            if (runningSet)
            {
                return runningMinMax.y;
            }
            else if (running1Set)
            {
                return running1MinMax.y;
            }
            else if(running2Set)
            {
                return running2MinMax.y;
            }
            else if (runBootingSet)
            {
                return runBootingMinMax.y;
            }
            return 1;
        }
        public float Value01(float angle)
        {
            return Mathf.InverseLerp(GetMin(), GetMax(), angle);
        }
    }
#if UNITY_EDITOR
    public class MyStructDrawer : OdinValueDrawer<StateBasedAngles>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            Rect rect = EditorGUILayout.GetControlRect();

            if (label != null)
            {
                rect = EditorGUI.PrefixLabel(rect, label);
            }

            StateBasedAngles value = this.ValueEntry.SmartValue;
            value.runBootingMinMax = EditorGUILayout.Vector2Field("runbootingMinMax", value.runBootingMinMax);
            value.running2MinMax = EditorGUILayout.Vector2Field("running2MinMax", value.running2MinMax);
            value.running1MinMax = EditorGUILayout.Vector2Field("running1MinMax", value.running1MinMax);
            value.runningMinMax = EditorGUILayout.Vector2Field("runningMinMax", value.runningMinMax);
            value.angle = EditorGUILayout.FloatField("angle", value.angle);
            if (GUILayout.Button("Reset"))
            {
                value.Reset();
            }

            GUIHelper.PushLabelWidth(40);
            GUIHelper.PopLabelWidth();

            float min = value.runBootingMinMax.x;
            float max = value.runBootingMinMax.y;

            float sum = 0;
            Color darkOrange = new Color(1, .5f, 0);
            Color lightOrange = new Color(1, .78f, 0);
            DrawRect(value.running2MinMax.x, darkOrange);
            DrawRect(value.running1MinMax.x, lightOrange);
            DrawRect(value.runningMinMax.x, Color.yellow);
            DrawRect(value.runningMinMax.y, Color.green);
            DrawRect(value.running1MinMax.y, Color.yellow);
            DrawRect(value.running2MinMax.y, lightOrange);
            DrawRect(max, darkOrange);

            var angle01 = Mathf.InverseLerp(min, max, value.angle);
            SirenixEditorGUI.DrawSolidRect(rect.SetX(rect.x + rect.width * angle01).SetWidth(rect.width * .01f), Color.white, false);
            this.ValueEntry.SmartValue = value;

            void DrawRect(float extremum, Color color)
            {
                float running2Min = Mathf.InverseLerp(min, max, extremum);
                SirenixEditorGUI.DrawSolidRect(rect.SetX(rect.x + rect.width * sum).SetWidth(rect.width * (running2Min - sum)).SetHeight(rect.height * .9f), color, false);
                sum = running2Min;
            }
        }
    }
#endif
}