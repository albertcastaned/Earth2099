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
		Debug.Log("Hubo una colisión con " + other.gameObject.tag);
        
    }

	public void OnTriggerEnter(Collider other){
		Debug.Log("Hubo un trigger con " + other.tag);
		if (other.gameObject.tag == "Player")
        {
			Debug.Log("1.- Se activa el Power Up");
            ActivatePowerUp();
            gameObject.SetActive(false);
        }
	}
    private void ActivatePowerUp()
    {
		Debug.Log("2.- Mandar llamar al controler activar el PowerUp");
        controller.ActivatePowerUp(powerUp);
    }

    public void SetPowerUp(PowerUp powerUp)
    {
        this.powerUp = powerUp;
        gameObject.name = powerUp.name;
        
    }
    
}
