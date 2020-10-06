using Firebase;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AuthSetup : MonoBehaviour
{

    public AuthManager authManager;

    private DependencyStatus dependencyStatus;

    // Start is called before the first frame update
    void Start()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                authManager.InitializeFirebase();
            }
            else
            {
                Debug.Log("No se pudo resolver todas las dependencias de Firebase: " + dependencyStatus);
            }
        });
    }

}
