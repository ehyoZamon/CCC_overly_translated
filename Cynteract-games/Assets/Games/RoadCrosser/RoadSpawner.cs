using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cynteract.RoadCrosser
{
    public class RoadSpawner : MonoBehaviour
    {
        public Road[] roadPrefabs;
        public Road[] emptyRoadPrefabs, carRoadPrefabs;
        public float distance;
        public List<Road> roads;

        public const int RoadBuffer = 5;

        public PlayerPacket playerPrefab, player;
        public static RoadSpawner instance;
        System.Random random = new System.Random();
        int currentRoadIndex = 0;
        private void Awake()
        {
            instance = this;
        }
        private void Start()
        {

            SpawnWorld();
        }

        private void SpawnWorld()
        {
            currentRoadIndex = 0;

            roads = new List<Road>();

            for (int i = 0; i < RoadBuffer; i++)
            {
                SpawnNewRoad(currentRoadIndex);
                currentRoadIndex++;
            }

            Vector3 position = transform.position + (currentRoadIndex * distance * Vector3.up);
            player = Instantiate(playerPrefab, position, Quaternion.identity, transform);
            player.player.nextPosition = position;
            roads.Add(Instantiate(emptyRoadPrefabs[0], position, Quaternion.identity, transform));
            currentRoadIndex++;
            for (int i = 0; i < RoadBuffer; i++)
            {
                SpawnNewRoad(currentRoadIndex);
                currentRoadIndex++;
            }
        }

        private Road SpawnNewRoad(int i)
        {
            Road roadToSpawn = roadPrefabs[random.Next(roadPrefabs.Length)];
            Road road = Instantiate(roadToSpawn, transform.position + (i * distance * Vector3.up), Quaternion.identity, transform);
            roads.Add(road);
            return road;
        }

        public void AddNewRoad()
        {
            var road = roads[RoadBuffer];
            SpawnNewRoad(currentRoadIndex);
            currentRoadIndex++;
            Destroy(roads[0].gameObject);
            roads.RemoveAt(0);
            switch (road)
            {
                case EmptyRoad _:
                    RoadCrossing.instance.ScorePoints(50);
                    break;
                case CarRoad _:
                    RoadCrossing.instance.ScorePoints(150);
                    break;
            }

        }

        public void Respawn()
        {
            RoadCrossing.instance.ResetScore();
            Destroy(player.gameObject);
            for (int i = 0; i < roads.Count; i++)
            {
                Destroy(roads[i].gameObject);
            }
            SpawnWorld();
        }

    }
}