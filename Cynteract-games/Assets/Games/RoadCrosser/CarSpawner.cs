using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.RoadCrosser
{
    public class CarSpawner : MonoBehaviour
    {
        public Car carPrefab;
        public float speed, interval;
        public const float RoadLength = 100;
        private float offset;
        public Vector3 direction = Vector3.right;
        public Transform leftSpawn, rightSpawn;
        private Vector3 spawnPoint;
        private void Start()
        {
            offset = UnityEngine.Random.Range(0, speed * interval);
            if (UnityEngine.Random.value > .5f)
            {
                spawnPoint = leftSpawn.position;
                direction = Vector3.right;
            }
            else
            {
                spawnPoint = rightSpawn.position;
                direction = Vector3.left;
            }
            PreSpawnCars();
            StartCoroutine(SpawnCarsCoroutine());
        }

        private void PreSpawnCars()
        {
            float stepDistance = speed * interval;
            int numberOfCars = ((int)(RoadLength / stepDistance)) + 1;
            for (int i = 0; i < numberOfCars; i++)
            {
                var car = Instantiate(carPrefab, spawnPoint + ((i * stepDistance) - offset) * direction, Quaternion.identity, transform);
                car.DestroyAfterSeconds((RoadLength + offset - (i * stepDistance)) / speed);

                car.speed = speed;
                car.direction = direction;
            }
        }

        IEnumerator SpawnCarsCoroutine()
        {
            yield return new WaitForSeconds(interval);

            while (true)
            {
                var car = Instantiate(carPrefab, spawnPoint - direction * offset, Quaternion.identity, transform);
                car.speed = speed;
                car.direction = direction;

                car.DestroyAfterSeconds((RoadLength + offset) / speed);
                yield return new WaitForSeconds(interval);
            }
        }

    }

}