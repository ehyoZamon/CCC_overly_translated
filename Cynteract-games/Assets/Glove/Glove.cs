using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Linq;
using System.Diagnostics;
using System.Net.Security;
using System.Threading;
using System.Linq;

namespace Cynteract.CGlove
{
    /// <summary>
    /// Center of Data and operations of one glove.
    /// </summary>
    [Serializable]
    public class Glove
    {
        public GloveInformation information;



        public GloveData data;
        public GloveCommunication communication;

        public string comPort;
        public string bleId;
        public List<GloveCallback> dataCallbacks, resetCallbacks;
        public List<GloveConnectedCallback>  onConnectedCallbacks;
        public List<GloveDisconnectedCallback>  onDisconnectedCallbacks;

        Mutex dataCallbackMutex, resetCallbackMutex, onConnectedCallbackMutex, onDisconnectedCallbackMutex;
        /// <summary>
        /// The right glove. Shorthand for <code>GloveManager.instance.GetRight()</code>  
        /// </summary>
        public static Glove Right
        {
            get
            {
                return GloveManager.instance.GetRight();
            }
        }
        /// <summary>
        /// The left glove. Shorthand for <code>GloveManager.instance.GetLeft()</code>  
        /// </summary>
        public static Glove Left
        {
            get
            {
                return GloveManager.instance.GetLeft();
            }
        }
        /// <summary>
        /// A single connected glove. Can be left or right.  Shorthand for <code>GloveManager.instance.GetAny()</code>  
        /// </summary>
        public static Glove Any
        {
            get
            {
                if (GloveManager.instance==null)
                {
                    return null;
                }
                return GloveManager.instance.GetAny();
            }
        }
        /// <summary>
        /// The <see cref="GloveData"/> of the right glove.
        /// Shorthand for <code>GloveManager.instance.GetRight().data</code>  
        /// </summary>
        public static GloveData RightData
        {
            get
            {
                return GloveManager.instance.GetRight().data;
            }
        }
        /// <summary>
        /// The <see cref="GloveData"/> of the left glove.
        /// Shorthand for <code>GloveManager.instance.GetLeft().data</code>  
        /// </summary>
        public static GloveData LeftData
        {
            get
            {
                return GloveManager.instance.GetLeft().data;
            }
        }
        /// <summary>
        /// The <see cref="GloveData"/> of a single connected glove.
        /// Shorthand for <code>GloveManager.instance.GetAny().data</code>  
        /// </summary>
        public static GloveData AnyData
        {
            get
            {
                return GloveManager.instance.GetAny().data;
            }
        }
        public static GloveData GetGloveData(Side side)
        {
            switch (side)
            {
                case Side.NotSet:
                    return AnyData;
                case Side.Left:
                    return LeftData;
                case Side.Right:
                    return RightData;
                default:
                    throw new Exception("No Glove specified");
            }
        }
        public Glove() : this("")
        {

        }
        public Glove(string comPort)
        {
            dataCallbacks = new List<GloveCallback>();
            resetCallbacks = new List<GloveCallback>();
            onConnectedCallbacks = new List<GloveConnectedCallback>();
            onDisconnectedCallbacks = new List<GloveDisconnectedCallback>();
            data = new GloveData(this);
            data.onReset = OnReset;
            information = new GloveInformation(this);
            
            dataCallbackMutex = new Mutex();
            resetCallbackMutex = new Mutex();
            onConnectedCallbackMutex = new Mutex();
            onDisconnectedCallbackMutex = new Mutex();
        }
        public void StartUSB(string comPort)
        {
            if (communication != null)
                throw new InvalidOperationException("There is already a communication channel");
            this.comPort = comPort;
            communication = new USB(this);
            communication.onDataChanged = OnDataChanged;
            communication.SubscribeOnConnected(OnConnected);
            communication.SubscribeOnDisconnected(OnDisconnected);
            communication.Start();
        }
        /*
        public void StartBLE(string bleId)
        {
            if (communication != null)
                throw new InvalidOperationException("There is already a communication channel");
            this.bleId = bleId;
            communication = new BLE(this);
            communication.onDataChanged = OnDataChanged;
            communication.SubscribeOnConnected(OnConnected);
            communication.SubscribeOnDisconnected(OnDisconnected);
            communication.Start();
        }
        */
        ~Glove()
        {
            if (dataCallbackMutex!=null)
            {
                dataCallbackMutex.ReleaseMutex();
            }
            if (onConnectedCallbackMutex != null)
            {
                onConnectedCallbackMutex.ReleaseMutex();
            }
            if (onDisconnectedCallbackMutex != null)
            {
                onDisconnectedCallbackMutex.ReleaseMutex();
            }
        }
        /// <summary>
        /// Retrieves <see cref="GloveInformation"/>. Tells the Glove to send information. Calls the callback, when done.
        /// </summary>
        /// <param name="callback"></param>
        public void RetrieveInformation(Action<GloveInformation> callback)
        {
            communication.RetrieveGloveInformation((string x) =>
            {
                information.RetrieveInformation(x);
                callback(information);
            }
            );
        }
        /// <summary>
        /// Add a subscription to be called back when the glove is connected.
        /// </summary>
        /// <param name="gloveConnectedCallback"></param>
        public void SubscribeOnConnected(GloveConnectedCallback gloveConnectedCallback)
        {
            onConnectedCallbackMutex.WaitOne();
            onConnectedCallbacks.Add(gloveConnectedCallback);
            onConnectedCallbackMutex.ReleaseMutex();
        }
        /// <summary>
        /// Remove the subscription to be called back when the glove is connected.
        /// </summary>
        /// <param name="gloveConnectedCallback"></param>
        public void UnsubscribeOnConnected(GloveConnectedCallback gloveConnectedCallback)
        {
            onConnectedCallbackMutex.WaitOne();
            onConnectedCallbacks.Remove(gloveConnectedCallback);
            onConnectedCallbackMutex.ReleaseMutex();

        }
        /// <summary>
        /// Remove the subscription to be called back when the glove is connected.
        /// </summary>
        /// <param name="owner">The owner of the callback (<c>this</c>, when the calling object added the callback).</param>
        public void UnsubscribeOnConnected(object owner)
        {
            onConnectedCallbackMutex.WaitOne();
            Unsubscribe(owner, onConnectedCallbacks);
            onConnectedCallbackMutex.ReleaseMutex();
        }

