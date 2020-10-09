using System;
using System.Threading.Tasks;
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
    protected string displayName = "";
    private bool loading;

    // Start is called before the first frame update
    void Start()
    {
        signupButton.onClick.AddListener(() => Submit());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            Submit();
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

        if (valid && !loading)
        {
            CreateNewUser();
        }

    }

    
    private void CreateNewUser()
    {
        loading = true;
        var loadingObj = GameManager.Instance.CreateLoadingDialog();
        string email = emailTextBox.text;
        string password = passwordTextBox.text;
        Debug.Log("Creando usuario en autenticacion de Firebase...");
        User user = new User(email);
        Firebase.CreateUser(user, password);
        Destroy(loadingObj);
        loading = false;
    }

    private static bool validateEmail(string email)
    {
        if (email != null)
            return Regex.IsMatch(email, MatchEmailPattern);
        else
            return false;
    }
}
