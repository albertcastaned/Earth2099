using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Proyecto26;
using UnityEngine.SceneManagement;

public class Firebase : MonoBehaviour
{
    private const string PROJECT_ID = "mango-a33ff";
    private static readonly string databaseUrl = $"https://{PROJECT_ID}.firebaseio.com/";

    //TODO: Esconder esta llave
    private const string API_KEY = "AIzaSyCCr15EpoZRMcHaArnG9JP-j6ywh_3WfKw";
    private const string baseAuthUrl = "https://identitytoolkit.googleapis.com/v1/accounts:";
    private static string userID;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        userID = PlayerPrefs.GetString("User UID");
        if(IsAuthenticated())
        {
            Debug.Log("Se recupero la sesion del usuario: " + userID);
            SceneManager.LoadScene("Launcher");
        }
    }

    public static bool IsAuthenticated() {
        return !string.IsNullOrEmpty(userID);
    }

    public static void CreateUserProfile(User user, string id)
    {

        RestClient.Put<User>($"{databaseUrl}users/{id}.json", user).Then(response =>
        {
            Debug.Log("El usuario se creo exitosamente en la base de datos");
            GameManager.Instance.CreateMessageDialog("Exito", "Se registraron tus datos exitosamente. Ya puedes iniciar sesion.");
        }).Catch(err =>
        {
            var error = err as RequestException;
            GameManager.Instance.CreateMessageDialog("Error", error.Response);
        });
    }

    public static void SignOut()
    {
        PlayerPrefs.DeleteKey("User UID");
        Debug.Log("Se cerro la sesion del usuario");
        SceneManager.LoadScene("AuthMenu");
    }

    public static void CreateUser(User user, string password)
    {

        var payLoad = $"{{\"email\":\"{user.email}\",\"password\":\"{password}\",\"returnSecureToken\":true}}";

        RestClient.Post($"{baseAuthUrl}signUp?key={API_KEY}", payLoad).Then(response =>
        {
            Debug.Log("Se creo el usuario en Firebase Auth");
            Debug.Log(response.Text);

            FirebaseSignupResponse fbResponse = JsonUtility.FromJson<FirebaseSignupResponse>(response.Text);

            Debug.Log(fbResponse);

            CreateUserProfile(user, fbResponse.localId);
        }
        ).Catch(err =>
        {
            var error = err as RequestException;
            GameManager.Instance.CreateMessageDialog("Error", error.Response);
        });

    }
    public static void LoginUser(User user, string password)
    {

        var payLoad = $"{{\"email\":\"{user.email}\",\"password\":\"{password}\",\"returnSecureToken\":true}}";

        RestClient.Post($"{baseAuthUrl}signInWithPassword?key={API_KEY}", payLoad).Then(response =>
        {
            Debug.Log("Se inicio sesion en Firebase Auth");


            FirebaseSignupResponse fbResponse = JsonUtility.FromJson<FirebaseSignupResponse>(response.Text);
            PlayerPrefs.SetString("User UID", fbResponse.localId);
            Debug.Log("Se guardo la sesion del usuario en PlayerPrefs");
            SceneManager.LoadScene("Launcher");

        }
        ).Catch(err =>
        {
            var error = err as RequestException;
            GameManager.Instance.CreateMessageDialog("Error", error.Response);
        });
    }

    [Serializable]
    private class FirebaseSignupResponse
    {
        public string idToken = "";
        public string email = "";
        public string refreshToken = "";
        public string expiresIn = "";
        public string localId = "";


        public override string ToString()
        {
            return $"{ idToken} { email} { refreshToken} { expiresIn } { localId }";
        }
    }

}
