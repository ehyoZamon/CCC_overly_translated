using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Cynteract.CGlove
{
    public class GloveDisplayer : SerializedMonoBehaviour
    {

        public GloveData.RotationSet.RotationType rotationType;

        [ShowInInspector]
        public Dictionary<ImuState, Color> imuStateColors;
        [Serializable]
        public class RotationBind
        {
            public Quaternion rotation;
            [Required]
            public Transform transform;
            [ReadOnly]
            public ImuState state;
            public bool showStateAsColor;
            [ShowIf("showStateAsColor")]
            public MeshRenderer meshRenderer;
            public StateBasedAngles xAngle = new StateBasedAngles();
            public Finger finger;
        }
        public Dictionary<Fingerpart, RotationBind> binds;
        
        // Start is called before the first frame update
        void Start()
        {

            Glove.Any.Subscribe(Callback);
        }

        private void Callback(Glove data)
        {
            foreach (var item in binds.Keys)
            {
                binds[item].rotation = data.data[item].GetRotation(rotationType);
                binds[item].state = data.information.GetSensorState(item);
                binds[item].xAngle.Evaluate(data.data[item].splitValues.xAngle, binds[item].state);
            }
        }
        private void Update()
        {
            foreach (var item in binds)
            {
                item.Value.transform.rotation = item.Value.rotation;
                if (item.Value.showStateAsColor)
                {
                    if (item.Key==Fingerpart.palmCenter|| Glove.Any.information.GetUseFinger(item.Value.finger))
                    {
                        item.Value.meshRenderer.material.color = imuStateColors[item.Value.state];
                    }
                    else
                    {
                        item.Value.meshRenderer.material.color = Color.black;
                    }
                    
                }
            }
        }
        private void OnDrawGizmos()
        {
            if (binds!=null)
            {
                foreach (var item in binds.Values)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawRay(item.transform.position, item.transform.right);
                    Gizmos.color = Color.green;
                    Gizmos.DrawRay(item.transform.position, item.transform.up);
                    Gizmos.color = Color.blue;
                    Gizmos.DrawRay(item.transform.position, item.transform.forward);
                }
            }
        }
        [Button]
        public void ResetAll()
        {
            Glove.Any.data.ResetAll();
        }
    }

}