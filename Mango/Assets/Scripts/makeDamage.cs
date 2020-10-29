using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class makeDamage : MonoBehaviour
{
    public int cantidad = 10;

    private void OnCollisionEnter(Collision other)
    {
       Debug.Log("Bala " + gameObject.name + " dispara a " + other.gameObject.name);
       if(other.gameObject.tag == "Enemy")
            other.gameObject.GetComponent<Health_enemy>().RestLife(cantidad);
        Destroy(gameObject);
    }
    
}
