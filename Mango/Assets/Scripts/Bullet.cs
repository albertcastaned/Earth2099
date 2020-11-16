using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bullet : MonoBehaviour
{
    public float damage = 10;

    void Start()
    {
        Destroy(gameObject, 5.0f);
    }

    void OnCollisionEnter(Collision collision)
    {
        //TODO: Different damage per bullet
        if (collision.gameObject.tag == "Enemy")
        {

            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if(enemy.photonView.IsMine)
                collision.gameObject.GetComponent<Enemy>().photonView.RPC("ReduceHealth", RpcTarget.AllBufferedViaServer, 10);
            
            Destroy(gameObject);

        }
        else if (collision.gameObject.tag != "Player" || collision.gameObject.tag != "Bullet")
        {
            Debug.Log(collision.gameObject.tag);
            Destroy(gameObject);
        }
    }
}
