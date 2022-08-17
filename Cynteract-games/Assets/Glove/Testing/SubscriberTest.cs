using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cynteract.CGlove;
using System;

public class SubscriberTest : MonoBehaviour
{
    public bool unsubscribe;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 5);
    }

    // Update is called once per frame
    void Update()
    {
        if (unsubscribe)
        {
            Glove.Left.Unsubscribe(this);
        }
        Glove.Left.Subscribe(Callback);
    }

    private void Callback(Glove data)
    {
        print("recieved");
    }
}
