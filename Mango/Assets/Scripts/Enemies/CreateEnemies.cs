using Photon.Pun;
using System.Collections;
using UnityEngine;
using Mango.Game;
using UnityEngine.AI;


public class CreateEnemies : MonoBehaviour
{
    public int enemiesPerSpawn = 3;
    public float spawnTime = 30f;
    public EnemySpawn[] enemySpawn;
    private Transform enemyParent;
    private LayerMask buildings, ground;

    public int enemyLimit;
    void Start()
    {
        enemyParent = GameObject.Find("Enemies").transform;
        buildings = LayerMask.GetMask("Buildings");
        ground = LayerMask.GetMask("Floor");

        if (PhotonNetwork.IsMasterClient)
        {
            InvokeRepeating(nameof(Spawn), spawnTime, spawnTime);
        }
    }
    void Spawn()
    {
        if (RoomController.Instance.currentEnemies >= enemyLimit)
            return;

        for (int i = 0; i < enemiesPerSpawn; i++)
        {
            GameObject newEnemyPrefab = GetRandomEnemy().prefab;


            // TODO: Get area of map
            float numx = Random.Range(-100, 100);
            float numy = Random.Range(-100, 100);

            Vector3 posicion = new Vector3(numx, 0f, numy);
            Vector3 resultPos = GetRandomPoint(posicion, 50f);

            GameObject newEnemy = RoomController.Instance.Spawn("Enemies/" + newEnemyPrefab.name, resultPos);
            newEnemy.transform.SetParent(enemyParent);
            RoomController.Instance.IncreaseCurrentEnemiesCount();
        }
    }


    public static Vector3 GetRandomPoint(Vector3 center, float maxDistance)
    {
        Vector3 randomPos = Random.insideUnitSphere * maxDistance + center;

        NavMeshHit hit;

        NavMesh.SamplePosition(randomPos, out hit, maxDistance, NavMesh.AllAreas);

        return hit.position;
    }

    EnemySpawn GetRandomEnemy()
    {
        var total = 0f;
        foreach (var enemy in enemySpawn)
        {
            total += enemy.spawnChance;
        }

        var random = Random.value * total;

        var current = 0f;
        foreach (var element in enemySpawn)
        {
            if (current <= random && random < current + element.spawnChance)
            {
                return element;
            }
            current += element.spawnChance;
        }

        return enemySpawn[enemySpawn.Length - 1];
    }

    [System.Serializable]
    public class EnemySpawn
    {
        public GameObject prefab;
        [Range(0.0f, 100f)]
        public float spawnChance;
    }

}
