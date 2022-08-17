using Cynteract.SpaceInvaders;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This script defines 'Enemy's' health and behavior. 
/// </summary>
public class Enemy : MonoBehaviour {

    #region FIELDS
    [Tooltip("Health points in integer")]
    public int health;

    [Tooltip("Enemy's projectile prefab")]
    public Projectile Projectile;

    [Tooltip("VFX prefab generating after destruction")]
    public GameObject destructionVFX;
    public GameObject hitEffect;
    public GameObject Coin;
    [HideInInspector] public int shotChance; //probability of 'Enemy's' shooting during tha path
    [HideInInspector] public float shotTimeMin, shotTimeMax; //max and min time for shooting from the beginning of the path
    #endregion

    public AudioSource[] HitSound;
    int money;

    private void Start()
    {
        money=SpaceInvaders.instance.Money;
        Invoke("ActivateShooting", Random.Range(shotTimeMin, shotTimeMax));
        health = 1 + (money / 10);
    }

    //coroutine making a shot
    void ActivateShooting() 
    {
        if (Random.value < (float)shotChance / (100 - (money / 10)))              
        {
            var projectile=Instantiate(Projectile, gameObject.transform.position-Vector3.up, Quaternion.Euler(0, 0, Random.Range(-10, 10)));
            projectile.owner = this;
        }
    }

    public void GetDamage(int damage) 
    {
        if(gameObject != null)
            //HitSound[Random.Range(0, HitSound.Length)].Play();
        health -= damage;
        GetComponent<Animation>().Play();
        if (health <= 0)
            Destruction();
        else
            Instantiate(hitEffect,transform.position,Quaternion.identity,transform);
    }    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (Projectile.GetComponent<Projectile>() != null)
                Player.instance.GetDamage(Projectile.GetComponent<Projectile>().damage);
            else
            {
                Player.instance.GetDamage(1);
            }
        }
    }
    public void Destruction()                           
    {
        SoundManger.instance.PlayRandomClipWithVirtualSource("EnemyDestroy",.1f);
        Instantiate(destructionVFX, transform.position, Quaternion.identity);
        Instantiate(Coin, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

}
