using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemyProjectile : MonoBehaviour
{
    public int damage = 20;
    public Transform collisionEffect;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 5f);
    }
    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            Instantiate(collisionEffect, transform.position, Quaternion.identity);

            collision.gameObject.GetComponent<Player>().photonView.RPC("ReduceHealth", RpcTarget.AllBufferedViaServer, (int)Random.Range(damage - 2, damage + 2));
            Destroy(gameObject);
        }
        else if (!collision.gameObject.CompareTag("Enemy") && !collision.gameObject.CompareTag("Bullet"))
        {
            Instantiate(collisionEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);

        }
        
    }
}
