using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : MonoBehaviour
{
    public float attractionPower;
    private float magnetLevel;
    private float radius;
    private float scale;
    public GameObject Player;
    public GameObject Holder;

    private void OnTriggerStay2D(Collider2D collider)
    {

        collider.transform.position = Vector3.MoveTowards(collider.transform.position, transform.position, attractionPower * Time.deltaTime);
        Debug.DrawLine(transform.position, collider.transform.position, Color.yellow);

    }

}
