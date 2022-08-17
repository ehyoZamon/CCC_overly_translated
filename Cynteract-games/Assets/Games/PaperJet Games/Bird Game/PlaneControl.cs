using Cynteract.CGlove;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneControl : MonoBehaviour
{
     CharacterController characterController;
    new Rigidbody rigidbody;
    public new  Camera camera;
    float x, y;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        rigidbody = GetComponent<Rigidbody>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }


    private void Update()
    {
        var wristSplit= Glove.Any.data.wristRotation.splitValues;
        x += wristSplit.xAngle*Time.deltaTime;
        y -= wristSplit.yAngle * Time.deltaTime;
        transform.rotation = Quaternion.AngleAxis(y, Vector3.up)* Quaternion.AngleAxis(x, Vector3.right);
        transform.Translate(Vector3.forward*Time.deltaTime*10, Space.Self);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Glove.AnyData.ResetWrist();
        }

    }
    private void LateUpdate()
    {
        camera.transform.position = transform.position - 10f * transform.forward;

        camera.transform.LookAt(transform);
    }

}
