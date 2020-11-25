using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpController : MonoBehaviour
{
    public GameObject prefabPowerUp;

    public GameObject powerupHudPrefab;
    public GameObject hudPowerUpDisplayParent;

    public Dictionary<string, float> activatePowerUps = new Dictionary<string, float>();

    private Dictionary<PowerUp, PowerupHudDisplay> activePowerUpsDisplayHud = new Dictionary<PowerUp, PowerupHudDisplay>();
    
    private List<PowerUp> keys = new List<PowerUp>();

    private Player player;


    class PowerupHudDisplay
    {
        public GameObject gameObject;
        public Text displayName;
        public Text countdown;

        public PowerupHudDisplay() { }
    }

    // Update is called once per frame
    void Update()
    {
        player = GetComponent<Player>();
        HandleActivePowerUps();
    }

    public void HandleActivePowerUps()
    {
        if (activatePowerUps.Count > 0)
        {
            List<PowerUp> tempPowerupList = new List<PowerUp>();
            foreach (PowerUp powerup in keys)
            {
                if (activatePowerUps[powerup.name] > 0)
                {
                    activatePowerUps[powerup.name] -= Time.deltaTime;
                    activePowerUpsDisplayHud[powerup].countdown.text = Mathf.FloorToInt(activatePowerUps[powerup.name]).ToString();
                }
                else
                {
					
                    activatePowerUps.Remove(powerup.name);
                    activePowerUpsDisplayHud[powerup].gameObject.Destroy();
                    activePowerUpsDisplayHud.Remove(powerup);
                    powerup.End(player);

                    tempPowerupList.Add(powerup);
                    
                }
            }

            keys.RemoveAll(item => tempPowerupList.Contains(item));
        }
    }

    public void ActivatePowerUp(PowerUp powerup)
    {
        if (!IsPowerupActive(powerup))
        {
			
            powerup.Start(player);

            if (powerup.duration > 0)
            {
                // Update Hud
                PowerupHudDisplay newDisplay = new PowerupHudDisplay
                {
                    gameObject = Instantiate(powerupHudPrefab, hudPowerUpDisplayParent.transform)
                };
                newDisplay.displayName = newDisplay.gameObject.transform.Find("PowerLabel").GetComponent<Text>();
                newDisplay.countdown = newDisplay.gameObject.transform.Find("Effect/TimerLabel").GetComponent<Text>();
                newDisplay.displayName.text = powerup.name;
                activePowerUpsDisplayHud.Add(powerup, newDisplay);

                activatePowerUps.Add(powerup.name, powerup.duration);
                keys.Add(powerup);
            }
        }
        else
        {
            activatePowerUps[powerup.name] += powerup.duration;
        }
    }

    bool IsPowerupActive(PowerUp powerup)
    {
        foreach(string p in activatePowerUps.Keys)
        {
            if (p == powerup.name)
                return true;
        }
        return false;
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
