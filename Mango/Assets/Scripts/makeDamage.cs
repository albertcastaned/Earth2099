using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class makeDamage : MonoBehaviour
{
    public int cantidad = 10;

    private void OnCollisionExit(Collision other)
    {
       // Debug.Log(other.gameObject.tag);
        if(other.gameObject.tag == "Enemy")
            other.gameObject.GetComponent<Health_enemy>().RestLife(cantidad);
        Destroy(gameObject);
    }
    
}
