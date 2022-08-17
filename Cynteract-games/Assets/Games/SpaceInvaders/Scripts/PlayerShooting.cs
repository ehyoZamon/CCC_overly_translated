using Cynteract.SpaceInvaders;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//guns objects in 'Player's' hierarchy
[System.Serializable]
public class Guns
{
    public GameObject rightGun, leftGun, centralGun;
    [HideInInspector] public ParticleSystem leftGunVFX, rightGunVFX, centralGunVFX; 
}

public class PlayerShooting : MonoBehaviour {

    [Tooltip("shooting frequency. the higher the more frequent")]
    public float fireRate;

    [Tooltip("projectile prefab")]
    public GameObject projectileObject;

    //time for a new shot
    [HideInInspector] public float nextFire;

    public int gunsLevel = 0; 
    public int dynamicGunsLevel = 0; 

    public Guns guns;
    public bool shootingIsActive = false; 
    [HideInInspector] public int maxweaponPower = 4; 
    public static PlayerShooting instance;
    public AudioSource ShootSound;
    public Text Damage;
    private bool justShot;
    public GameObject loadedGlow;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    private void Start()
    {
        projectileObject.GetComponent<Projectile>().damage = 4;
        //receiving shooting visual effects components
        guns.leftGunVFX = guns.leftGun.GetComponent<ParticleSystem>();
        guns.rightGunVFX = guns.rightGun.GetComponent<ParticleSystem>();
        guns.centralGunVFX = guns.centralGun.GetComponent<ParticleSystem>();
        
        
    }

    public void IncreaseGunLevel()
    {
        gunsLevel++;
        if (gunsLevel >= 4)
        {
            projectileObject.GetComponent<Projectile>().damage += 2;
        }
    }
    public void Update()
    {
        //Damage.text = "Damage: " + projectileObject.GetComponent<Projectile>().damage;
        bool shootAction = SpaceInvaderInput.GetAction(SpaceInvaderInput.shoot);
        if (shootAction ) 
        {
            if (Time.time > nextFire && !justShot)
            {

                justShot = true;
                loadedGlow.SetActive(false);
                StartCoroutine(ShootXTimes(5,.12f));
                nextFire = Time.time + 1 / fireRate;
            }
        }
        else
        {
            justShot = false;
            loadedGlow.SetActive(true);

        }

    }
   
    IEnumerator ShootXTimes(int number, float delay)
    {
        for (int i = 0; i < number; i++)
        {
            MakeAShot();
            yield return new WaitForSeconds(delay);
        }
    }
    void MakeAShot()
    {
        ShootSound.Play();

        if (gunsLevel < 5)
        {
            dynamicGunsLevel = gunsLevel;
        }
        else
        {
            dynamicGunsLevel = 4;
        }
        switch (dynamicGunsLevel)
        {
            case 0:
                CreateLazerShot(projectileObject, guns.centralGun.transform.position, Vector3.zero);
                //guns.centralGunVFX.Play();
                break;
            case 1:
                CreateLazerShot(projectileObject, guns.rightGun.transform.position, Vector3.zero);
                //guns.leftGunVFX.Play();
                CreateLazerShot(projectileObject, guns.leftGun.transform.position, Vector3.zero);
                //guns.rightGunVFX.Play();
                break;
            case 2:
                CreateLazerShot(projectileObject, guns.centralGun.transform.position, Vector3.zero);
                CreateLazerShot(projectileObject, guns.rightGun.transform.position, new Vector3(0, 0, -3));
               // guns.leftGunVFX.Play();
                CreateLazerShot(projectileObject, guns.leftGun.transform.position, new Vector3(0, 0, 3));
               //guns.rightGunVFX.Play();
                break;
            case 3:
                CreateLazerShot(projectileObject, guns.rightGun.transform.position, new Vector3(0, 0, 0));
               // guns.leftGunVFX.Play();
                CreateLazerShot(projectileObject, guns.leftGun.transform.position, new Vector3(0, 0, 0));
                //guns.rightGunVFX.Play();
                CreateLazerShot(projectileObject, guns.leftGun.transform.position, new Vector3(0, 0, 4));
                CreateLazerShot(projectileObject, guns.rightGun.transform.position, new Vector3(0, 0, -4));
                break;
            case 4:
                CreateLazerShot(projectileObject, guns.centralGun.transform.position, Vector3.zero);
                CreateLazerShot(projectileObject, guns.rightGun.transform.position, new Vector3(0, 0, -1));
                // guns.leftGunVFX.Play();
                CreateLazerShot(projectileObject, guns.leftGun.transform.position, new Vector3(0, 0, 1));
                // guns.rightGunVFX.Play();
                CreateLazerShot(projectileObject, guns.leftGun.transform.position, new Vector3(0, 0, 7));
                CreateLazerShot(projectileObject, guns.rightGun.transform.position, new Vector3(0, 0, -7));
                break;
        }
    }

    void CreateLazerShot(GameObject lazer, Vector3 pos, Vector3 rot)
    {
        Instantiate(lazer, pos, Quaternion.Euler(rot));
    }
}
