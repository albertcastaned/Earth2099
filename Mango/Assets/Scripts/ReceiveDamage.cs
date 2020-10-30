using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReceiveDamage : MonoBehaviour
{
    public int cantidad = 10;

    private void OnCollisionEnter(Collision other)
    { 
        Debug.Log("Colisiona con " + other.gameObject.tag);
        if(other.gameObject.tag == "Player")
            other.gameObject.GetComponent<Health_Player>().RestLife(cantidad);
        //Destroy(gameObject);
    }
}
