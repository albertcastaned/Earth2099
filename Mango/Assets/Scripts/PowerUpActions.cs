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

	public void IncreaseLifeStartAction(Player player)
    {
		Debug.Log("Incrementará la vida....");
		if(player.health < 90)
        	player.health += 10;
		else 
			player.health = 100;
		player.UpdateHealthUI();
    }

	public void IncreaseLifeEndAction(Player player)
    {
		Debug.Log("Ya pasarón los 20 segundos");
    }
}
