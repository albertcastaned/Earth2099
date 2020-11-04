using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createEnemies : MonoBehaviour
{
    public GameObject prefabEnemy;
    public float spawnTime = 15f;


    void Start()
    {
        InvokeRepeating("Spawn", spawnTime, spawnTime);
    }
    // Update is called once per frame
    void Spawn()
    {
        
        float numx = (Random.Range(0,30));
        float numy = Random.Range(0,30);
        
        Vector3 posicion= new Vector3(numx,0.5f,numy);
        var objecto = Instantiate(prefabEnemy, posicion, gameObject.transform.rotation);

    }
}
