using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Mango.Game;

public class createPowerups : MonoBehaviour
{
    public float spawnTime = 15f;
    public PowerupSpawn[] powerupSpawn;

    public GameObject prefabLife;
    
    
    void Start()
    {
        if(PhotonNetwork.IsMasterClient)
            InvokeRepeating("Spawn", spawnTime, spawnTime);
    }
    // Update is called once per frame
    void Spawn()
    {
        GameObject newPowerupPrefab = GetRandomPowerup().prefab;

        // TODO: Get area of map
        float numx = Random.Range(-100, 100);
        float numy = Random.Range(-100, 100);

        Vector3 posicion = new Vector3(numx, 0f, numy);
        Vector3 resultPos = GetRandomPoint(posicion, 50f);

        GameObject newEnemy = RoomController.Instance.Spawn(newPowerupPrefab.name, resultPos);
        RoomController.Instance.IncreaseCurrentEnemiesCount();
    }


    public static Vector3 GetRandomPoint(Vector3 center, float maxDistance)
    {
        Vector3 randomPos = Random.insideUnitSphere * maxDistance + center;

        NavMeshHit hit;

        NavMesh.SamplePosition(randomPos, out hit, maxDistance, NavMesh.AllAreas);

        return hit.position;
    }

    PowerupSpawn GetRandomPowerup()
    {
        var total = 0f;
        foreach (var powerup in powerupSpawn)
        {
            total += powerup.spawnChance;
        }

        var random = Random.value * total;

        var current = 0f;
        foreach (var element in powerupSpawn)
        {
            if (current <= random && random < current + element.spawnChance)
            {
                return element;
            }
            current += element.spawnChance;
        }

        return powerupSpawn[powerupSpawn.Length - 1];
    }

    [System.Serializable]
    public class PowerupSpawn
    {
        public GameObject prefab;
        [Range(0.0f, 100f)]
        public float spawnChance;
    }

}
