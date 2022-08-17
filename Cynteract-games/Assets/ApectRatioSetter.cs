using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApectRatioSetter : MonoBehaviour
{
    private int lastWidth;
    private int lastHeight;

    // Start is called before the first frame update
    void Start()
    {
        SetResolution();

    }

    private void SetResolution()
    {
        lastWidth = Screen.width;
        lastHeight = Screen.height;
        int height = (int)(Screen.width * (9f / 16f));
        Screen.SetResolution(Screen.width, height, true);
    }


}
