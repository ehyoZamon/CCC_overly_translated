using Cynteract.CGlove;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GloveInformationSimulator : MonoBehaviour
{
    public enum Mode
    {
        Start, Update
    }
    string[] testNames = { "pinkyBase", "pinkyCenter" };

    public Mode mode;
    // Start is called before the first frame update
    void Start()
    {
        if (mode==Mode.Start)
        {
            RecieveInformation();
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (mode==Mode.Update)
        {

        RecieveInformation();
        }
    }

    private void RecieveInformation()
    {
        JObject information = new JObject();
        information["IMU"] = new JObject();
        for (int i = 0; i < testNames.Length; i++)
        {
            information["IMU"][i.ToString()] = testNames[i];
        }
        Glove.Left.information.RetrieveInformation(information.ToString());
    }
}
