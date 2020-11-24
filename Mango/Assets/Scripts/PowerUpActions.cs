using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpActions : MonoBehaviour
{
    public void HighSpeedStartAction(Player player)
    {
		Debug.Log("Aumenta la velocidad");
        player.speed *= 2;
		Debug.Log("La velocidad es " + player.speed);
    }

    public void HighSpeedEndAction(Player player)
    {
		Debug.Log("Disminuye la velocidad");
        player.speed /= 2;
    }
}
