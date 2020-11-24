using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
public class PartyHealth : MonoBehaviourPunCallbacks
{

    public Transform layoutContentParent;
    public GameObject healthBarPrefab;
    private Dictionary<Photon.Realtime.Player, GameObject> playerHealthMap = new Dictionary<Photon.Realtime.Player, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerListOthers)
        {
            AddPlayerHealthbar(player);
        }
    }


    void AddPlayerHealthbar(Photon.Realtime.Player player)
    {

        GameObject newHealthbar = Instantiate(healthBarPrefab, layoutContentParent.transform);
        newHealthbar.name = player.ActorNumber.ToString();

        TextMeshProUGUI playerName = newHealthbar.transform.Find("Name").GetComponent<TextMeshProUGUI>();
        playerName.text = player.NickName;

        TextMeshProUGUI health = newHealthbar.transform.Find("Bar/Fillrect/Percentage").GetComponent<TextMeshProUGUI>();
        health.text = "Loading...";

        health.name = player.ActorNumber.ToString();
        playerHealthMap.Add(player, newHealthbar);
    }

    public void UpdateHealth(Photon.Realtime.Player player, int health, int maxHealth)
    {
        GameObject[] partyHealth = GameObject.FindGameObjectsWithTag("PartyHealthText");

        for (int i = 0; i < partyHealth.Length; i++)
        {
            if(partyHealth[i].name == player.ActorNumber.ToString())
            {
                partyHealth[i].transform.parent.gameObject.GetComponent<Image>().fillAmount = (float)health / maxHealth;
                partyHealth[i].GetComponent<TextMeshProUGUI>().text = health + " / " + maxHealth;
                return;
            }
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        AddPlayerHealthbar(newPlayer);
        base.OnPlayerEnteredRoom(newPlayer);
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {

        playerHealthMap[otherPlayer].Destroy();
        base.OnPlayerLeftRoom(otherPlayer);
    }

}
