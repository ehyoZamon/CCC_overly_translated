using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupWindowCanvas : MonoBehaviour
{
   public static PopupWindowCanvas instance;
   private void Awake()
    {
        instance = this;
    }
}
