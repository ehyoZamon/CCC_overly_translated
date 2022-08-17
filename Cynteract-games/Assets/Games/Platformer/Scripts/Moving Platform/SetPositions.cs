#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Cynteract.Platformer.MovingPlatform))]
public class SetPositions : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Cynteract.Platformer.MovingPlatform obj = (Cynteract.Platformer.MovingPlatform)target;
        if (GUILayout.Button("Set start element")) 
        {
            obj.SetStart();
        } 
        else if (GUILayout.Button("Set end element"))
        {
            obj.SetEnd();
        }
    }
}
#endif