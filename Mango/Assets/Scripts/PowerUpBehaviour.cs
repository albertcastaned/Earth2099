using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpBehaviour : MonoBehaviour
{
    
    [SerializeField] private PowerUp powerUp;
	public void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player")
        {
			Debug.Log("1.- Se activa el Power Up");
            ActivatePowerUp(other.gameObject.GetComponent<PowerUpController>());
            gameObject.SetActive(false);
        }
	}

    private void ActivatePowerUp(PowerUpController controller)
    {
        controller.ActivatePowerUp(powerUp);
    }

    public void SetPowerUp(PowerUp powerUp)
    {
        this.powerUp = powerUp;
        gameObject.name = powerUp.name;
        
    }
    
}
