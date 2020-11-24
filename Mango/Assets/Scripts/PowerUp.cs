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
	public Player playerF;
	[SerializeField]
	public string name;

	[SerializeField]
	public float duration;

	[SerializeField]
	public PlayerModifier startAction;

	[SerializeField]
	public PlayerModifier endAction;
   
	public void End(Player player){
		if(endAction != null)
			endAction.Invoke(player);
	}

	public void Start(Player player)
	{
		Debug.Log("5.- Power Up start Invoke");
		if(startAction != null){
			playerF = player;
			startAction.Invoke(player);
		}
	}
	
}
