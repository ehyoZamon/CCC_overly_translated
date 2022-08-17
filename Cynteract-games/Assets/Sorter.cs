using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Sorter : MonoBehaviour
{
    [ReadOnly]
    public bool asc = true;
    public bool flipped, startAsc;
    private Transform parent;
    private dynamic selector;
    public Image  upArrow,downArrow;
    public Button button;
    Sorter[] sorters;
    public void Init<C>(Transform parent, Func<C, IComparable> selector, Sorter[] sorters) where C:Component
    {
        this.parent = parent;
        this.selector = selector;
        button.onClick.AddListener(Toggle);
        asc = startAsc;

        this.sorters = sorters.Where(x => x != this).ToArray();
    }
    public void Toggle()
    {
        Sort(parent, selector);
        upArrow.gameObject.SetActive(asc);
        downArrow.gameObject.SetActive(!asc);
        foreach (var item in sorters)
        {
            item.Deactivate();
        }
        asc = !asc;
    }
    public void Deactivate()
    {
        upArrow.gameObject.SetActive(false);
        downArrow.gameObject.SetActive(false);
        asc = startAsc;
    }

    public void Sort<Component, FieldType>(Transform parent, Func<Component, FieldType> selector) where FieldType : IComparable
    {
        SortTransformChildren.Sort(parent, selector, asc^flipped);
    }


}
