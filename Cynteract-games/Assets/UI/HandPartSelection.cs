using Cynteract.CynteractInput;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandPartSelection : MonoBehaviour
{

    public Color selectedColor, unselectedColor;
    public GloveInput.AxesEnum axis; 
    public GloveInput.ActionsEnum action;
    public Image image;

    [HideInInspector]
    public Type type;
    private HandPartSelectionMaster master;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Init(HandPartSelectionMaster master)
    {
        this.master = master;
        type = master.type;
    } 
    public void Select()
    {
        switch (type)
        {
            case Type.Axis:
                master.Select(this,axis);
                break;
            case Type.Action:
                master.Select(this,action);
                break;
            default:
                break;
        }
        image.color = selectedColor;
    }
    public void Deselect()
    {
        image.color =unselectedColor;
    }
}
