using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;
using Cynteract.CGlove;
using Cynteract.CCC;

public class Fire : MonoBehaviour {
    public GameObject ball;
    public Transform cannonBallParent;
    public float power;
    public Transform particle;
    public Trail trail;
    
    internal void Shoot()
    {
        GameObject instanceBall;

        CameraShaker.Instance.Shake(CameraShakePresets.DirectedBump(-transform.forward));
        
        if (Cynteract.Database.DatabaseManager.instance?.GetSettings().useVibration??false)
        {
            Glove.Any.VibrateAll(VibrationPattern.LongStrongClick);
        
        }
        Instantiate(particle,transform.position, transform.rotation, cannonBallParent);
        instanceBall = Instantiate(ball, transform.position, Quaternion.identity, cannonBallParent);
        instanceBall.GetComponent<Rigidbody2D>().velocity = power * transform.forward;
        var tr = Instantiate(trail, cannonBallParent);
        tr.cannonball = instanceBall.transform;

    }
}
