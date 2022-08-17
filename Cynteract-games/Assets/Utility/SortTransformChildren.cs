using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SortTransformChildren 
{
	public static void SortByName(Transform parent)
	{
		Sort(parent, (Transform t1, Transform t2) => { return t1.name.CompareTo(t2.name); });
	}
	public static void Sort<Component,FieldType>(Transform parent,Func<Component, FieldType> selector, bool asc=true) where FieldType:IComparable
    {
		Sort(parent, (Transform t1, Transform t2) => {
			var c1 = t1.GetComponent<Component>();
			var c2 = t2.GetComponent<Component>();
            if (asc)
            {
				return selector(c1).CompareTo(selector(c2));
            }
			return selector(c2).CompareTo(selector(c1));
		});
    }
	public static void Sort<Component>(Transform parent, Comparison<Component> comparison, bool asc = true)
	{
		Sort(parent, (Transform t1, Transform t2) => {
			var c1 = t1.GetComponent<Component>();
			var c2 = t2.GetComponent<Component>();
            if (asc) return comparison(c1,c2);
			return comparison(c2, c1);
		});
	}
	public static void Sort<Component>(Transform parent, bool asc = true) where Component:IComparable
	{
		Sort(parent, (Transform t1, Transform t2) => {
			var c1 = t1.GetComponent<Component>();
			var c2 = t2.GetComponent<Component>();

			if (asc)  return c1.CompareTo(c2);
			return c2.CompareTo(c1);
		});
	}
	public static void Sort(Transform parent, Comparison<Transform> comparison)
    {
		List<Transform> children = new List<Transform>();
		for (int i = parent.childCount - 1; i >= 0; i--)
		{
			Transform child = parent.GetChild(i);
			children.Add(child);
			child.SetParent(null, false);
		}
		children.Sort(Comparer<Transform>.Create(comparison));
		foreach (Transform child in children)
		{
			child.SetParent(parent, false);
		}
	}
}
