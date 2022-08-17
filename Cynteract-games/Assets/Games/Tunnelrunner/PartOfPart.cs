using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Tunnelrunner
{
    public class PartOfPart : MonoBehaviour
    {
        EdgeCollider2D polygonCollider;
        public float collidingStart;
        public void GeneratePolyCollider(Mesh mesh)
        {

            polygonCollider = gameObject.AddComponent<EdgeCollider2D>();
            var points = new List<Vector2>();
            for (int i = 0; i < mesh.vertexCount; i+=2)
            {
                points.Add(mesh.vertices[i]);
            }
            for (int i = mesh.vertexCount-1; i >= 1; i -= 2)
            {
                points.Add(mesh.vertices[i]);
            }
            points.Add(mesh.vertices[0]);
            polygonCollider.points = points.ToArray();

            /*
            polygonCollider.pathCount = mesh.vertexCount / 5;


            for (int i = 0; i < polygonCollider.pathCount; i++)
            {
                var points = new Vector2[5];
                for (int j = 0; j < points.Length; j++)
                {
                    Vector3 vertex = mesh.vertices[j + i * 5];
                    points[j] = new Vector2(vertex.x, vertex.y);
                }
                polygonCollider.SetPath(i, points);
            }

            */
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                collidingStart = Time.fixedTime;
            }
        }
        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                if (Time.fixedTime-collidingStart>.05f)
                {
                    Tunnelrunner.instance.Destruction();
                }
            }
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            collidingStart = Time.fixedTime;
        }
    }
}