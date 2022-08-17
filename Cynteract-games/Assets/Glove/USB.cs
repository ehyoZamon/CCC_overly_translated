﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using System.IO.Ports;
using UnityEngine;
using UnityEngine.UIElements;
using System.IO;
using System.Threading.Tasks;

namespace Cynteract.CGlove
{
    public class USB : GloveCommunication
    {
        [DllImport("CP210xManufacturing.dll", EntryPoint = "CP210x_GetNumDevices")]
        public static extern int CP210x_GetNumDevices(
        [In, Out] ref UInt32 deviceNum);

        [DllImport("CP210xManufacturing.dll", EntryPoint = "CP210x_Open")]
        public static extern int CP210x_Open(
        [In] UInt32 deviceNumb,
        [In, Out] ref IntPtr deviceHandle);

        [DllImport("CP210xManufacturing.dll", EntryPoint = "CP210x_Close")]
        public static extern int CP210x_Close(
        [In, Out] IntPtr deviceHandle);

        [DllImport("CP210xManufacturing.dll", EntryPoint = "CP210x_Reset")]
        public static extern int CP210x_Reset(
        [In, Out] IntPtr deviceHandle);

        [DllImport("CP210xManufacturing.dll", EntryPoint = "CP210x_GetDeviceProductString",
        CharSet = CharSet.Ansi)]
        public static extern int CP210x_GetDeviceProductString(
        [In, Out] IntPtr deviceHandle,
        [In, Out] StringBuilder Product,
        [In, Out] ref byte Length,
        [In] bool ConvertToASCII);


        [DllImport("CP210xManufacturing.dll", EntryPoint = "CP210x_SetProductString",
        CharSet = CharSet.Ansi)]
        public static extern int CP210x_SetProductString(
        [In, Out] IntPtr deviceHandle,
        [In] StringBuilder Product,
        [In] byte Length,
        [In] bool ConvertToUniCode);


        private Stream stream;
        private SerialPort serial;
        bool firstConnect = true;


        class PackageReadBuffer
        {
            public long transmissionStartTime = 0;
            public byte[] data = new byte[1024];
            public int offset = 0;
        };

        PackageReadBuffer packageReadBuffer = new PackageReadBuffer();
        byte[] packageSendBuffer;

        public USB(Glove glove) : base(glove)
        {
            packageSendBuffer = new byte[Marshal.SizeOf(dataSend)];
        }

