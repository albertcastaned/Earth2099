using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Proyecto26;

public class Firebase : MonoBehaviour
{
    private const string PROJECT_ID = "mango-a33ff";
    private static readonly string databaseUrl = $"https://{PROJECT_ID}.firebaseio.com/";
    private const string API_KEY = "AIzaSyCCr15EpoZRMcHaArnG9JP-j6ywh_3WfKw";
    private static readonly string baseAuthUrl = "https://identitytoolkit.googleapis.com/v1/accounts:";

    public static void CreateUserProfile(User user, string id)
    {

        RestClient.Put<User>($"{databaseUrl}users/{id}.json", user).Then(response =>
        {
            Debug.Log("El usuario se creo exitosamente en la base de datos");
        }).Catch(err =>
        {
            var error = err as RequestException;
            GameManager.Instance.CreateMessageDialog("Error", error.Response);
        });
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
