using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createPowerups : MonoBehaviour
{
    public GameObject prefabLife;
        public float spawnTime = 30f;
    
    
        void Start()
        {
            if(PhotonNetwork.IsMasterClient)
                InvokeRepeating("Spawn", spawnTime, spawnTime);
        }
        // Update is called once per frame
        void Spawn()
        {
            
            float numx = (Random.Range(-10,10));
            float numy = Random.Range(10,10);
            
            Vector3 posicion= new Vector3(numx,2f,numy);
            var objecto = PhotonNetwork.Instantiate(prefabLife.name, posicion, gameObject.transform.rotation);
            Debug.Log("Insantiated power up life");
        }
}
