using Photon.Pun;

using UnityEditor;
using UnityEngine;

public class CreateEnemies : MonoBehaviour
{
    public int limit = 1;
    private int count = 0;
    public float spawnTime = 30f;
    public EnemySpawn[] enemySpawn;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            InvokeRepeating(nameof(Spawn), spawnTime, spawnTime);
        }
    }
    // Update is called once per frame
    void Spawn()
    {
        if (count >= limit)
            return;

        count++;
        float numx = (Random.Range(-10,10));
        float numy = Random.Range(-10,10);
        
        Vector3 posicion = new Vector3(numx,0f,numy);

        GameObject newEnemyPrefab = GetRandomEnemy().prefab;
        PhotonNetwork.Instantiate(newEnemyPrefab.name, posicion, gameObject.transform.rotation);  
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
