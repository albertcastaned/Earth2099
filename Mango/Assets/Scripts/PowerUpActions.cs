using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpActions : MonoBehaviour
{
    [SerializeField] 
    public GameObject playerPrefab;
    private Player player;
    void Start()
    {
        player = (Player)playerPrefab.GetComponentsInChildren(typeof(Player))[0];
    }
    public void HighSpeedStartAction()
    {
        player.speed *= 2;
    }

    public void HighSpeedEndAction()
    {
        player.speed /= 2;
    }
}
