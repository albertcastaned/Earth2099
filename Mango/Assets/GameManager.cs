using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private static GameManager instance;

    public static GameManager Instance {  get { return instance;  } }

    [Header("UI")]
    public GameObject loadingDialog;
    public GameObject messageDialog;

    void Awake()
    {
        //Singleton
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }
   
    public GameObject CreateLoadingDialog()
    {
        GameObject dialog = Instantiate(loadingDialog);

        return dialog;
    }
}
