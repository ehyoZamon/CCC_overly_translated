using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booster : MonoBehaviour
{
    new ParticleSystem particleSystem;
    new Collider2D collider;
    private void Awake()
    {
        particleSystem = GetComponent<ParticleSystem>();
        collider = GetComponent<Collider2D>();
    }
    public void Boost()
    {

        var em = particleSystem.emission;
        em.enabled = true;
        collider.enabled = true;
    }
    public void StopBoosting()
    {



            var em = particleSystem.emission;
            em.enabled = false;
            collider.enabled = false;

    }
}
