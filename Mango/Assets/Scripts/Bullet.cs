using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bullet : MonoBehaviourPun
{
    public float damage = 10;
    public Transform collisionEffect;
    public Transform collisionEnemyEffect;

    void Start()
    {
        Destroy(gameObject, 5.0f);
    }

    void OnCollisionEnter(Collision collision)
    {
        //TODO: Different damage per bullet
        if (collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Enemy>().photonView.RPC("ReduceHealth", RpcTarget.AllBufferedViaServer, 10);
            PhotonNetwork.Instantiate(collisionEnemyEffect.name, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else if (!collision.gameObject.CompareTag("Player") || !collision.gameObject.CompareTag("Bullet"))
        {
            PhotonNetwork.Instantiate(collisionEffect.name, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
