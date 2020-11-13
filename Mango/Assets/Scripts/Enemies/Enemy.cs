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
    [Header("Animator")]
    public Animator animator;

    [Header("Stats")]
    public int maxHealth = 100;
    public float speed = 2f;
    public int damage = 10;
    public float sightRange = 10f;
    public float attackRange = 5f;
    public float walkPointRange = 10f;
    public float timeBetweenAttacks = 5f;

    [Header("HUD")]
    public Image barraVida;
    public TextMeshProUGUI lifeText;
    public DamagePopupText popupTextPrefab;

    private LayerMask isGround, isPlayer;

    private int health;
    private NavMeshAgent agent;
    private Player target;
    private Vector3 walkPoint;
    private bool playerInSightRange, playerInAttackRange, walkPointSet, alreadyAttacked;

    private Player[] playersInSightRange;
    private Player[] playersInAttackRange;

    void Start()
    {
        isGround = LayerMask.GetMask("Floor");
        isPlayer = LayerMask.GetMask("Player");
        agent = GetComponent<NavMeshAgent>();
        health = maxHealth;
        agent.speed = speed;
        lifeText.text = health.ToString() + " / " + maxHealth.ToString();
        UpdateAnimation("IsWalking", true);
    }

    void UpdateAnimation(string parameter, bool value)
    {
        animator.SetBool(parameter, value);
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
        UpdateAnimation("IsAttacking", false);
        UpdateAnimation("IsChasing", false);
        UpdateAnimation("IsWalking", true);

        // Remove target when out of sight
        if (target != null)
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
        UpdateAnimation("IsAttacking", false);
        UpdateAnimation("IsChasing", true);
        UpdateAnimation("IsWalking", false);
        if (target == null)
            ChooseRandomTargetInList(ref playersInSightRange);
        agent.SetDestination(target.transform.position);
    }

    private void AttackPlayer()
    {
        UpdateAnimation("IsAttacking", true);
        UpdateAnimation("IsChasing", false);
        UpdateAnimation("IsWalking", false);

        agent.SetDestination(transform.position);

        if(target == null)
        {
            ChooseRandomTargetInList(ref playersInAttackRange);
        }

        transform.LookAt(target.transform.position);

        if(!alreadyAttacked)
        {
            UpdateAnimation("IsAttacking", true);
            target.photonView.RPC("ReduceHealth", RpcTarget.All, damage);
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

    delegate void OnAnimationFinished();



    IEnumerator WaitForAnimation(OnAnimationFinished onAnimationFinished)
    {
        yield return new WaitForEndOfFrame();

        AnimatorStateInfo animatorStateInfo =  animator.GetCurrentAnimatorStateInfo(0);
        while(animatorStateInfo.normalizedTime < 1.0f)
        {
            animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return null;

        }
        onAnimationFinished();
        Debug.Log("Finished hurt animation");

    }

    [PunRPC]
    public void ReduceHealth(int amount)
    {
        if(health - amount < 0)
        {
            return;
        }
        health -= amount;
        UpdateHealthUI();
        CreateFloatingText("-" + amount);
        OnAnimationFinished onAnimationFinished;

        agent.isStopped = true;
        animator.Play("GetHit", 0, 0);
        UpdateAnimation("GotHit", true);
        onAnimationFinished = delegate ()
        {
            UpdateAnimation("GotHit", false);
            agent.isStopped = false;
        };
        StopCoroutine(nameof(WaitForAnimation));
        StartCoroutine(nameof(WaitForAnimation), onAnimationFinished);
        if (health <= 0)
        {
            onAnimationFinished = delegate ()
            {
                UpdateAnimation("IsDead", true);
                agent.isStopped = false;
                PhotonNetwork.Destroy(gameObject);
            };
            agent.isStopped = true;
            animator.Play("Die", 0, 0);
            StopCoroutine(nameof(WaitForAnimation));
            StartCoroutine(nameof(WaitForAnimation), onAnimationFinished);
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
