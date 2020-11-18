using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSync : MonoBehaviourPun
{

    // Lista de scripts que deben ser solo activas para el jugador local ( Script de movimiento de jugador, script de camara, etc )
    public MonoBehaviour[] localScripts;

    // Lista de objectos que deben ser solo activos para el jugador local ( Camara, sonido, etc )
    public GameObject[] localObjects;

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
}
