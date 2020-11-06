using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class AuthHandler : MonoBehaviour
{
    public const string MatchEmailPattern =
        @"^(([\w-]+\.)+[\w-]+|([a-zA-Z]{1}|[\w-]{2,}))@"
        + @"((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\."
        + @"([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|"
        + @"([a-zA-Z]+[\w-]+\.)+[a-zA-Z]{2,4})$";

    public TMP_Text title;
    public TMP_InputField emailTextBox;
    public TMP_InputField passwordTextBox;
    public TMP_InputField confirmPasswordTextBox;
    public TMP_Text emailErrorText;
    public TMP_Text passwordErrorText;

    public TMP_Text submitText;
    public TMP_Text changeAuthTypeLabelText;
    public TMP_Text changeAuthTypeButtonText;
    public bool isLogin = true;
    protected string displayName = "";
    private bool loading;

    void Start()
    {
        ChangeAuthType();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            Submit();
    }

    public void ChangeAuthType()
    {
        emailErrorText.text = "";
        passwordErrorText.text = "";
        isLogin = !isLogin;
        confirmPasswordTextBox.gameObject.SetActive(!isLogin);
        title.text = isLogin ? "Inicar Sesion" : "Registrar Usuario";
        submitText.text = isLogin ? "Ingresar" : "Registrar";
        changeAuthTypeButtonText.text = isLogin ? "Registrar Usuario" : "Iniciar Sesion";
        changeAuthTypeLabelText.text = isLogin ? "¿No tienes cuenta?" : "¿Ya tienes cuenta?";
    }
    public void Submit()
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

        if(!ValidateEmail(emailTextBox.text))
        {
            emailErrorText.text = "Este campo debe tener un correo valido.";
            emailErrorText.enabled = true;
            valid = false;
        }

        if (string.IsNullOrEmpty(passwordTextBox.text) || (!isLogin && string.IsNullOrEmpty(confirmPasswordTextBox.text)))
        {
            passwordErrorText.text = "Este campo no puede estar vacio.";
            passwordErrorText.enabled = true;
            valid = false;

        }
        else if (!isLogin && passwordTextBox.text != confirmPasswordTextBox.text)
        {
            passwordErrorText.text = "La contraseña no coincide";
            passwordErrorText.enabled = true;
            valid = false;
        }

        if (valid && !loading)
        {
            if (!isLogin)
                CreateNewUser();
            else
                LoginUser();
        }
    }

    private void LoginUser()
    {
        loading = true;
        var loadingObj = GameManager.Instance.CreateLoadingDialog();
        string email = emailTextBox.text;
        string password = passwordTextBox.text;
        Debug.Log("Iniciando sesion con autenticacion de Firebase...");
        User user = new User(email);
        Firebase.LoginUser(user, password);
        Destroy(loadingObj);
        loading = false;
    }

    public void LoginAnonymous()
    {
        loading = true;
        var loadingObj = GameManager.Instance.CreateLoadingDialog();
        Debug.Log("Iniciando sesion anonimamente...");
        Firebase.LoginAnonymous();
        Destroy(loadingObj);
        loading = false;

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

    private static bool ValidateEmail(string email)
    {
        return (email != null) && Regex.IsMatch(email, MatchEmailPattern);
    }
}
