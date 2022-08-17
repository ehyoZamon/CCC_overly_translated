using Cynteract.CGlove;
#if NO_ODIN
#else 
using Sirenix.OdinInspector;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Threading;
using UnityEngine;
namespace Cynteract.CGlove
{
    /// <summary>
    /// Stores connected Gloves as <see cref="Glove"/>.
    /// </summary>
    public class GloveManager : MonoBehaviour
    {
#if NO_ODIN
#else
        [DrawWithUnity]
#endif
        public List<Glove> gloves = new List<Glove>();

        public bool enableBluetooth = false;
        public bool disableSerial = false;

        private Glove left;
        private Glove right;
        private Glove any;
        public static GloveManager instance;

        private object callbackLock = new object();
        Thread scanThread;
        private bool closed=false;
        private Scan scan;

        private void Awake()
        {

            instance = this;
        }
        /// <summary>
        /// Initializes the GloveManager. Searches for gloves and starts communication
        /// </summary>
        public void Init()
        {
            scanThread = new Thread(ScanDevices);
            scanThread.Start();
        }

        private async void ScanDevices()
        {
            while (!closed)
            {
                 scan = new Scan();
                
                var ports = await scan.ScanDevices();

                if (ports != null && ports.ids != null && ports.ids.Length > 0)
                {
                    foreach (var item in ports.ids)
                    {

                        lock (callbackLock)
                        {
                            if (gloves.Find(x=>x.comPort==item)!=null)
                            {
                               // print($"{item} still there");
                            }
                            else
                            {
                                
                                Debug.Log($"usb device {item} found");
                                Glove g = new Glove();
                                g.SubscribeOnConnected(OnConnected);
                                g.StartUSB(item);
                                gloves.Add(g);
                            }
                        }
                    }
                }
                
                Thread.Sleep(500);
            }

        }

        private void OnConnected(GloveInformation information)
        {
            var any = Glove.Any;
            var anyDataCallbacks = any.dataCallbacks;
            var anyResetCallbacks = any.resetCallbacks;
            var anyConnectedCallbacks = any.onConnectedCallbacks;
            var anyDisconnectedCallbacks = any.onDisconnectedCallbacks;
            this.any = information.glove;
            if (information.side == Side.Left)
            {
                var left = Glove.Left;

                var leftCallbacks = left.dataCallbacks;
                var leftResetCallbacks = left.resetCallbacks;
                var leftConnectedCallbacks = left.onConnectedCallbacks;
                var leftDisconnectedCallbacks = left.onDisconnectedCallbacks;

                this.left = information.glove;
                this.left.dataCallbacks = leftCallbacks;
                AddDataCallbacks(ref this.left.dataCallbacks);

                this.left.resetCallbacks = leftResetCallbacks;
                AddResetCallbacks(ref this.left.resetCallbacks);

                this.left.onConnectedCallbacks = leftConnectedCallbacks;
                AddConnectedCallbacks(ref this.left.onConnectedCallbacks);

                this.left.onDisconnectedCallbacks = leftDisconnectedCallbacks;
                AddDisconnectedCallbacks(ref this.left.onDisconnectedCallbacks);
            }
            if (information.side == Side.Right )
            {
                var right = Glove.Right;
                var rightCallbacks = right.dataCallbacks;
                var rightResetCallbacks = right.resetCallbacks;
                var rightConnectedCallbacks = right.onConnectedCallbacks;
                var rightDisconnectedCallbacks = right.onDisconnectedCallbacks;

                this.right = information.glove;
                this.right.dataCallbacks = rightCallbacks;
                AddDataCallbacks(ref this.right.dataCallbacks);

                this.right.resetCallbacks = rightResetCallbacks;
                AddResetCallbacks(ref this.right.resetCallbacks);

                this.right.onConnectedCallbacks = rightConnectedCallbacks;
                AddConnectedCallbacks(ref this.right.onConnectedCallbacks);

                this.right.onDisconnectedCallbacks = rightDisconnectedCallbacks;
                AddDisconnectedCallbacks(ref this.right.onDisconnectedCallbacks);
            }
            void AddDataCallbacks(ref List <GloveCallback> listToAddTo)
            {
                foreach (var item in anyDataCallbacks)
                {
                    if (!listToAddTo.Contains(item))
                    {
                        listToAddTo.Add(item);
                    }
                }
            }
            void AddResetCallbacks(ref List<GloveCallback> listToAddTo)
            {
                foreach (var item in anyResetCallbacks)
                {
                    if (!listToAddTo.Contains(item))
                    {
                        listToAddTo.Add(item);
                    }
                }
            }
            void AddConnectedCallbacks(ref List<GloveConnectedCallback> listToAddTo)
            {
                foreach (var item in anyConnectedCallbacks)
                {
                    if (!listToAddTo.Contains(item))
                    {
                        listToAddTo.Add(item);
                    }
                }
            }
            void AddDisconnectedCallbacks(ref List<GloveDisconnectedCallback> listToAddTo)
            {
                foreach (var item in anyDisconnectedCallbacks)
                {
                    if (!listToAddTo.Contains(item))
                    {
                        listToAddTo.Add(item);
                    }
                }
            }
        }
        /// <summary>
        /// Returns the left Glove. If no left glove is connected, returns a placeholder with full functionality but no data.
        /// </summary>
        /// <returns></returns>
        public Glove GetLeft()
        {
            if (left == null)
            {
                left = new Glove();
            }
            return left;
        }
        /// <summary>
        /// Returns the right Glove. If no right glove is connected, returns a placeholder with full functionality but no data.
        /// </summary>
        /// <returns></returns>
        public Glove GetRight()
        {
            if (right == null)
            {
                right = new Glove();
            }
            return right;
        }
        /// <summary>
        /// Returns a single connected. If no glove is connected, returns a placeholder with full functionality but no data.
        /// </summary>
        /// <returns></returns>
        public Glove GetAny()
        {
            if (any == null)
            {
                any = new Glove();
            }
            return any;
        }
        
        private void OnDestroy()
        {
            print("On Destroy");
            Close();
        }

        public void Close()
        {

            closed = true;
            foreach (var item in gloves)
            {
                item.communication.Close();
            }
        }

        public void Remove(Glove glove)
        {
            Debug.Log($"Remove Glove {glove.comPort}");
            var foundGlove = gloves.Find(x => x == glove);
            if (foundGlove==null)
            {
                return;
            }
            gloves.Remove(foundGlove);

        }
    }
}
