using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 
using UnityEngine.Events;

[Serializable]
public class PowerUp 
{
	[SerializeField]
	public string name;

	[SerializeField]
	public float duration;

	[SerializeField]
	public UnityEvent startAction;

	[SerializeField]
	public UnityEvent endAction;
   
	public void End(){
		if(endAction != null)
			endAction.Invoke();
	}

	public void Start()
	{
		Debug.Log("5.- Power Up start Invoke");
		if(startAction != null)
			startAction.Invoke();
	}
}
