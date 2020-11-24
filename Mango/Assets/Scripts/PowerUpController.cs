using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    public GameObject prefabPowerUp;
   
    public Dictionary<PowerUp, float> activatePowerUps = new Dictionary<PowerUp,float>();
    
    private List<PowerUp> keys = new List<PowerUp>();

    private Player player;

    // Update is called once per frame
    void Update()
    {
        player = GetComponent<Player>();
        HandleActivePowerUps();
    }

    public void HandleActivePowerUps()
    {
        bool changed = false;

        if (activatePowerUps.Count > 0)
        {
            foreach (PowerUp powerup in keys)
            {
                if (activatePowerUps[powerup] > 0)
                {
                    activatePowerUps[powerup] -= Time.deltaTime;
                }
                else
                {
					
                    changed = true;
                    activatePowerUps.Remove(powerup);
                    powerup.End(player);
                }
            }
        }

        if (changed)
        {
            keys = new List<PowerUp>(activatePowerUps.Keys);
                
        }
    }

    public void ActivatePowerUp(PowerUp powerup)
    {
        if (!activatePowerUps.ContainsKey(powerup))
        {
			
			Debug.Log("3.- Power Up start");
            powerup.Start(player);
            activatePowerUps.Add(powerup, powerup.duration);
        }
        else
        {
        	Debug.Log("4.- Power Up duration");
            activatePowerUps[powerup] += powerup.duration;
        }

        keys = new List<PowerUp>(activatePowerUps.Keys);
    }

    public GameObject SpawnPowerUp(PowerUp powerup, Vector3 posicion)
    {
        GameObject powerupGameObject = Instantiate(prefabPowerUp);
        var powerUpBehaviour = powerupGameObject.GetComponent<PowerUpBehaviour>();
        powerUpBehaviour.SetPowerUp(powerup);
        powerupGameObject.transform.position = posicion;

        return powerupGameObject;

    }
}
