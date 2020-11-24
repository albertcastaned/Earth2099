using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ApplicationManager : Singleton<ApplicationManager> {
	

	public void Quit () 
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}

	public void LoadingScene ()
	{
		SceneManager.LoadScene(1, LoadSceneMode.Single);
	}
}
