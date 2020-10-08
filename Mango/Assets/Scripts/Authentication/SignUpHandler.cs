using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class SignUpHandler : MonoBehaviour
{
    public const string MatchEmailPattern =
        @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
        + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
        + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
        + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

    public TMP_InputField emailTextBox;
    public TMP_InputField passwordTextBox;
    public TMP_InputField confirmPasswordTextBox;
    public Button signupButton;
    public TMP_Text emailErrorText;
    public TMP_Text passwordErrorText;
    protected FirebaseAuth auth;
    protected string displayName = "";
    private FirebaseUser newUser;

    // Start is called before the first frame update
    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        signupButton.onClick.AddListener(() => Submit());
    }

    void Submit()
    {
        emailErrorText.enabled = false;
        passwordErrorText.enabled = false;
        bool valid = true;

        if (string.IsNullOrEmpty(emailTextBox.text))
        {
            emailErrorText.text = "Este campo no puede estar vacio.";
            emailErrorText.enabled = true;
            valid = false;
        }

        if(!validateEmail(emailTextBox.text))
        {
            emailErrorText.text = "Este campo debe tener un correo valido.";
            emailErrorText.enabled = true;
            valid = false;
        }

        if (string.IsNullOrEmpty(passwordTextBox.text) || string.IsNullOrEmpty(confirmPasswordTextBox.text))
        {
            passwordErrorText.text = "Este campo no puede estar vacio.";
            passwordErrorText.enabled = true;
            valid = false;

        }
        else if (passwordTextBox.text != confirmPasswordTextBox.text)
        {
            passwordErrorText.text = "La contraseña no coincide";
            passwordErrorText.enabled = true;
            valid = false;

        }

        if (valid)
        {
            CreateNewUserAuth();
        }
        
    }

    
    private Task CreateNewUserAuth()
    {
        string email = emailTextBox.text;
        string password = passwordTextBox.text;
        Debug.Log("Creando usuario...");

        return auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("Se cancelo la creacion del usuario.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("Error al crear al usuario: " + task.Exception);
                return;
            }

            newUser = task.Result;

            // Create User Data Profile
            DatabaseReference database = FirebaseDatabase.DefaultInstance.RootReference;


            User user = new User(newUser.Email);

            database.Child("users").Child(newUser.UserId).SetRawJsonValueAsync(JsonUtility.ToJson(user)).ContinueWith(databaseTask =>
            {
                if (databaseTask.IsCanceled)
                {
                    Debug.LogError("Se cancelo la creacion del usuario.");
                    return;
                }
                if (databaseTask.IsFaulted)
                {
                    Debug.LogError("Error al crear al usuario: " + databaseTask.Exception);
                    return;
                }

            });

            ////////////////////////////
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
            newUser.DisplayName, newUser.UserId);
        });
    }





    private static bool validateEmail(string email)
    {
        if (email != null)
            return Regex.IsMatch(email, MatchEmailPattern);
        else
            return false;
    }
}
