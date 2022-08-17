using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Cynteract.Platformer
{
    public class LevelController : MonoBehaviour
    {
        private string[] scenes;
        public string currentLevel;
        private int currentIndex;
        public PlayerMovement player;
        public static LevelController instance;
        public bool autoLoad;
        public bool respawning = false;
        private AssetBundle myLoadedAssetBundle;

        private void Awake()
        {
            instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        private void Start()
        {
            if (autoLoad) LoadLevels();
        }

        private void LoadLevels()
        {
            Debug.Log(Application.streamingAssetsPath);
           
            myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "jumpandrunlevels"));
            if (myLoadedAssetBundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
                return;
            }  
            scenes = myLoadedAssetBundle.GetAllScenePaths();
            SpawnFirstLevel();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                if (currentIndex < scenes.Length - 1)
                {
                    SpawnNextLevel();
                }
            }
        }
        public void SpawnNextLevel()
        {
            if (currentIndex >= scenes.Length - 1)
            {
                currentIndex = -1;
            }
            SpawnLevel(currentIndex + 1);
        }
        public void Respawn()
        {
            if (respawning)
            {
                return;
            }
            respawning = true;
            JumpAndRun.instance.ResetLevelScore();
            SceneManager.UnloadSceneAsync(currentLevel);
            SceneManager.LoadSceneAsync(currentLevel, LoadSceneMode.Additive).completed += x => respawning = false;
        }
        private void SpawnLevel(int index)
        {

            player.Freeze();
            SceneManager.UnloadSceneAsync(currentLevel);
            currentIndex = index;
            currentLevel = scenes[index];
            SceneManager.LoadScene(currentLevel, LoadSceneMode.Additive);
        }
        private void SpawnFirstLevel()
        {

            player.Freeze();
            currentIndex = 0;
            currentLevel = scenes[0];
            SceneManager.LoadScene(currentLevel, LoadSceneMode.Additive);
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            SceneManager.SetActiveScene(arg0);
            player.transform.position = Vector3.zero;
            player.Unfreeze();
            player.DeactivatePartay();
        }
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            myLoadedAssetBundle.Unload(true);
        }
    }
}