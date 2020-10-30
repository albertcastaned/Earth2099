using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health_Player : MonoBehaviour
{
    public float vida = 100;
    public int vidaMax = 100;

    public Image barraVida;

    public void RestLife(int amount)
    {
        vida -= amount;
        RevisaVida();
        Debug.Log(vida);

        if (vida == 0)
        {
            Destroy(gameObject);
        }
    }

    public void RevisaVida()
    {
        barraVida.fillAmount = vida / vidaMax;
    }
}
