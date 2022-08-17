#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TextToTextmeshPro : OdinEditorWindow
{

    public Text selectedTextMesh;
    public TMPro.TMP_FontAsset fontAsset; 
    [MenuItem("Tools/Text To TextmeshPro")]
    private static void OpenWindow()
    {
        var window = GetWindow<TextToTextmeshPro>();

        // Nifty little trick to quickly position the window in the middle of the editor.
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(300, 300);
        window.Init();
    }

    public void Init()
    {

    }
    [Button]
    public void ChangeToTextMeshPro()
    {
        if (selectedTextMesh)
        {
            var fontSize = selectedTextMesh.fontSize;
            var color = selectedTextMesh.color;
            var text = selectedTextMesh.text;
            var gO = selectedTextMesh.gameObject;
            var allignment = selectedTextMesh.alignment;
            DestroyImmediate(selectedTextMesh);
            var tmpro =gO.AddComponent<TMPro.TextMeshProUGUI>();
            tmpro.fontSize = fontSize;
            tmpro.color = color;
            tmpro.alignment = ToTmproAllignment(allignment);
            tmpro.text = text;
            if (fontAsset!=null)
            {
                tmpro.font = fontAsset;
            }
        }
    }

    private TMPro.TextAlignmentOptions ToTmproAllignment(TextAnchor allignment)
    {
        switch (allignment)
        {
            case TextAnchor.UpperLeft:
                return TMPro.TextAlignmentOptions.TopLeft;
            case TextAnchor.UpperCenter:
                return TMPro.TextAlignmentOptions.TopGeoAligned;
            case TextAnchor.UpperRight:
                return TMPro.TextAlignmentOptions.TopRight;
            case TextAnchor.MiddleLeft:
                return TMPro.TextAlignmentOptions.Left;
            case TextAnchor.MiddleCenter:
                return TMPro.TextAlignmentOptions.MidlineGeoAligned;
            case TextAnchor.MiddleRight:
                return TMPro.TextAlignmentOptions.MidlineRight;
            case TextAnchor.LowerLeft:
                return TMPro.TextAlignmentOptions.BottomLeft;
            case TextAnchor.LowerCenter:
                return TMPro.TextAlignmentOptions.BottomGeoAligned;
            case TextAnchor.LowerRight:
                return TMPro.TextAlignmentOptions.BottomRight;
            default:
                throw new Exception("Not Valid");
                
        }
    }

    private void OnInspectorUpdate()
    {
        selectedTextMesh = Selection.activeTransform.gameObject.GetComponent<Text>();
    }
}

#endif