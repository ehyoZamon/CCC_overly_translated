using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.Tunnelrunner {
    public class PartGenerator : MonoBehaviour {

        MeshRenderer meshRenderer;
        public MeshFilter meshFilterUpperPart;
        public MeshFilter meshFilterLowerPart;
        public MeshCollider meshColliderUpper;
        public MeshCollider meshColliderLower;
        public PartOfPart lowerPart, upperPart;
        BoxCollider2D boxCollider;
        public WorldGenerationSettings worldGenerationSettings;

        public void SpawnPart()
        {
            float[] offsets = new float[worldGenerationSettings.stepsPerPart];
            float[] diameters = new float[worldGenerationSettings.stepsPerPart];

            offsets[0] = GetlastOffset();
            diameters[0] = GetLastDiameter();
            for (int i = 1; i < worldGenerationSettings.stepsPerPart; i++)
            {

                offsets[i] = UnityEngine.Random.Range(-LerpedOffset(worldGenerationSettings, i), LerpedOffset(worldGenerationSettings, i));
                diameters[i] = Mathf.Lerp(GetLastDiameter(), worldGenerationSettings.CurrentDiameter(), (float)i / worldGenerationSettings.stepsPerPart);
                SetLastOffset(offsets[i]);
            }
            Vector3[] spawnPoints;
            Mesh upperMesh;
            Mesh lowerMesh;
            ProceduralMesh.Tunnel2D(worldGenerationSettings.celling, worldGenerationSettings.bottom, worldGenerationSettings.stepDistance, offsets, diameters, worldGenerationSettings.interpolationSteps, 2, out upperMesh, out lowerMesh, out spawnPoints);
            meshFilterUpperPart.mesh = upperMesh;
            meshFilterLowerPart.mesh = lowerMesh;
            Vector3[] respawnPoints = new Vector3[offsets.Length];


            for (int i = 0; i < respawnPoints.Length; i++)
            {
                respawnPoints[i] = new Vector3(i * worldGenerationSettings.stepDistance + transform.position.x, offsets[i], 0);
            }
            Tunnelrunner.instance.levelStats.points.Add(respawnPoints);
            if (ThereAreItems())
            {
                for (int i = 0; i < spawnPoints.Length; i += worldGenerationSettings.itemDistance)
                {
                    Transform item = GetItemToSpawn();
                    Instantiate(item, transform.position + spawnPoints[i] + UnityEngine.Random.Range(-worldGenerationSettings.CurrentOffset() / 2, worldGenerationSettings.CurrentOffset() / 2) * Vector3.up, item.rotation, transform);
                }
            }


            lowerPart.GeneratePolyCollider(lowerMesh);
            upperPart.GeneratePolyCollider(upperMesh);


            SetBoxCollider();
            SetLastOffset(offsets[offsets.Length - 1]);
            SetLastDiameter(diameters[diameters.Length - 1]);

        }

        private bool ThereAreItems()
        {
            LevelSet.Lvl currentLevel =

                 Tunnelrunner.instance.worldGenerator.levels.lvls[Tunnelrunner.instance.levelStats.currentLevel];

            if (currentLevel.items.Length == 0)
            {
                return false;
            }
            return true;
        }
        public Transform GetItemToSpawn()
        {
            LevelSet.Lvl currentLevel =

                Tunnelrunner.instance.worldGenerator.levels.lvls[Tunnelrunner.instance.levelStats.currentLevel];

            List<Transform> objects = new List<Transform>();
            for (int i = 0; i < currentLevel.items.Length; i++)
            {
                for (int j = 0; j < currentLevel.items[i].probability; j++)
                {
                    objects.Add(currentLevel.items[i].objectsToSpawn);
                }
            }

            return objects[UnityEngine.Random.Range((int)0, objects.Count)];
        }
        private void SetBoxCollider()
        {
            boxCollider.size = new Vector3(worldGenerationSettings.stepDistance * (worldGenerationSettings.stepsPerPart - 1), Mathf.Abs(worldGenerationSettings.celling) + Mathf.Abs(worldGenerationSettings.bottom), 1);
            boxCollider.offset = new Vector3(worldGenerationSettings.stepDistance * (worldGenerationSettings.stepsPerPart - 1) / 2, 0, 0);        }

        private float LerpedOffset(WorldGenerationSettings worldSets, int i)
        {
            return Mathf.Lerp(GetlastOffset(), worldSets.CurrentOffset(), (float)i / worldSets.stepsPerPart);
            
        }

        private float GetlastOffset()
        {
            return Tunnelrunner.instance.levelStats.lastOffset;
        }

        private float GetLastDiameter()
        {
            return Tunnelrunner.instance.levelStats.lastDiameter;
        }


        private void SetLastOffset(float offset)
        {
            Tunnelrunner.instance.levelStats.lastOffset = offset;

        }
        private void SetLastDiameter(float diameter)
        {
            Tunnelrunner.instance.levelStats.lastDiameter = diameter;
        }
        private void Awake()
        {
            boxCollider = GetComponent<BoxCollider2D>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.tag == "Player")
            {
                Tunnelrunner.instance.worldGenerator.SpawnNewPart();
            }
        }
    }
}