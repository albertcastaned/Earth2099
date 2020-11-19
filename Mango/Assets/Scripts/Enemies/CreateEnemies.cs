using Photon.Pun;
using System.Collections;
using UnityEngine;
using Mango.Game;
using UnityEngine.AI;


public class CreateEnemies : MonoBehaviour
{
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
    // Update is called once per frame
    void Spawn()
    {
        if (RoomController.Instance.currentEnemies >= enemyLimit)
            return;
        // TODO: Get area of map
        float numx = (Random.Range(-100, 100));
        float numy = Random.Range(-100, 100);
        GameObject newEnemyPrefab = GetRandomEnemy().prefab;

        Vector3 posicion = new Vector3(numx, 0f, numy);

        Vector3 randomNavMeshPos = Random.insideUnitSphere * 300f + posicion;


        NavMesh.SamplePosition(randomNavMeshPos, out NavMeshHit hit, 300f, NavMesh.AllAreas);

        GameObject newEnemy = RoomController.Instance.Spawn(newEnemyPrefab.name, hit.position);
        newEnemy.transform.SetParent(enemyParent);
        RoomController.Instance.IncreaseCurrentEnemiesCount();

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