        // TODO specify comport to reset a specific device only
        public static bool ResetDevice()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)){
                IntPtr siRef = IntPtr.Zero;
                uint numDevice = 0;
                int code;
                code = CP210x_GetNumDevices(ref numDevice);
                if (code != 0)
                {
                    Debug.Log("CP210x_GetNumDevices failed with code " + code);
                    return false;
                }

                if (numDevice != 1)
                {
                    if (numDevice == 0)
                    {
                        Debug.Log("no device found");
                    }
                    else
                        Debug.Log("more than one device found, but must be one");
                    return false;
                }

                code = CP210x_Open(0, ref siRef);
                if (code != 0)
                {
                    Debug.Log("CP210x_Open failed with code " + code);
                    return false;
                }

                code = CP210x_Reset(siRef);
                if (code != 0)
                {
                    Debug.Log("CP210x_Reset failed with code " + code);
                }

                code = CP210x_Close(siRef);
                if (code != 0)
                {
                    Debug.Log("CP210x_Close failed with code " + code);
                    return false;
                }

                Thread.Sleep(100);

                return true;
            }
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Debug.Log("Not done yet");
            }
            return false;
        }
        /*
        public static bool ResetAllDevices()
        {
            uint numDevice = 0;
            int code;
            code = CP210x_GetNumDevices(ref numDevice);
            if (code != 0)
            {
                Debug.Log("CP210x_GetNumDevices failed with code " + code);
                return false;
            }
            if (numDevice == 0)
            {
                Debug.Log("no device found");
            }
            for (uint i = 0; i < numDevice; i++)
            {
                Reset(i);
            }
            return true;
        }
        public static bool Reset(uint index)
        {
            int code;
            IntPtr siRef = IntPtr.Zero;
            code = CP210x_Open(index, ref siRef);
            if (code != 0)
            {
                Debug.Log("CP210x_Open failed with code " + code);
                return false;
            }

            code = CP210x_Reset(siRef);
            if (code != 0)
            {
                Debug.Log("CP210x_Reset failed with code " + code);
            }

            code = CP210x_Close(siRef);
            if (code != 0)
            {
                Debug.Log("CP210x_Close failed with code " + code);
                return false;
            }
            return true;
        }*/
        public static bool PrintDevices()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                uint numDevice = 0;
                IntPtr deviceHandle = IntPtr.Zero;

                var code = CP210x_GetNumDevices(ref numDevice);
                if (code != 0)
                {
                    //Debug.Log("CP210x_GetNumDevices failed with code " + code);
                    return false;
                }
                for (uint i = 0; i < numDevice; i++)
                {
                    CP210x_Open(i, ref deviceHandle);

                    StringBuilder productStringSet = new StringBuilder("Hello World");
                    byte lengthSet = (byte)productStringSet.Length;
                    CP210x_SetProductString(deviceHandle, productStringSet, lengthSet, true);

                    CP210x_Close(deviceHandle);

                    CP210x_Open(i, ref deviceHandle);

                    StringBuilder product = new StringBuilder();
                    byte length = 11;
                    CP210x_GetDeviceProductString(deviceHandle, product, ref length, true);
                    Debug.Log(product.ToString());
                    CP210x_Close(deviceHandle);
                }
                return true;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                // Get a list of serial port names.
                string[] ports = SerialPort.GetPortNames();

                Debug.Log("The following serial ports were found:");

                // Display each port name to the console.
                foreach(string port in ports)
                {
                    if (port.Contains("tty.SLAB_USBtoUART")) // or tty.usbserial-... without VCP drivers
                    {
                        Debug.Log(port + "<-Glove");
                    
                    }
                    else if(port.Contains("tty.usbserial-")){ // without VCP drivers this will be one of these
                        Debug.Log(port + "<-Maybe");
                    }
                    else{
                        Debug.Log(port);
                    }
                }

                return true;
            }
            return false;

        }

        /*
        public static Scan ScanDevices()
        {
            Scan s = new Scan();
            s.Found = null;
            s.Finished = null;
            new Thread(() =>
            {
                foreach (string comPort in DetectComPorts())
                {
                    s.Found?.Invoke(comPort);
                }
                s.Finished?.Invoke();
            }).Start();
            return s;
        }*/


        public static string[] DetectComPorts()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                List<string> ports = new List<string>();
                string name = "CP210";
                string extra = "USB to UART";

                RegistryKey rk2 = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
                string temp;
                foreach (string s3 in rk2.GetSubKeyNames())
                {
                    RegistryKey rk3 = rk2.OpenSubKey(s3);
                    foreach (string s in rk3.GetSubKeyNames())
                    {
                        if (s.Contains("VID") && s.Contains("PID"))
                        {
                            RegistryKey rk4 = rk3.OpenSubKey(s);
                            foreach (string s2 in rk4.GetSubKeyNames())
                            {
                                RegistryKey rk5 = rk4.OpenSubKey(s2);
                                if ((temp = (string)rk5.GetValue("FriendlyName")) != null && (temp.Contains(name) || (temp.Contains(extra))))
                                {
                                    RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                                    if (rk6 != null && (temp = (string)rk6.GetValue("PortName")) != null)
                                    {
                                        if (Array.IndexOf(SerialPort.GetPortNames(), temp) != -1)
                                        {
                                            ports.Add(temp);
                                            /* extra filtering to differentiate from falsely recognized device didn't work out on adabru's pc
                                            RegistryKey rk7 = rk6.OpenSubKey("Ceip");
                                            if (rk7!=null)
                                            {
                                                dynamic dev=rk7.GetValue("DeviceInformation");
                                                if (dev!=0)
                                                {
                                                    ports.Add( temp);
                                                }
                                            }
                                            */

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return ports.ToArray();
            }

            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {

                string[] ports = SerialPort.GetPortNames();
                var possible_ports = new List<string>();
            
                foreach(string port in ports){
                    if (port.Contains("tty.SLAB_USBtoUART")) // or tty.usbserial-... without VCP drivers
                    {
                        possible_ports.Insert(0,port); //Add as first element to make testing faster
                          
                        string[] port_as_array = { port };
                        return port_as_array;
                    }
                    else if(port.Contains("tty.usbserial-")){ // without VCP drivers this will be one of these
                        possible_ports.Add(port);
                    }
                }
                return possible_ports.ToArray();
                /* 
                //Debug.Log(string.Join(",",possible_ports) );
                Stream stream;
                foreach (string port in possible_ports)
                {
                    try
                    {
                        SerialPort serial = new SerialPort();   
                        serial.PortName = port;
                        serial.ReadTimeout = 1000;
                        serial.WriteTimeout = 1000;
                        serial.DtrEnable = true;
                        serial.RtsEnable = true;
                        serial.BaudRate = 230400;
        
                        serial.Open();
        
                        serial.DiscardOutBuffer();
                        serial.DiscardInBuffer();

                        // Console.ReadLine();
                    
                        stream = serial.BaseStream;

                        if (stream == null)
                        {
                            throw new ObjectDisposedException("serial");
                        }

                        StreamReader sr = new StreamReader(stream);
                        string msg = sr.ReadLine();
                        msg +=sr.ReadLine(); 
                        msg +=sr.ReadLine(); 
                    
                        bool correct = msg.Contains("CYNTERACT");
                        if (correct)
                        {
                            string[] port_as_array = { port };
                            Debug.Log("Using Port " + port);
                            return port_as_array;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception on serial connect: " + e.ToString());
                    }
                }
             */
             }

            //TODO: throw error
            Debug.Log("No Glove found!");
            return null;
        }

        //// following code would need System.Management which is problematic in Unity
        //
        //public static string DetectComPort()
        //{
        //    string name = "Silicon Labs CP210x USB to UART Bridge";
        //    ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from Win32_SerialPort");
        //    foreach (ManagementObject port in searcher.Get())
        //    {
        //        if (port["Name"].ToString().Contains(name))
        //            return port["DeviceID"].ToString();
        //    }
        //    return null;
        //}

        protected override bool Connect()
        {
            // check connection health
            try
            {
                if (serial != null && serial.IsOpen)
                {
                    var callGet = serial.BytesToRead;
                }
            }
            catch (IOException)
            {
                firstConnect = true;
                // the connection is broken
                onDisconnectedCallBack(this);
                RemoveGlove();
                //serial.Close();

            }

            CConsole.Log(glove.comPort,CSubConsoleType.Glove);

            if (serial == null || !serial.IsOpen)
            {
                serial = new SerialPort(glove.comPort, 230400);
                serial.ReadTimeout = 1000;
                serial.WriteTimeout = 1000;
                serial.Open();
                serial.DiscardOutBuffer();
                serial.DiscardInBuffer();


                stream = serial.BaseStream;
                if (firstConnect)
                {
                    firstConnect = false;
                    return true;
                }
            }

            return false;
        }

        protected override void ReadPackage()
        {
            if (stream == null)
            {
                throw new ObjectDisposedException("serial");
            }

            var buffer = packageReadBuffer;
            int x;
            PacketType packetType = PacketType.None;
            while ((x = stream.ReadByte()) != -1)
            {
                // case 1: last package timed out
                if (buffer.transmissionStartTime > 0 && DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond > buffer.transmissionStartTime + 2000)
                {
                    Debug.Log("timeout");
                    buffer.transmissionStartTime = 0;
                }
                // case 2: transmission of new package
                if (buffer.transmissionStartTime == 0)
                {
                    buffer.offset = 0;
                    buffer.transmissionStartTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                }
                // case 3: buffer overflow, this package will be corrupted
                if (buffer.offset == buffer.data.Length)
                {
                    Debug.Log("buffer overflow");
                    buffer.offset = 0;
                }
                // case 4: continuation of already started package
                buffer.data[buffer.offset++] = (byte)x;
                // case 5: end of package reached
                if (buffer.offset >= Protocol.PACKAGE_DELIM.Length &&
                     Protocol.MemoryCompare(Protocol.PACKAGE_DELIM, buffer.data, buffer.offset - Protocol.PACKAGE_DELIM.Length))
                {
                    //Debug.Log(b.offset);
                    buffer.transmissionStartTime = 0;
                    if (Protocol.MemoryCompare(Encoding.ASCII.GetBytes("DATA"), buffer.data))
                    {
                        if (buffer.offset - Protocol.PACKAGE_DELIM.Length != Marshal.SizeOf(dataReceive))
                        {
                            Debug.Log("data packet has wrong size");
                            packetType = PacketType.None;
                            break;
                        }
                        else
                        {
                            DeserializeDataReceived(packageReadBuffer.data);
                            packetType = PacketType.Data;
                            break;

                        }
                    }
                    else if (Protocol.MemoryCompare(Encoding.ASCII.GetBytes("DEBUG"), buffer.data))
                    {
                        debugReceive = Encoding.ASCII.GetString(packageReadBuffer.data, 5, packageReadBuffer.offset - 5 - Protocol.PACKAGE_DELIM.Length);
                        packetType = PacketType.Debug;
                        break;

                    }
                    else if (buffer.data[0] == '{')
                    {
                        // glove information sent as json
                        informationReceive = Encoding.ASCII.GetString(packageReadBuffer.data, 0, packageReadBuffer.offset - Protocol.PACKAGE_DELIM.Length);
                        packetType = PacketType.Information;
                        break;

                    }
                    packetType = PacketType.None;
                    break;

                }
            }

            switch (packetType)
            {
                case PacketType.Data:
                    lock (callbackLock)
                    {
                        dataReceiveCallback(dataReceive);
                    }
                    break;
                case PacketType.Information:
                    lock (callbackLock)
                    {
                        informationReceiveCallback(informationReceive);
                    }
                    break;
                case PacketType.Debug:
                    CConsole.Log(debugReceive, CSubConsoleType.Glove);
                    Debug.Log("[glove debug]" + debugReceive);
                    break;
                case PacketType.None:
                    break;
            }
            //Debug.Log($"PackageReadbufferOffset {packageReadBuffer.offset} BufferOffset{buffer.offset}");
        }

        protected override bool WritePackage()
        {
            if (stream == null)
                throw new ObjectDisposedException("serial");
            SerializeDataSend(packageSendBuffer);
            stream.Write(packageSendBuffer, 0, Marshal.SizeOf(dataSend));
            stream.Write(Protocol.PACKAGE_DELIM, 0, Protocol.PACKAGE_DELIM.Length);
            return true;
        }

        public override void Reset()
        {

            try
            {
                if (serial != null)
                {
                    serial.Close();
                    Thread.Sleep(100);
                }
                if (!ResetDevice())
                {
                    Debug.Log("Resetting device failed.");
                    RemoveGlove();
                }

            }
            catch (UnauthorizedAccessException exc)
            {
                Debug.Log("Closing serial port failed, reset failed." + exc);
                RemoveGlove();

            }
        }

        public override void Close()
        {
            base.Close();
            if (serial != null)
            {
                Debug.Log("Closing Serial");
                serial.Close();
            }
        }
        
    }
}
