using System;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SignUpHandler : MonoBehaviour
{
    public TMP_InputField emailTextBox;
    public TMP_InputField passwordTextBox;
    public TMP_InputField confirmPasswordTextBox;
    public Button signupButton;
    public TMP_Text emailErrorText;
    public TMP_Text passwordErrorText;
    protected FirebaseAuth auth;
    protected string displayName = "";
    
    // Start is called before the first frame update
    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        signupButton.onClick.AddListener(() => Submit());
    }

    void Submit()
    {
        passwordErrorText.enabled = false;
        if (passwordTextBox.text != confirmPasswordTextBox.text)
        {
            passwordErrorText.text = "La contraseña no coincide";
            passwordErrorText.enabled = true;
        }
        else
        {
            CreateNewUser();
        }
    }
    public Task CreateNewUser()
    {
        string email = emailTextBox.text;
        string password = passwordTextBox.text;
        Debug.Log("Creando usuario...");

        return auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            Firebase.Auth.FirebaseUser newUser = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                newUser.DisplayName, newUser.UserId);
        });
    }
}
