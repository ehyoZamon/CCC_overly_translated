using Cynteract.CynteractInput;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.RoadCrosser
{
    public class Player : MonoBehaviour
    {
        int currentRoad = 0;
        public Rigidbody2D rigidbody;
        public Vector3 nextPosition;
        public float speed;
        public float moveSpeed;
        bool justMoved=false;
        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (RoadCrosserInput.GetAction(RoadCrosserInput.stepOver))
            {
                if (!justMoved)
                {
                    justMoved = true;
                    RoadSpawner.instance.AddNewRoad();
                    nextPosition = new Vector3(transform.position.x, RoadSpawner.instance.roads[RoadSpawner.RoadBuffer].transform.position.y);
                }
            }
            else
            {
                justMoved = false;
            }

            nextPosition += Vector3.right * Time.deltaTime * moveSpeed*(CInput.GetAxis(RoadCrosserInput.moveLeftRight)*2-1);
            

        }
        void FixedUpdate()
        {
            rigidbody.velocity = speed * (nextPosition - transform.position);
        }
        void OnCollisionEnter2D(Collision2D col)
        {
            RoadSpawner.instance.Respawn();
        }
    }
}