using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ApplicationManager : Singleton<ApplicationManager> {
	

	public void Quit () 
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#endif
		Application.Quit();

	}

	public void LoadingScene ()
	{
		SceneManager.LoadScene(1, LoadSceneMode.Single);
	}
}
