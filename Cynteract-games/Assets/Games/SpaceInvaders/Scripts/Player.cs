using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cynteract.SpaceInvaders;

/// <summary>
/// This script defines which sprite the 'Player" uses and its health.
/// </summary>

public class Player : MonoBehaviour
{
    public GameObject destructionFX, brokenHeart;
    public GameObject PuppySprite;
    public AudioSource DeathSound;

    public static Player instance;
    public AudioSource[] CoinSound;
    public PlayerMoving playerMoving;
    public PlayerShooting playerShooting;
    public int coins = 0;
    public int puppyLevel = 0;
    public int lives = 3;
    public Shield shield;

    public static bool canPlayerDie = true;
    private void Awake()
    {
        if (instance == null) 
            instance = this;
        canPlayerDie = true;
    }
    private void Start()
    {
        SpaceInvaders.instance.PlayerSpawned();
        LivesDisplayer.instance.Init(lives);
    }
    public void ActivateShield()
    {
        shield.Activate();
    }
    public void Heal(int damage)
    {
        if (lives<3)
        {
            lives+=damage;
            LivesDisplayer.instance.UpdateLives(lives);
        }


    }
    public void GetDamage(int damage)   
    {
        SpaceInvaders.instance.PlayerGotDamaged();
        if (canPlayerDie)
        {
            canPlayerDie = false;
            lives--;
            if(lives > 0)
            {
                LivesDisplayer.instance.UpdateLives(lives);
                DestroyEnemies();
                Instantiate(brokenHeart, transform.position, Quaternion.identity);
                Instantiate(destructionFX, transform.position, Quaternion.identity);

            }
            else
            {
                lives = 0;
                LivesDisplayer.instance.UpdateLives(lives);
                Instantiate(brokenHeart, transform.position, Quaternion.identity);

                Instantiate(destructionFX, transform.position, Quaternion.identity);
                Destruction();
                SpaceInvaders.instance.PlayerDied();
            }
            canPlayerDie = true;

        }

    }

    private void DestroyEnemies()
    {
        LevelController.instance.DestroyEnemiesInCircle(transform.position, 10);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Coin")
        {
            CoinSound[Random.Range(0, CoinSound.Length)].Play();
        }
    }
    //'Player's' destruction procedure
    void Destruction()
    {
        LevelController.instance.RespawnAfterSeconds(2);
        DeathSound.Play();
        Instantiate(destructionFX, transform.position, Quaternion.identity); //generating destruction visual effect and destroying the 'Player' object
        Destroy(gameObject);
    }
    public void Update()
    {
        
    }
}
















