using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Sphererunner
{
    public class WorldGenerator : MonoBehaviour, ILevelLoader
    {
        public Level level;
        Vector3 lastEndpoint;
        public Vector3 lastLevelSwitcher;
        public Transform tutorialLevel;
        public bool twoDimensionalGraphics;
        public static WorldGenerator instance;
        private int currentLevel;
        public CinemachineVirtualCamera virtualCamera;
        private void Awake()
        {
            instance = this;
            LevelLoading.loader = this;
        }
        public void NextLevel()
        {
            Sphererunner.levelsPlayed++;
            currentLevel++;
            //GlobalValues.backgroundController.changeBackground();
            //GlobalValues.musicController.changeMusic();
            Sphererunner.instance.NextLevel();
            SpawnNextLevel();
        }

        public void Restart()
        {
            SpawnNewLevel();
        }
        public void ResetPlayer()
        {
            SphereMovement.instance.Spawn(lastLevelSwitcher);
        }

        private void SpawnNextLevel(int except=2)
        {
            DestroyOldLevelExcept(except);
            Transform[] partsToSpawn = GetPartsToSpawn(Mathf.Min(currentLevel, level.lvls.Length - 1));
            for (int i = 0; i < partsToSpawn.Length; i++)
            {
                Transform spawnedItem = Instantiate(partsToSpawn[i], lastEndpoint, Quaternion.identity, transform);
                lastEndpoint = spawnedItem.GetChild(0).position;
                Transform flatPart = Instantiate(level.flatPart, lastEndpoint, Quaternion.identity, transform);
                lastEndpoint = flatPart.GetChild(0).position;
            }
            Transform endPart = Instantiate(level.endpart, lastEndpoint, Quaternion.identity, transform);
            lastEndpoint = endPart.GetChild(0).position;

        }

        public void SpawnNewLevel()
        {
            SpawnLevel(0);
        }
        public void SpawnNewLevelWithTutorial()
        {
            lastLevelSwitcher = (3 * Vector3.up + Vector3.right);
            lastEndpoint = Vector3.zero;
            DestroyOldLevel();
            
            currentLevel = 0;
            SpawnStartPart();
            SpawnTutorial();
            SpawnPlayer();
        }

        private void SpawnTutorial()
        {
            Transform spawnedItem = Instantiate(tutorialLevel, lastEndpoint, Quaternion.identity, transform);
            lastEndpoint = spawnedItem.GetChild(0).position;
            Transform endPart = Instantiate(level.endpart, lastEndpoint, Quaternion.identity, transform);
            lastEndpoint = endPart.GetChild(0).position;
        }

        public void LoadLevel(int level)
        {
            SpawnLevel(level);
        }
        public int GetNumberOfLevels()
        {
            return level.lvls.Length;
        }
        public void SpawnLevel(int levelIndex)
        {
            lastLevelSwitcher = (3 * Vector3.up + Vector3.right);
            lastEndpoint = Vector3.zero;
            DestroyOldLevel();

            currentLevel = levelIndex;
            SpawnStartPart();
            //GlobalValues.musicController.changeMusic();
            SpawnNextLevel();
            SpawnPlayer();
        }

        private void SpawnStartPart()
        {
            Transform spawnedItem = Instantiate(level.startPart, lastEndpoint, Quaternion.identity, transform);
            lastEndpoint = spawnedItem.GetChild(0).position;
        }

        private void DestroyOldLevel()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
        private void DestroyOldLevelExcept(int amount)
        {
            int childCount = transform.childCount;
            for (int i = 0; i < childCount - amount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }
        private void SpawnPlayer()
        {
            SphereMovement.instance.Spawn(3 * Vector3.up + Vector3.right);

            SphereMovement.instance.autoDirection = 1;
        }


        private Transform[] GetPartsToSpawn(int levelIndex)
        {
            Transform[] newParts;
            Transform[] allParts = level.currentParts(levelIndex, out newParts);


            List<Transform> partsToSpawn = new List<Transform>();

            for (int i = 0; i < newParts.Length; i++)
            {
                partsToSpawn.Insert(UnityEngine.Random.Range(0, partsToSpawn.Count), newParts[i]);
            }

            while (partsToSpawn.Count < level.lvls[levelIndex].numberOfParts)
            {
                partsToSpawn.Insert(
                    UnityEngine.Random.Range(0, partsToSpawn.Count),
                    allParts[UnityEngine.Random.Range(0, allParts.Length)]
                    );
            }
            return partsToSpawn.ToArray();
        }


    }
}