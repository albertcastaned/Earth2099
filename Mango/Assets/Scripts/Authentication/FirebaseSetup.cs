using Firebase;
using Firebase.Unity.Editor;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Auth;
using Firebase.Database;
using System;

public class FirebaseSetup : MonoBehaviour
{

    private DependencyStatus dependencyStatus;
    FirebaseAuth auth;
    FirebaseUser user;

    private readonly System.Uri fbDatabaseUri = new System.Uri("https://mango-a33ff.firebaseio.com/");

    // Start is called before the first frame update
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.Log("No se pudo resolver todas las dependencias de Firebase: " + dependencyStatus);
            }
        });
    }

    public void InitializeFirebase()
    {
        Debug.Log("Cargando Firebase...");
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
        FirebaseApp.DefaultInstance.Options.DatabaseUrl = fbDatabaseUri;



        LoadReference();
    }

    public void LoadReference()
    {
        FirebaseDatabase reference = FirebaseDatabase.DefaultInstance;
        Debug.Log(reference.RootReference.ToString());
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && user != null)
            {
                Debug.Log("Se cerro sesion: " + user.UserId);
            }
            user = auth.CurrentUser;
            if (signedIn)
            {
                Debug.Log("Se inicio sesion: " + user.UserId);
            }
        }
    }

}
