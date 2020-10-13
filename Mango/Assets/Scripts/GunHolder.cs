using System;
using UnityEngine;
using UnityEngine.Serialization;

public class GunHolder : MonoBehaviour
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
        int prevIndex = selectedGunIndex;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedGunIndex = 0;
        } else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedGunIndex = 1;
        }

        if (prevIndex != selectedGunIndex)
        {
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
}
