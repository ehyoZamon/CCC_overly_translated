using Cynteract.CGlove;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetTester : MonoBehaviour
{
    public Quaternion rawRotation;
    public Transform leftSideOffset, rightSideOffset, raw, offset;
    private void Update()
    {
        raw.rotation = rawRotation;
        offset.rotation = Quaternion.Inverse(leftSideOffset.rotation) * raw.rotation * Quaternion.Inverse(rightSideOffset.rotation);
    }
    private void OnDrawGizmos()
    {
        var cubes = new Transform[] { leftSideOffset, rightSideOffset, raw, offset };
        foreach (var item in cubes)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(item.position, item.right);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(item.position, item.up);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(item.position, item.forward);
        }
    }
    private void Start()
    {
        Glove.Any.Subscribe(OnGloveData);
    }

    private void OnGloveData(Glove data)
    {
        rawRotation = data.data.wristRotation.absolute;
    }
}