        /// <summary>
        /// Add a subscription to be called back when the glove is disconnected.
        /// </summary>
        /// <param name="gloveDisconnectedCallback"></param>
        public void SubscribeOnDisconnected(GloveDisconnectedCallback gloveDisconnectedCallback)
        {
            onDisconnectedCallbackMutex.WaitOne();
            onDisconnectedCallbacks.Add(gloveDisconnectedCallback);
            onDisconnectedCallbackMutex.ReleaseMutex();
        }
        /// <summary>
        /// Remove the subscription to be called back when the glove is disconnected.
        /// </summary>
        /// <param name="gloveDisconnectedCallback"></param>
        public void UnsubscribeOnDisconnected(GloveDisconnectedCallback gloveDisconnectedCallback)
        {
            onDisconnectedCallbackMutex.WaitOne();
            onDisconnectedCallbacks.Remove(gloveDisconnectedCallback);
            onDisconnectedCallbackMutex.ReleaseMutex();

        }
        /// <summary>
        /// Remove the subscription to be called back when the glove is disconnected.
        /// </summary>
        /// <param name="owner">The owner of the callback (<c>this</c>, when the calling object added the callback).</param>
        public void UnsubscribeOnDisconnected(object owner)
        {
            onDisconnectedCallbackMutex.WaitOne();
            Unsubscribe(owner, onDisconnectedCallbacks);
            onDisconnectedCallbackMutex.ReleaseMutex();

        }


        /// <summary>
        /// Add a subscription to be called back when the glove is Reset.
        /// </summary>
        /// <param name="callback"></param>
        public void SubscribeOnReset(GloveCallback callback)
        {
            resetCallbackMutex.WaitOne();
            resetCallbacks.Add(callback);
            resetCallbackMutex.ReleaseMutex();
        }


        /// <summary>
        /// Add a subscription to be called back when the glove sends Data.
        /// </summary>
        /// <param name="callback"></param>
        public void Subscribe(GloveCallback callback)
        {
            dataCallbackMutex.WaitOne();
            dataCallbacks.Add(callback);
            dataCallbackMutex.ReleaseMutex();
        }
        /// <summary>
        /// Remove the subscription to be called back when the glove sends Data.
        /// </summary>
        /// <param name="callback"></param>
        public void Unsubscribe(GloveCallback callback)
        {
            dataCallbacks.Remove(callback);
        }
        /// <summary>
        /// Add a subscription to be called back when the glove sends Data.
        /// </summary>
        /// <param name="owner">The owner of the callback (<c>this</c>, when the calling object added the callback).</param>
        public void Unsubscribe(object owner)

        {
            Unsubscribe(owner, dataCallbacks);
        }

        public void RemoveSelf()
        {
            GloveManager.instance.Remove(this);
        }

        private void Unsubscribe(object owner,dynamic callbacks)
        {
            for (int i = 0; i < callbacks.Count; i++)
            {
                if (callbacks[i].Target.ToString() == owner.ToString())
                {
                    callbacks.Remove(callbacks[i]);
                }
            }
        }
        /// <summary>
        /// Let´s the specified fingerpart vibrate with the specified strength.
        /// </summary>
        /// <param name="fingerpart"></param>
        /// <param name="value"></param>
        public void Vibrate(Fingerpart fingerpart, float value)

