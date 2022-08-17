using Cynteract.SpaceInvaders;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#region Serializable classes
[System.Serializable]
public class EnemyWaves 
{

    [Tooltip("Enemy wave's prefab")]
    public GameObject wave;
}

#endregion

public class LevelController : MonoBehaviour {
    public int level = 1;

    //Serializable classes implements
    public EnemyWaves[] enemyWaves; 
    public EnemyWaves brickWave; 

    public GameObject obstacle;
    public GameObject bonus;
    public ShieldBonus shieldBonus;
    public HeartBonus heartBonus;
    public float timeForNewObstacle;
    public float timeForNewBonus;
    public int scoreBetweenHearts;
    private int lastScoreWithHeart;
    public GameObject[] planets;
    public float timeBetweenPlanets;
    public float planetsSpeed;
    List<GameObject> planetsList = new List<GameObject>();

    Camera mainCamera;
    public int waveNumber = 0;
    public int numberOfWaves;

    public Player playerPrefab;
    public Transform startPosition;
    public static LevelController instance;
    private void Awake()
    {
        PlayerPrefs.DeleteAll();
        instance = this;
    }
    public void SpawnWaves()
    {
        PlayerPrefs.SetInt("numberOfWaves", numberOfWaves);
        mainCamera = Camera.main;
        StartCoroutine(CreateEnemyWave(3, enemyWaves[0].wave));
        StartCoroutine(CreateBrickWave(3, brickWave.wave));
        StartCoroutine(ObstacleCreation());
        StartCoroutine(BonusCreation());
        //StartCoroutine(PlanetsCreation());
    }



    public void RespawnAfterSeconds(int v)
    {
        StartCoroutine(WaitAfterDeath(v));
    }


    
    IEnumerator CreateEnemyWave(float delay, GameObject Wave) 
    {
        if (delay != 0)
            yield return new WaitForSeconds(delay);
        if (Player.instance != null )
        {
            Instantiate(Wave);
            waveNumber++;
            StartCoroutine(CreateEnemyWave(7, enemyWaves[UnityEngine.Random.Range(0, enemyWaves.Length)].wave));
        }
    }
    IEnumerator CreateBrickWave(float delay, GameObject Wave)
    {
        if (delay != 0)
            yield return new WaitForSeconds(delay);
        if (Player.instance != null)
        {
            Instantiate(Wave);
            StartCoroutine(CreateBrickWave(20 + 0.05f * SpaceInvaders.instance.Money, brickWave.wave));
        }
    }

    IEnumerator ObstacleCreation() 
    {
        while (true) 
        {
            yield return new WaitForSeconds(timeForNewObstacle);

            Instantiate(obstacle, new Vector3(PlayerMoving.instance.transform.position.x, PlayerMoving.instance.borders.maxX, 
                mainCamera.ViewportToWorldPoint(Vector2.up).y + obstacle.GetComponent<Renderer>().bounds.size.y / 2 + 10),
                    Quaternion.identity);
        }
    }

    public void SpawnHeart()
    {
        Instantiate(heartBonus, GetMiddleSpawn(heartBonus.GetComponent<Renderer>()), Quaternion.identity);
    }

    IEnumerator BonusCreation()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeForNewBonus);
            Instantiate(shieldBonus, GetMiddleSpawn(shieldBonus.GetComponent<Renderer>()), Quaternion.identity);
        }
    }


    private Vector2 GetRandomSpawn(Renderer renderer)
    {
        return new Vector2(GetRandomX(), GetSpawnY(renderer));
    }
    private Vector2 GetMiddleSpawn(Renderer renderer)
    {
        return new Vector2(GetMiddleX(), GetSpawnY(renderer));
    }
    private static float GetRandomX()
    {
        return UnityEngine.Random.Range(PlayerMoving.instance.borders.minX, PlayerMoving.instance.borders.maxX);
    }
    private static float GetMiddleX()
    {
        return (PlayerMoving.instance.borders.minX +PlayerMoving.instance.borders.maxX)/2;
    }
    private float GetSpawnY(Renderer renderer)
    {
        return mainCamera.ViewportToWorldPoint(Vector2.up).y + renderer.bounds.size.y / 2 + 10;
    }

    IEnumerator PlanetsCreation()
    {
        for (int i = 0; i < planets.Length; i++)
        {
            planetsList.Add(planets[i]);
        }
        yield return new WaitForSeconds(10);
        while (true)
        {
            int randomIndex = UnityEngine.Random.Range(0, planetsList.Count);
            GameObject newPlanet = Instantiate(planetsList[randomIndex]);
            planetsList.RemoveAt(randomIndex);
            if (planetsList.Count == 0)
            {
                for (int i = 0; i < planets.Length; i++)
                {
                    planetsList.Add(planets[i]);
                }
            }
            newPlanet.GetComponent<DirectMoving>().speed = planetsSpeed;

            yield return new WaitForSeconds(timeBetweenPlanets);
        }
    }
    public IEnumerator WaitAfterDeath(int delay)
    {
        if (delay != 0)
            yield return new WaitForSeconds(delay);
        CreateNewWorld();
    }
    public void CreateBrickWave()
    {
        Instantiate(brickWave.wave);

    }
    public void CreateNewWorld()
    {
        //SceneManager.LoadScene("" + SceneManager.GetActiveScene().name);
        DestroyOldWorld();
        RespawnPlayer();
        ResetScore();
        StopAllCoroutines();
        SpawnWaves();
        


    }

    public static void ResetScore()
    {
        SpaceInvaders.instance.ResetScore();
    }

    public void RespawnPlayer()
    {
        if (Player.instance!=null)
        {
            Destroy(Player.instance.gameObject);
        }
        Player.instance = Instantiate(playerPrefab);
        Player.instance.transform.position = startPosition.position;
    }

    public void DestroyOldWorld()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] bricks = GetBricks();
        var bonusses = GameObject.FindGameObjectsWithTag("Bonus");
        var coins = GameObject.FindGameObjectsWithTag("Coin");
        var projectiles = GameObject.FindGameObjectsWithTag("Projectile");

        DestroyAll(enemies, bricks, bonusses, coins, projectiles);
    }
    public void StartBricks()
    {
        foreach (var item in GetBricks())
        {
            #warning hardcoded speed, use speed from prefab instead
            item.GetComponent<Brick>().brickSpeed = -1;
        }
    }
    public void StopBricks()
    {
        foreach (var item in GetBricks())
        {
            item.GetComponent<Brick>().brickSpeed = 0;
        }
    }
    private static GameObject[] GetBricks()
    {
        return GameObject.FindGameObjectsWithTag("Brick");
    }

    public void DestroyAll(params GameObject[][] gameObjects)
    {
        foreach (var item in gameObjects)
        {
            foreach (var gO in item)
            {
                Destroy(gO);
            }
        }
    }
    public void DestroyEnemiesInCircle(Vector3 position, float radius)
    {
        print("DestroyEnemies");
        var colliders = Physics2D.OverlapCircleAll(position, radius);
        foreach (var item in colliders)
        {
            Enemy enemy = item.gameObject.GetComponent<Enemy>();
            if (enemy)
            {
                enemy.Destruction();
            }
            Brick brick = item.gameObject.GetComponent<Brick>();
            if (brick)
            {
                brick.Destruction();

            }
            Projectile projectile = item.gameObject.GetComponent<Projectile>();
            if (projectile)
            {
                projectile.Destruction();
            }
            Obstacle obstacle = item.gameObject.GetComponent<Obstacle>();
            if (obstacle)
            {
                obstacle.Destruction();
            }
        }
    }
}

