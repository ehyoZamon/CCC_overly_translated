using Cynteract.SpaceInvaders;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This script defines 'Enemy's' health and behavior. 
/// </summary>
public class Brick : MonoBehaviour {

    #region FIELDS
    [Tooltip("Health points in integer")]
    public int health;
    public int money;
    public float brickSpeed;

    [Tooltip("VFX prefab generating after destruction")]
    public GameObject destructionVFX;
    public GameObject hitEffect;
    public GameObject Coin;
    #endregion

    public void Start()
    {
        money = SpaceInvaders.instance.Money;
        health = 1 + (money / 10);
        if (money < 500)
        {
            brickSpeed = -1.2f - (0.004f * money);
        }
    }
    public void GetDamage(int damage) 
    {
        health -= damage;
        if (health <= 0)
            Destruction();
        else
            Instantiate(hitEffect,transform.position,Quaternion.identity,transform);
    }    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Player.instance.GetDamage(1);
        }
    }

    public void Destruction()
    {
        if (Random.Range(0, 2) == 1)
            Instantiate(Coin, transform.position, Quaternion.identity);
        SoundManger.instance.PlayRandomClipWithVirtualSource("EnemyDestroy",.1f);

        Instantiate(destructionVFX, transform.position, Quaternion.identity); 
        Destroy(gameObject);
    }

    void Update()
    {
        if (this.gameObject.tag == "Brick")
        {
            if (health < 10)
                gameObject.GetComponent<SpriteRenderer>().color = new Color(0.3f, 0.3f, 0.3f);
            else if (health < 30)
                gameObject.GetComponent<SpriteRenderer>().color = new Color(0.6f, 0.6f, 0.6f);
            else if (health < 60)
                gameObject.GetComponent<SpriteRenderer>().color = new Color(0.8f, 0.8f, 0.8f);
            else if (health < 90)
                gameObject.GetComponent<SpriteRenderer>().color = new Color(0.9f, 0.9f, 0.9f);
            else if (health < 150)
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        }

        gameObject.transform.Translate(Vector3.up * (brickSpeed * Time.deltaTime));
    }
}