        {
            try
            {
                var index = information.GetVibrationIndex(fingerpart);
                communication.Vibrate(index, value);
            }
            catch (Exception e)
            {

                UnityEngine.Debug.LogWarning(e);
            }
        }
        public void VibrateRawIndex(int index, float value)

        {
            try
            {
                communication.Vibrate(index, value);
            }
            catch (Exception e)
            {

                UnityEngine.Debug.LogWarning(e);
            }
        }
        /// <summary>
        /// Let´s the specified fingerpart vibrate with the specified pattern.
        /// </summary>
        public void Vibrate(Fingerpart fingerpart, VibrationPattern pattern)
        {
            try
            {
                var index = information.GetVibrationIndex(fingerpart);
                communication.Vibrate(index, 0, (int)pattern);
            }
            catch (Exception e)
            {

                UnityEngine.Debug.LogWarning(e);
            }
        }
        public void VibrateRawIndex(int index, VibrationPattern pattern)
        {
            try
            {
                communication.Vibrate(index, 0, (int)pattern);
            }
            catch (Exception e)
            {

                UnityEngine.Debug.LogWarning(e);
            }
        }
        /// <summary>
        /// Let´s all fingerparts vibrate with the specified strength.
        /// </summary>
        public void VibrateAll(float value)
        {
            var values = new float[GloveInformation.vibrationNumber];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = value;
            }
            communication.Vibrate(values);
        }
        /// <summary>
        /// Let´s all fingerparts vibrate with a random  strength between <c>min</c> and <c>max</c>.
        /// </summary>
        public void VibrateAllRandom(float min, float max)
        {
            var values = new float[GloveInformation.vibrationNumber];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = UnityEngine.Random.Range(min, max);
            }
            communication.Vibrate(values);
        }
        /// <summary>
        /// Let´s all fingerparts vibrate with a specified pattern.
        /// </summary>
        public void VibrateAll( VibrationPattern pattern)
        {
            if (communication==null)
            {
                return;
            }
            var values = new float[GloveInformation.vibrationNumber];
            var waveforms = new float[GloveInformation.vibrationNumber];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = 0;
                waveforms[i] = (int)pattern;
            }
            communication.Vibrate(values, waveforms);
        }
        /// <summary>
        /// Let´s all fingerparts vibrate with a random pattern out of the patterns specified.
        /// </summary>
        public void VibrateAllRandom(params VibrationPattern []patterns)
        {
            var values = new float[GloveInformation.vibrationNumber];
            var waveforms = new float[GloveInformation.vibrationNumber];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = 0;
                waveforms[i] = (int)patterns[UnityEngine.Random.Range(0, patterns.Length)];
            }
            communication.Vibrate(values, waveforms);
        }
        /// <summary>
        /// Stops all vibrations
        /// </summary>
        public void VibrateStop()
        {

            try
            {
                var vibrations = new float[GloveInformation.vibrationNumber];
                communication.Vibrate(vibrations);
            }
            catch (Exception e)
            {

                UnityEngine.Debug.LogWarning(e);
            }
        }


        public void OnConnected(GloveCommunication gloveCommunication)
        {
            gloveCommunication.RetrieveGloveInformation(
                (_information) =>
                {
                    information.RetrieveInformation(_information);
                    onConnectedCallbackMutex.WaitOne();
                    foreach (var item in onConnectedCallbacks)
                    {
                        //try
                        // {
                        item(information);
                        //}
                        //catch (Exception e)
                        //{

                        // UnityEngine.Debug.LogError(e);
                        //}
                    }
                    onConnectedCallbackMutex.ReleaseMutex();
                });
        }
        public void OnDisconnected(GloveCommunication gloveCommunication)
        {
            information = new GloveInformation(this);

            onDisconnectedCallbackMutex.WaitOne();
            foreach (var item in onDisconnectedCallbacks)
            {
                try
                {
                    item(information);
                }
                catch (Exception e)
                {

                    UnityEngine.Debug.LogError(e);
                }
            }
            onDisconnectedCallbackMutex.ReleaseMutex();
            try
            {
                OnDataChanged(new DataReceive());

            }
            catch (Exception e)
            {

                 UnityEngine.Debug.LogError(e);
                
            }
        }
        public void OnDataChanged(DataReceive data)
        {
            this.data.Calculate(data);
            this.information.Update(data);
            dataCallbackMutex.WaitOne();

            foreach (var callback in dataCallbacks)
            {
                try
                {
                    callback(this);
                }
                catch (Exception e)
                {

                    UnityEngine.Debug.LogError(e);
                }

            }
            dataCallbackMutex.ReleaseMutex();
        }
        public void OnReset()
        {
            resetCallbackMutex.WaitOne();

            foreach (var callback in resetCallbacks)
            {
                try
                {
                    callback(this);
                }
                catch (Exception e)
                {

                    UnityEngine.Debug.LogError(e);
                }

            }
            resetCallbackMutex.ReleaseMutex();
        }
    }
}
