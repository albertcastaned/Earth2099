﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpActions : MonoBehaviour
{
    public void HighSpeedStartAction(Player player)
    {
        player.speed *= 2;
    }

    public void HighSpeedEndAction(Player player)
    {
        Debug.Log("Speed reduces");
        player.speed /= 2;
    }

	public void IncreaseLifeStartAction(Player player)
    {
        player.health += 10;
        if (player.maxHealth < player.health)
            player.health = player.maxHealth;

		player.UpdateHealthUI();
    }
}
