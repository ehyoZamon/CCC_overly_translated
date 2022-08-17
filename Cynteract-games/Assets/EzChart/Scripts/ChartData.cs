using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChartUtil
{
    [System.Serializable]
    public class Data
    {
        public Data()
        {
            show = true;
            value = 0.0f;
        }

        public Data(float value, bool show)
        {
            this.value = value;
            this.show = show;
        }

        public bool show;
        public float value;
    }

    [System.Serializable]
    public class Series
    {
        public string name = "";
        public bool show = true;
        public List<Data> data = new List<Data>();
    }

    public class ChartData : MonoBehaviour
    {
        public List<Series> series = new List<Series>();
        public List<string> categories = new List<string>();
    }
}