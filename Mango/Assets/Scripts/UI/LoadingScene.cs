using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LoadingScene : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(cargarNivel());
    }

    private IEnumerator cargarNivel()
    {
        yield return new WaitForSeconds(1.0f);
        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }
}
