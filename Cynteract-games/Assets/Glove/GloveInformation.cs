using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
namespace Cynteract.CGlove
{
    /// <summary>
    /// Contains information about the glove. For glove sensor data see <seealso cref="GloveData"/>
    /// </summary>
    [Serializable]
    public class GloveInformation
    {
        public const int vibrationNumber = 10;
        #warning Hardcoded Vibration Number

        [NonSerialized]
        public Glove glove;
        [SerializeField]
        private bool[] useFingers;
        private int[] imuEnumToIndex, vibrationEnumIndex, forceEnumIndex;
        private Dictionary<string, int> imuIndizesDictionary, vibrationIndizesDictionary, forceIndizesDictionary;
        private DataReceive lastDataRecieved;

        public bool RecievedInformation
        {
            get;
            private set;
        }
        public Side side;
        public GloveInformation(Glove glove)
        {
            side = Side.NotSet;
            this.glove = glove;
        }
        public void Update(DataReceive data)
        {
            lastDataRecieved = data;
        }
        public void RetrieveInformation(string jsonString) 
        {
            Debug.Log(jsonString);
            RecievedInformation = true;
            dynamic data = JObject.Parse(jsonString);
            if (data.Hand=="Links")
            {
                side = Side.Left;
            }
            else if (data.Hand=="Rechts")
            {
                side = Side.Right;
            }
            imuIndizesDictionary = new Dictionary<string, int>();
            vibrationIndizesDictionary = new Dictionary<string, int>();
            forceIndizesDictionary = new Dictionary<string, int>();
            foreach (var part in data.IMU)
            {
                imuIndizesDictionary.Add(part.Value.ToString(),  int.Parse(part.Name));
            }
            foreach (var part in data.Vibration)
            {
                vibrationIndizesDictionary.Add(part.Value.ToString(), int.Parse(part.Name));
            }
            foreach (var part in data.Force)
            {
                forceIndizesDictionary.Add(part.Value.ToString(), int.Parse(part.Name));
            }
            imuEnumToIndex = DictionaryToArray<Fingerpart>(imuIndizesDictionary);
            vibrationEnumIndex = DictionaryToArray<Fingerpart>(vibrationIndizesDictionary);
            forceEnumIndex = DictionaryToArray<Fingerpart>(forceIndizesDictionary);

            JArray useFingersJArray = data.UseFingers;
            if (useFingersJArray!=null)
            {
                useFingers = useFingersJArray.ToObject<bool[]>();
            }
            else
            {
                useFingers = new bool[] { true, true, true, true, true };
            }
        }

        private int[] DictionaryToArray<T>(Dictionary<string, int> imuIndizesDictionary) where T: IConvertible
        {
            int[] array = new int[Enum.GetNames(typeof(T)).Length];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = -1;
            }
            foreach (var item in imuIndizesDictionary)
            {
                array[(int)Enum.Parse(typeof(T), item.Key)] = item.Value;
            }
            return array;
        }

        public ImuState GetSensorState(Fingerpart fingerpart)
        {
            return GetSensorState(lastDataRecieved, fingerpart);
        }
        public ImuState GetSensorState(DataReceive data, int index)
        {
            if (data==null||index==-1)
            {
                return ImuState.NotConnected;
            }
            return (ImuState)GetSensorStateInt(data,index);
        }
        public ImuState GetSensorState(DataReceive data, Fingerpart fingerpart)
        {
            return GetSensorState(data, GetIMUIndex(fingerpart));
        }
        public int GetIMUIndex(Fingerpart fingerpart)
        {
            if (imuEnumToIndex==null)
            {
                return -1;
            }
            return imuEnumToIndex[(int)fingerpart];
        }
        public int GetForceIndex(Fingerpart fingerpart)
        {
            return forceEnumIndex[(int)fingerpart];
        }
        public int GetVibrationIndex(Fingerpart fingerpart)
        {
            return vibrationEnumIndex[(int)fingerpart];
        }
        public bool GetUseFinger(Finger finger)
        {
            if (useFingers==null)
            {
                return false;
            }
            return useFingers[(int)finger];
        }
        public int GetSensorStateInt(DataReceive data , int i)
        {
            if (glove == null) return 2;
            else if (!WorkingSensor(data, i)) return 1;
            else if ((data.imuStatus[i] & 0b1111) == 0) return 0;
            else if ((data.imuStatus[i] & 0b1111) == 1) return 1;
            else if ((data.imuStatus[i] & 0b1111) == 2) return 2;
            else if ((data.imuStatus[i] & 0b1111) == 3)
            {
                if ((data.imuStatus[i] & 0b110000) == 0b000000) return 3;
                else if ((data.imuStatus[i] & 0b110000) == 0b010000) return 4;
                else if ((data.imuStatus[i] & 0b110000) == 0b100000) return 5;
                else if ((data.imuStatus[i] & 0b110000) == 0b110000) return 6;
            }
            return 2;
        }

        private static bool WorkingSensor(DataReceive data, int i)
        {
            return ((data.imuStatus[i] & 0b1111) == 3);
        }
    }
}
