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
			Renderer ren = gameObject.GetComponent<Renderer>();
			ren.enabled = false;
            ActivatePowerUp(other.gameObject.GetComponent<PowerUpController>());
            
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
