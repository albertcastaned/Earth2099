using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpActions : MonoBehaviour
{
    [SerializeField] 
    private GameObject player;

    private Player scriptPLayer;
    
    public void HighSpeedStartAction()
    {
        scriptPLayer = player.GetComponent<Player>();
        scriptPLayer.speed = scriptPLayer.speed * 2;
    }

    public void HighSpeedEndAction()
    {
        scriptPLayer = player.GetComponent<Player>();
        scriptPLayer.speed = scriptPLayer.speed / 2;
    }
}
