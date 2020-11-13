using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bullet : MonoBehaviour
{
    public float speed = 20;

    void Start()
    {
        Destroy(gameObject, 10.0f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * speed);
    }

    void OnCollisionEnter(Collision collision)
    {
        //TODO: Different damage per bullet
        if (collision.gameObject.tag == "Enemy")
        {

            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (enemy.photonView.IsMine)
            {
                collision.gameObject.GetComponent<Enemy>().photonView.RPC("ReduceHealth", RpcTarget.AllBufferedViaServer, 10);
            }
            Destroy(gameObject);

        }
        else if (collision.gameObject.tag != "Player")
        {
            Destroy(gameObject);
        }
    }
}
