using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 
using UnityEngine.Events;

[Serializable]
public class PowerUp : MonoBehaviour
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
		startAction.Invoke();
	}
}
