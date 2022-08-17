using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeltaTimeTester : MonoBehaviour
{
    public float timeSinceStart;
    public float deltaTimeAccumulated;
    private float startTime;
    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.fixedTime;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceStart = Time.fixedTime - startTime;
        deltaTimeAccumulated += Time.deltaTime;
    }
}
