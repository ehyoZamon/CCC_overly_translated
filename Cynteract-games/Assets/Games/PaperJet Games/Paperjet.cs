using Cynteract.Hitzestrom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paperjet : MonoBehaviour
{
    public PaperJetControlAbstract paperjetControl;
    public void InHeat()
    {
        paperjetControl.inHeat = true;
    }
    public void NotInHeat()
    {
        paperjetControl.inHeat = false;

    }
}
