using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
[ExecuteInEditMode]
public class RelativeSize : UIBehaviour, ILayoutSelfController, ILayoutController
{
    [NonSerialized]
    RectTransform rectTransform;
    private DrivenRectTransformTracker m_Tracker;
    [OnValueChanged("UpdateSize")]
    [Range(0, 1)]
    public float x = 1, y = 1;
    private void OnWillRenderObject()
    {
        UpdateSize();
    }
    private void Update()
    {
        
       // UpdateSize();
    }
    protected override void OnEnable()
    {
        UpdateSize();
    }

    private void UpdateSize()
    {
        if (!IsActive())
        {
            return;
        }
        m_Tracker.Clear();
        if (rectTransform==null)
        {
            rectTransform = GetComponent<RectTransform>();
        }

        Vector2 parentSize = this.GetParentSize();
        Vector2 size = Vector2.zero;
        this.m_Tracker.Add((UnityEngine.Object)this, this.rectTransform, DrivenTransformProperties.Anchors |DrivenTransformProperties.SizeDelta);
        this.rectTransform.anchorMin = Vector2.zero;
        this.rectTransform.anchorMax = Vector2.one;
        this.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, parentSize.x* this.x);
        this.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, parentSize.y* this.y);
    }
    private float GetSizeDeltaToProduceSize(float size, int axis)
    {
        return size - this.GetParentSize()[axis] * (this.rectTransform.anchorMax[axis] - this.rectTransform.anchorMin[axis]);
    }
    private Vector2 GetParentSize()
    {
        RectTransform parent = this.rectTransform.parent as RectTransform;
        if (!(bool)((UnityEngine.Object)parent))
            return Vector2.zero;
        return parent.rect.size;
    }
    protected void SetDirty()
    {
        if (!IsActive())
        {
            return;
        }
        UpdateSize();
    }
    protected override void OnRectTransformDimensionsChange()
    {
        this.UpdateSize();
    }
    public void SetLayoutHorizontal()
    {
        UpdateSize();
    }

    public void SetLayoutVertical()
    {
        UpdateSize();
    }
    protected override void OnDisable()
    {
        m_Tracker.Clear();
    }

}