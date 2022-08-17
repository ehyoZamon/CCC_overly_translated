using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Tunnelrunner {
    public class WorldGenerator : MonoBehaviour {
        Vector3 lastPartPositionOffset = Vector3.zero;
        Vector3 spawnPoint = Vector3.zero;
        public PartGenerator partGenerator;
        public WorldGenerationSettings worldGenerationSettings;
        public LevelSet levels;
        private void Awake()
        {
            Tunnelrunner.instance.worldGenerator = this;
            

        }
        public void SpawnNewWorld()
        {
            /*GameObject[] temps = GameObject.FindGameObjectsWithTag("temp");
            for (int i = 0; i < temps.Length; i++)
            {
                Destroy(temps[i]);
            }*/
            lastPartPositionOffset = -worldGenerationSettings.stepDistance * worldGenerationSettings.stepsPerPart * Vector3.right;
            DestroyOldParts();
            SpawnNewPart();
            SpawnNewPart();

        }

        private void DestroyOldParts()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
        }

        public void SpawnNewPart()
        {

            var part=Instantiate(partGenerator, lastPartPositionOffset, Quaternion.identity, transform);
            part.worldGenerationSettings = worldGenerationSettings;
            part.SpawnPart();
            lastPartPositionOffset += ((worldGenerationSettings.stepsPerPart - 1) * worldGenerationSettings.stepDistance * Vector3.right);
            if (transform.childCount > 4)
            {
                DestroyOldestPart();
            }
        }

        private void DestroyOldestPart()
        {
            Destroy(transform.GetChild(0).gameObject);
            try
            {
                Tunnelrunner.instance.levelStats.points.RemoveAt(0);

            }
            catch
            {


            }
        }
    }
}