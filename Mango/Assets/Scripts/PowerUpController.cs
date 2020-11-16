using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpController : MonoBehaviour
{
    public GameObject prefabPowerUp;

    public List<PowerUp> powerups;
    
    public Dictionary<PowerUp, float> activatePowerUps = new Dictionary<PowerUp,float>();
    
    private List<PowerUp> keys = new List<PowerUp>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
                    powerup.End();
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
            powerup.Start();
            activatePowerUps.Add(powerup, powerup.duration);
        }
        else
        {
            activatePowerUps[powerup] += powerup.duration;
        }

        keys = new List<PowerUp>(activatePowerUps.Keys);
    }

    public GameObject SpawnPowerUp(PowerUp powerup, Vector3 posicion)
    {
        GameObject powerupGameObject = Instantiate(prefabPowerUp);
        var powerUpBehaviour = powerupGameObject.GetComponent<PowerUpBehaviour>();
        powerUpBehaviour.controller = this;
        powerUpBehaviour.SetPowerUp(powerup);
        powerupGameObject.transform.position = posicion;

        return powerupGameObject;

    }
}
