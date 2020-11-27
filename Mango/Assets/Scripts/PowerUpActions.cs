using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
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
        player.photonView.RPC("IncreaseHealth", RpcTarget.All, 50);
    }
    
    public void IncreaseBulletsGun(Player player)
    {
        player.aumentarBalas();
    }
}
