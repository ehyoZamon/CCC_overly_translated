#if UNITY_EDITOR
using Cynteract.CGlove;
using Cynteract.CGlove.LinearCalibration;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using UnityEngine.Scripting;


public class InterpolationWindow : OdinMenuEditorWindow
{
    [Serializable]
    public class Hand

    {
        [ReadOnly]
        public Side side;
        [Serializable]
        public class FingerCollection
        {
            public Finger[] fingers ={
            new Finger("Thumb"),
            new Finger("Index"),
            new Finger("Middle"),
            new Finger("Ring"),
            new Finger("Pinky"),
            };
        }
        [ReadOnly]
        public string name;


        public FingerCollection fingerCollection;
        private OdinMenuTree tree;

        [Preserve]
        [JsonConstructor]
        public Hand()
        {

        }
        public Hand(string name, Side side)
        {
            this.side = side;
            this.name = name;
            fingerCollection = new FingerCollection();
        }
        public Hand(string name, Side side, ref OdinMenuTree tree) : this(name, side)
        {
            this.tree = tree;
        }

        public void AddToOdinMenuTree(ref OdinMenuTree tree)
        {
            tree.Add(name, this);
            tree.Add(name + "/" + "Fingers/", fingerCollection);

            foreach (var finger in fingerCollection.fingers)
            {
                finger.AddToOdinMenuTree(name + "/" + "Fingers/" + finger.name, ref tree);
            }

        }
        public void Init()
        {
            foreach (var finger in fingerCollection.fingers)
            {
                if (finger.fingerParts == null)
                {

                    finger.fingerParts = new InterpolationManager.FingerSelection[2];
                }
                finger.hand = this;
                for (int i = 0; i < finger.fingerParts.Length; i++)
                {
                    if (finger.fingerParts[i] == null)
                    {
                        finger.fingerParts[i] = new InterpolationManager.FingerSelection();
                    }

                       finger.fingerParts[i].finger = finger;
                    
                    if (finger.fingerParts[i].fingerAngles == null)
                    {
                        finger.fingerParts[i].fingerAngles = new InterpolationManager.FingerAngle[2];
                    }
                    for (int j = 0; j < finger.fingerParts[i].fingerAngles.Length; j++)
                    {
                        if (finger.fingerParts[i].fingerAngles[j] == null)
                        {
                            finger.fingerParts[i].fingerAngles[j] = new InterpolationManager.FingerAngle();
                        }
                        finger.fingerParts[i].fingerAngles[j].fingerPart = finger.fingerParts[i];
                    }


                }

            }
        }
        [ButtonGroup("FileManagement")]
        [Button(ButtonSizes.Large)]
        public void New()
        {
            fingerCollection = new FingerCollection();
            Init();
            AddToOdinMenuTree(ref tree);
            tree.SortMenuItemsByName();
        }
        [ButtonGroup("FileManagement")]
        [Button(ButtonSizes.Large)]
        public void Load()
        {
            try
            {
                var data = JsonFileManager.Load<Hand>("Interpolation" + name, JsonFileManager.Type.Newtonsoft);
                this.fingerCollection = data.fingerCollection;
                AddToOdinMenuTree(ref tree);
                tree.SortMenuItemsByName();
            }
            catch (Exception e)
            {

                Debug.LogWarning(e);
            }

        }
        [ButtonGroup("FileManagement")]
        [Button(ButtonSizes.Large)]
        public void Save()
        {
            JsonFileManager.Save(this, "Interpolation" + name, JsonFileManager.Type.Newtonsoft);
        }
        [ButtonGroup("Set")]
        [Button(ButtonSizes.Large)]
        public void SetStandardValues()
        {

            Set();
            switch (side)
            {
                case Side.NotSet:
                    break;
                case Side.Left:
                        InterpolationManager.left.SetStandardValues();
                    break;
                case Side.Right:

                        InterpolationManager.right.SetStandardValues();
                    break;
                default:
                    break;
            }
        }

        [Button(ButtonSizes.Large)]
        public void GenerateGrid()
        {
            Set();
            switch (side)
            {
                case Side.NotSet:
                    break;
                case Side.Left:
                    InterpolationManager.left.GenerateGrids();
                    break;
                case Side.Right:

                    InterpolationManager.right.GenerateGrids();
                    break;
                default:
                    break;
            }
        }
        public void Set()
        {
            switch (side)
            {
                case Side.NotSet:
                    break;
                case Side.Left:
                    if (InterpolationManager.left == null)
                    {
                        InterpolationManager.left = new InterpolationManager.Hand();
                    }
                    InterpolationManager.left.side = side;
                    InterpolationManager.left.fingerParts = GetAllFingerParts();
                    InterpolationManager.left.SetDimensions();
                    break;
                case Side.Right:
                    if (InterpolationManager.right == null)
                    {
                        InterpolationManager.right = new InterpolationManager.Hand();
                    }
                    InterpolationManager.right.side = side;
                    InterpolationManager.right.fingerParts = GetAllFingerParts();
                    InterpolationManager.right.SetDimensions();

                    break;
                default:
                    break;
            }
        }

        private InterpolationManager.FingerSelection[] GetAllFingerParts()
        {
            List<InterpolationManager.FingerSelection> parts = new List<InterpolationManager.FingerSelection>();

            List<Fingerpart> fingerParts = new List<Fingerpart>();
            foreach (var item in fingerCollection.fingers)
            {
                foreach (var part in item.fingerParts)
                {
                    if (!fingerParts.Contains(part.fingerpart))
                    {
                        fingerParts.Add(part.fingerpart);
                        parts.Add(part);
                    }

                }
            }
            return parts.ToArray();
        }
    }
    [Serializable]
    public class Finger
    {
        [ReadOnly]
        public string name;
        public InterpolationManager.FingerSelection[] fingerParts;
        [NonSerialized]
        public Hand hand;

        public Finger(string name)
        {
            this.name = name;
        }

        public void AddToOdinMenuTree(string name, ref OdinMenuTree tree)
        {
            tree.Add(name, this);
            tree.Add(name + "/Base", fingerParts[0]);
            tree.Add(name + "/Center", fingerParts[1]);

        }

    }

    public Hand leftHand, rightHand;
   // [MenuItem("Interpolation/Window")]
    private static void OpenWindow()
    {
        var window = GetWindow<InterpolationWindow>()  ;
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);

    }
    protected override void OnGUI()
    {
        EditorGUI.BeginChangeCheck();

        base.OnGUI();
        // End the code block and update the label if a change occurred
        if (EditorGUI.EndChangeCheck())
        {
            leftHand.Set();
            rightHand.Set();
        }
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        var tree = new OdinMenuTree(supportsMultiSelect: true);
        leftHand = new Hand("Left", Side.Left, ref tree);
        rightHand = new Hand("Right", Side.Right, ref tree);
        tree.SortMenuItemsByName();
        leftHand.Init();
        rightHand.Init();
        leftHand.Load();
        rightHand.Load();
        leftHand.AddToOdinMenuTree(ref tree);
        rightHand.AddToOdinMenuTree(ref tree);
        return tree;
    }





}
#endif