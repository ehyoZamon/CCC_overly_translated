using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorControl : MonoBehaviour
{
    public Texture2D tex;
    private void Start()
    {
        Cursor.SetCursor(tex, new Vector2(10,5), CursorMode.Auto);
    }
    private void Update()
    {
       
    }
}
