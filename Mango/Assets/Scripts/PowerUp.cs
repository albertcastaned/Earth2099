using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 
using UnityEngine.Events;


[System.Serializable]
public class PlayerModifier : UnityEvent<Player>
{ }

[Serializable]
public class PowerUp 
{
	[SerializeField]
	public string name;

	[SerializeField]
	public float duration;

	[SerializeField]
	public PlayerModifier startAction;

	[SerializeField]
	public PlayerModifier endAction;

	public void End(Player player){
		Debug.Log("End called");
		if (endAction != null)
			endAction.Invoke(player);
	}

	public void Start(Player player)
	{
		if(startAction != null){
			startAction.Invoke(player);
		}
	}
	
}
