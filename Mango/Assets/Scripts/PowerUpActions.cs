using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpActions : MonoBehaviour
{
    [SerializeField] 
    private Player player;

    public void HighSpeedStartAction()
    {
        player.speed = player.speed * 2;
    }

    public void HighSpeedEndAction()
    {
        player.speed = player.speed / 2;
    }
}
