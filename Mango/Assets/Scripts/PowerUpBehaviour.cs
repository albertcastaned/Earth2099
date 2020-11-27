using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class PowerUpBehaviour : MonoBehaviourPun
{
    
    [SerializeField] private PowerUp powerUp;

    void Start()
    {
        SetPowerUp(powerUp);
    }

	public void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player")
        {
            Debug.Log("1. Mandar activar");
            ActivatePowerUp(other.gameObject.GetComponent<PowerUpController>());
        }
	}

    private void ActivatePowerUp(PowerUpController controller)
    {
        Debug.Log("Es hora de llamar al controller del player");
        controller.ActivatePowerUp(powerUp);
        photonView.RPC(nameof(DeactivatePowerup), RpcTarget.All);
    }

    [PunRPC]
    void DeactivatePowerup()
    {
        transform.parent.gameObject.SetActive(false);
        Destroy(transform.parent.gameObject, powerUp.duration + 1f);
    }

    public void SetPowerUp(PowerUp powerUp)
    {
        this.powerUp = powerUp;
        gameObject.name = powerUp.name;
    }
    
}
