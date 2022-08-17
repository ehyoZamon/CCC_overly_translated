using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellEmmitor : MonoBehaviour
{
    #region FIELDS
    [Tooltip("Enemy's projectile prefab")]
    public GameObject Projectile;

    [Tooltip("current weapon")]
    [Range(1, 4)]
    public int weapon = 1;

    [Tooltip("current weapon")]
    [Range(0.1f, 10)]
    public float shotTime = 0.5f;

    #endregion

    private int angle;
    public int gunAngle;
    public int angleSpeed;
    public float delay = 1;
    public bool randomAngle = false;
    public bool lazerOn = true;
    void Start()
    {
        InvokeRepeating("BasicShooting", delay, shotTime);

    }

    //coroutine making a shot
    void BasicShooting()
    {
        if (lazerOn)
        {
            if (randomAngle)
            {
                Instantiate(Projectile, gameObject.transform.position, Quaternion.Euler(0, 0, Random.Range(-10, 10)));
            }
            else
                Instantiate(Projectile, gameObject.transform.position, gameObject.transform.rotation);
        }
    }
}
