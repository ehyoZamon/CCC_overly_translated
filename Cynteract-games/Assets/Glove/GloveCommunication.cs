﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using System.Threading;
using System.IO.Ports;
using System.Xml.Linq;
using System.Xml;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Cynteract.CGlove
{
    /// <summary>
    /// Handles direct communication with the Glove
    /// </summary>
    public abstract partial class GloveCommunication
    {


        [NonSerialized]
        protected Glove glove;

        public RawGloveDataCallback onDataChanged;
        private Action<DataSend> sendDataCallbackOverride;
        private Action<string> informationCallback;
        private bool requestInformation = false;
        protected Action<GloveCommunication> onConnectedCallBack = (GloveCommunication comm) => { };
        protected Action<GloveCommunication> onDisconnectedCallBack = (GloveCommunication comm) => { };

        private bool quit = false;
        private float[] vibrationValues = new float[GloveInformation.vibrationNumber];
        private float[] vibrationWaveforms = new float[GloveInformation.vibrationNumber];

        /// <summary> Struct that must be populated and is sent when <see cref="SendData()"/> is called. </summary>
        public DataSend dataSend = new DataSend();
        /// <summary> Struct that is populated and can be read when a call to <see cref="ReceivePackage()"/> returns <see cref="PacketType.Data"/>. </summary>
        public DataReceive dataReceive = new DataReceive();
        /// <summary> String that is updated when a call to <see cref="ReceivePackage()"/> returns <see cref="PacketType.Debug"/>. </summary>
        public string debugReceive;
        /// <summary> JSON-String that is updated when a call to <see cref="ReceivePackage()"/> returns <see cref="PacketType.Information"/>. </summary>
        public string informationReceive;


        protected object callbackLock = new object();
        protected Action<DataReceive> dataReceiveCallback;
        protected Action<DataSend> dataSendCallback;
        protected Action<string> informationReceiveCallback;

        private int failureCounter = 0;

        private Thread receiveThread, sendThread;

        public GloveCommunication(Glove glove)
        {
            this.glove = glove;
        }
        ~GloveCommunication()
        {
            Close();
        }
        public void Vibrate(int index, float value)
        {
            vibrationValues[index] = value * 100;
            vibrationWaveforms[index] = 0;
        }
        public void Vibrate(int index, float value, int waveform)
        {
            vibrationValues[index] = value * 100;
            vibrationWaveforms[index] = waveform;
        }
        public void Vibrate(float[] vibrations)
        {
            for (int i = 0; i < GloveInformation.vibrationNumber; i++)
            {
                vibrationValues[i] = vibrations[i] * 100;
                vibrationWaveforms[i] = 0;
            }
        }
        public void Vibrate(float[] vibrations, float[] waveforms)
        {
            for (int i = 0; i < GloveInformation.vibrationNumber; i++)
            {
                vibrationValues[i] = vibrations[i] * 100;
                vibrationWaveforms[i] = waveforms[i];
            }
        }

        public virtual void Close()
        {
            quit = true;
            if (receiveThread != null)
            {
                Debug.Log("Closing Recieve Thread");
                receiveThread.Abort();
            }
            if (sendThread != null)
            {
                Debug.Log("Closing Send Thread");
                sendThread.Abort();
            }
        }



        public void SetDataSendCallback(Action<DataSend> sendDataCallback)
        {
            sendDataCallbackOverride = sendDataCallback;
        }
        public void RetrieveGloveInformation(Action<string> callback)
        {
            informationCallback = callback;
            requestInformation = true;
        }
        public void SubscribeOnConnected(Action<GloveCommunication> action)
        {
            onConnectedCallBack = action;
        }
        public void SubscribeOnDisconnected(Action<GloveCommunication> action)
        {
            onDisconnectedCallBack = action;
        }

        // public abstract statiic Scan ScanDevices();

        // returns whether connection was done the first time for the specific device; if connecting fails, an error should be thrown
        protected abstract bool Connect();

        // blocking
        protected abstract void ReadPackage();

        // blocking
        protected abstract bool WritePackage();

        public abstract void Reset();

        public void Start()
        {
            dataReceiveCallback = (DataReceive data) => onDataChanged(data);
            dataSendCallback = (DataSend data) =>
            {
                if (sendDataCallbackOverride != null)
                {
                    sendDataCallbackOverride(data);
                }
                else
                {
                    // (re-)send vibration commands
                    for (int i = 0; i < 8; i++)
                    {
                        data.vibration[i] = (byte)vibrationValues[i];
                        data.vibrationPattern[i] = (byte)vibrationWaveforms[i];
                        vibrationWaveforms[i] = 0;
                    }
                }
                data.requestInformation = requestInformation;
                requestInformation = false;
            };
            informationReceiveCallback = (string information) =>
            {
                // unstrip double quotes to create valid json again
                string json = Regex.Replace(information, @"[\w]+", (m) => '"' + m.ToString() + '"');
                informationCallback?.Invoke(json);
            };
            receiveThread = new Thread(ReceiveRoutine);
            sendThread = new Thread(SendRoutine);
            receiveThread.Start();
            sendThread.Start();
            Task task = ConnectionHeartbeat();
        }

        public async Task ConnectionHeartbeat()
        {
            while (!quit)
            {
                try
                {
                    if (Connect())
                    {
                        onConnectedCallBack(this);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
                // heartbeat rate
                await Task.Delay(500);
            };
        }
        private void ReceiveRoutine()
        {
            while (!quit)
            {
                try
                {
                    ReadPackage();
                    failureCounter = 0;
                }
                catch (Exception e) when (e is ObjectDisposedException || e is IOException || e is TimeoutException)
                {
                    // Debug.LogError(e);
                    failureCounter++;
                    // too many failures in a row, try resetting connector device
                    if (failureCounter >= 5)
                    {
                        //Debug.Log("Reset connector device.");
                        Reset();
                        failureCounter = 0;
                    }
                    // stream was closed or has problems, wait for reconnection/resolution
                    Thread.Sleep(500);
                }

            }
        }


        private void SendRoutine()
        {
            Thread.Sleep(5000);
            while (!quit)
            {
                try
                {
                    Thread.Sleep(20);
                    lock (callbackLock)
                    {
                        dataSendCallback(dataSend);
                    }
                    WritePackage();
                }
                catch (Exception e) when (e is ObjectDisposedException || e is IOException || e is TimeoutException)
                {
                    //Debug.LogError(e);
                    failureCounter++;
                    // too many failures in a row, try resetting connector device
                    if (failureCounter >= 5)
                    {
                        // Debug.Log("Reset connector device.");
                        Reset();
                        failureCounter = 0;
                    }
                    // stream was closed or has problems, wait for reconnection/resolution
                    Thread.Sleep(500);
                }
            }
        }
        protected void DeserializeDataReceived(byte[] data)
        {
            // for serialization and deserialization of struct see https://stackoverflow.com/a/2887
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                //dataReceive = (DataReceive)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(DataReceive));
                dataReceive = Marshal.PtrToStructure<DataReceive>(handle.AddrOfPinnedObject());
            }
            finally
            {
                handle.Free();
            }
        }

        protected void SerializeDataSend(byte[] data)
        {
            GCHandle h = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                Marshal.StructureToPtr<DataSend>(dataSend, h.AddrOfPinnedObject(), false);
            }
            finally
            {
                h.Free();
            }
        }
        public void RemoveGlove()
        {
            Debug.Log($"RemoveGlove, removing");
            glove.RemoveSelf();
            Debug.Log($"RemoveGlove, closing");
            try
            {
                Close();
            }
            catch (Exception e)
            {

                Debug.LogError(e);
            }
        }
    }
}
