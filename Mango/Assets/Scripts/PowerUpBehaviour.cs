using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpBehaviour : MonoBehaviour
{
    
    public PowerUpController controller;
    [SerializeField] private PowerUp powerUp;

    private Transform transform_;

    private void Awaker()
    {
        transform_ = transform;
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "player")
        {
            ActivatePowerUp();
            gameObject.SetActive(false);
        }
    }

    private void ActivatePowerUp()
    {
        controller.ActivatePowerUp(powerUp);
    }

    public void SetPowerUp(PowerUp powerUp)
    {
        this.powerUp = powerUp;
        gameObject.name = powerUp.name;
        
    }
    
}
