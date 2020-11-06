using Mango.Game;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviourPun
{
    public int health = 100;
    public int maxHealth = 100;

    [Header("HUD")]
    public Image barraVida;
    public TextMeshProUGUI lifeText;
    public DamagePopupText popupTextPrefab;

    public float walkPointRange;
    public float timeBetweenAttacks;
    public float sightRange, attackRange;
    public LayerMask isGround, isPlayer;

    private NavMeshAgent agent;
    private Player target;
    private Vector3 walkPoint;
    private bool walkPointSet;
    private bool alreadyAttacked;
    private bool playerInSightRange, playerInAttackRange;

    public Player[] playersInSightRange;
    public Player[] playersInAttackRange;



    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lifeText.text = health.ToString() + " / " + maxHealth.ToString();
    }


    // Update is called once per frame
    void Update()
    {
        // Might be performance heavy. Need testing to verify

        if (!PhotonNetwork.IsMasterClient)
            return;

        // Check sight and attack range
        playerInSightRange = CheckPlayersInRange(sightRange, ref playersInSightRange);
        playerInAttackRange = CheckPlayersInRange(attackRange, ref playersInAttackRange);

        if (!playerInAttackRange && !playerInSightRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
    }

    private bool CheckPlayersInRange(float range, ref Player[] playerList)
    {
        // Counting not alive players
        Collider[] totalPlayersColliders = 
            Physics.OverlapSphere(transform.position, range, isPlayer).Where(player => player.gameObject.GetComponent<Player>().IsAlive).ToArray();

        List<Player> auxPlayers = new List<Player>();
        foreach (Collider collider in totalPlayersColliders)
            auxPlayers.Add(collider.gameObject.GetComponent<Player>());

        playerList = auxPlayers.ToArray();
        return playerList.Length > 0;
    }


    private void Patroling()
    {

        // Remove target when out of sight
        if(target != null)
        {
            target = null;
        }

        
        if (!walkPointSet) SearchWalkPoint();
        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 5f)
            walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if(Physics.Raycast(walkPoint, -transform.up, 30f, isGround))
        {
            walkPointSet = true;
        }
    }

    private void ChasePlayer()
    {
        if(target == null)
            ChooseRandomTargetInList(ref playersInSightRange);
        agent.SetDestination(target.transform.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        if(target == null)
        {
            ChooseRandomTargetInList(ref playersInAttackRange);
        }

        transform.LookAt(target.transform.position);

        if(!alreadyAttacked)
        {
            //TODO: Variable enemy damage
            target.photonView.RPC("ReduceHealth", RpcTarget.All, 2);
            ChooseRandomTargetInList(ref playersInAttackRange);
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ChooseRandomTargetInList(ref Player[] playerRangeList)
    {
        target = playerRangeList[Random.Range(0, playerRangeList.Length - 1)];
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, walkPointRange);
        Gizmos.DrawLine(walkPoint, walkPoint + Vector3.up * 100f);
        if(target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(target.transform.position, target.transform.position + Vector3.up * 100f);
        }

    }

    [PunRPC]
    public void ReduceHealth(int amount)
    {
        health -= amount;
        UpdateHealthUI();
        CreateFloatingText("-" + amount);
        if (health <= 0 && photonView.IsMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void UpdateHealthUI()
    {
        barraVida.fillAmount = (float)health / maxHealth;
        lifeText.text = health.ToString() + " / " + maxHealth.ToString();
    }



    private void CreateFloatingText(string text)
    {
        DamagePopupText instance = Instantiate(popupTextPrefab, transform);

        instance.SetText(text);
    }
}
