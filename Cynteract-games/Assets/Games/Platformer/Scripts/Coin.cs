using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Platformer
{
    public class Coin : MonoBehaviour
    {
        public Transform visualCoin;
        public float amplitude, speed;
        public Vector3 direction;
        public CircleCollider2D circleCollider;
        public int points = 10;
        private void Start()
        {
            circleCollider.radius = 2 * amplitude;
        }
        private void Update()
        {
            visualCoin.localPosition = direction * amplitude * Mathf.Sin(speed * Time.fixedTime);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.name == "Player")
            {
                Destroy(gameObject);
                JumpAndRun.instance.AddScore(points, visualCoin.transform);
            }
        }
    }
}