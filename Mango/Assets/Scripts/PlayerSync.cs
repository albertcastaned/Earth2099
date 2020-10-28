using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSync : MonoBehaviourPun, IPunObservable
{

    // Lista de scripts que deben ser solo activas para el jugador local ( Script de movimiento de jugador, script de camara, etc )
    public MonoBehaviour[] localScripts;

    // Lista de objectos que deben ser solo activos para el jugador local ( Camara, sonido, etc )
    public GameObject[] localObjects;
    public GameObject gunHolder;

    // Valores que deben sincronizados
    Vector3 latestPos;
    Quaternion latestRot;
    private int latestSelectedGun;

    // Start is called before the first frame update
    void Start()
    {
        // Desactivar objetos y scripts de jugadores remotos
        if (!photonView.IsMine)
        {
            for (int i = 0; i < localScripts.Length; i++)
            {
                localScripts[i].enabled = false;
            }
            for (int i = 0; i < localObjects.Length; i++)
            {
                localObjects[i].SetActive(false);
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Enviar nuestros datos a otros jugadores
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(gunHolder.GetComponent<GunHolder>().selectedGunIndex);
        }
        else
        {
            // Recibir datos de otros jugadores
            latestPos = (Vector3)stream.ReceiveNext();
            latestRot = (Quaternion)stream.ReceiveNext();
            latestSelectedGun = (int)stream.ReceiveNext();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, latestPos, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, latestRot, Time.deltaTime * 5);
            gunHolder.GetComponent<GunHolder>().SelectGun(latestSelectedGun);
        }
    }
}
