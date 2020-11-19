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
		Debug.Log("Aumenta la velocidad");
        player.speed *= 4;
		Debug.Log("La velocidad es " + player.speed);
    }

    public void HighSpeedEndAction()
    {
		Debug.Log("Disminuye la velocidad");
        player.speed /= 4;
    }
}
