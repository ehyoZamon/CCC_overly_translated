using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UiHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    bool hovering;
    private void Update()
    {
        if (hovering)
        {
            OnStay();
        }
    }

    protected virtual void OnStay()
    {
    }
    protected virtual void OnEnter()
    {
    }
    protected virtual void OnLeave()
    {
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
        OnEnter();

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
        OnLeave();

    }
}
