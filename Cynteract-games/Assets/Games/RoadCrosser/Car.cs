using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.RoadCrosser
{
    public class Car : MonoBehaviour
    {
        public Rigidbody2D frank;
        public float speed = 1;
        public Vector3 direction;

        // Start is called before the first frame update
        void Start()
        {
            frank.velocity = speed * direction;
        }
        public void DestroyAfterSeconds(float seconds)
        {
            Destroy(gameObject, seconds);
        }
        // Update is called once per frame
        void Update()
        {

        }
    }
}