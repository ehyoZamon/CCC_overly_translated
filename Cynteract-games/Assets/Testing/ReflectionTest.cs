using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
public class ParentReflectionTest : MonoBehaviour
{
    public bool boolValue;

}
public class ReflectionTest : ParentReflectionTest
{
    [Button]
    public void PrintFields()
    {
        BindingFlags bindingFlags = BindingFlags.Public |
                            BindingFlags.NonPublic |
                            BindingFlags.Instance |
                            BindingFlags.Static;

        foreach (FieldInfo field in typeof(ReflectionTest).GetFields(bindingFlags))
        {
            Debug.Log(field.Name);
        }    
    }

}
