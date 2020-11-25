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
            ActivatePowerUp(other.gameObject.GetComponent<PowerUpController>());
        }
	}

    private void ActivatePowerUp(PowerUpController controller)
    {
        controller.ActivatePowerUp(powerUp);
        photonView.RPC(nameof(DeactivatePowerup), RpcTarget.All);
    }

    [PunRPC]
    void DeactivatePowerup()
    {
        gameObject.SetActive(false);
        Destroy(gameObject, powerUp.duration + 1f);
    }

    public void SetPowerUp(PowerUp powerUp)
    {
        this.powerUp = powerUp;
        gameObject.name = powerUp.name;
    }
    
}
