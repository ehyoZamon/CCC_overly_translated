using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

/// <summary>
/// This script generates an enemy wave. It defines how many enemies will be emerging, their speed and emerging interval. 
/// It also defines their shooting mode. It defines their moving path.
/// </summary>

public class BrickWave : MonoBehaviour {

    #region FIELDS
    [Tooltip("Enemy's prefab")]
    public GameObject enemy;

    public Transform[] spawnPoints;
    #endregion

    void Start()
    {
        //enemy.GetComponentInChildren<SpriteRenderer>().color = new Vector4(UnityEngine.Random.Range(0, 256), UnityEngine.Random.Range(0, 256), UnityEngine.Random.Range(0, 256), 255);
        for(int i = 0; i < spawnPoints.Length; i++)
        {
            if(UnityEngine.Random.Range(0, 2) < 1)
            {
                Instantiate(enemy, spawnPoints[i].position, Quaternion.identity, LevelController.instance.transform);
            }
        }
    }
}
