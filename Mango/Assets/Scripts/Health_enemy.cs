using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health_enemy : MonoBehaviour
{
    public int vida = 100;

    public void RestLife(int amount)
    {
        vida -= amount;
        Debug.Log(vida);

        if (vida == 0)
        {
            Destroy(gameObject);
        }
    }
    
}
