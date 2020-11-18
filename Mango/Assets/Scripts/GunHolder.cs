using System;
using Photon.Pun;
using UnityEngine;

public class GunHolder : MonoBehaviourPun
{
    public int selectedGunIndex = 0;
    [NonSerialized]
    public GameObject SelectedGun;

    // Start is called before the first frame update
    void Start()
    {
        SelectGun();
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;

        int prevIndex = selectedGunIndex;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Change event 1");
            selectedGunIndex = 0;
        } else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Change event 2");
            selectedGunIndex = 1;
        }

        if (prevIndex != selectedGunIndex)
        {
            Debug.Log("Call SelectGun()");
            SelectGun();
        }
    }

    private void SelectGun()
    {
        var i = 0;
        // We can access all the child (the two guns) in this way
        foreach (Transform gun in transform)
        {
            if (selectedGunIndex == i)
            {
                SelectedGun = gun.gameObject;
                gun.gameObject.SetActive(true);
            }
            else
            {
                gun.gameObject.SetActive(false);
            }
            i++;
        }
    }
    
    public void SelectGun(int index)
    {
        var i = 0;
        selectedGunIndex = index;
        // We can access all the child (the two guns) in this way
        foreach (Transform gun in transform)
        {
            if (selectedGunIndex == i)
            {
                SelectedGun = gun.gameObject;
                gun.gameObject.SetActive(true);
            }
            else
            {
                gun.gameObject.SetActive(false);
            }
            i++;
        }
    }
}
