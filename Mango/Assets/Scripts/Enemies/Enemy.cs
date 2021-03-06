﻿using Mango.Game;
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
    protected Animator animator;

    [Header("Stats")]
    public int maxHealth = 100;
    public float speed = 2f;
    public int damage = 10;
    public float sightRange = 10f;
    public float attackRange = 5f;
    public float walkPointRange = 10f;
    public float timeBetweenAttacks = 5f;
    public bool stunnedByHits = true;
    public int points;

    [Header("HUD")]
    public Image barraVida;
    public TextMeshProUGUI lifeText;

    [Header("Prefabs")]
    public DamagePopupText popupTextPrefab;
    public Transform deathAnimation;

    protected LayerMask isGround, isPlayer;

    protected int health;
    protected NavMeshAgent agent;
    protected Player target;
    protected Vector3 walkPoint;
    protected bool playerInSightRange, playerInAttackRange, walkPointSet, alreadyAttacked;

    protected Player[] playersInSightRange;
    protected Player[] playersInAttackRange;

    protected EnemyState currentState;
    protected delegate void OnAnimationFinished();

    protected List<Player> auxPlayers;

    protected AudioManager audioManager;
    protected Vector3 originalDestPosition;

    protected enum EnemyState
    {
        AttackIdle,
        Patrolling,
        Chasing,
        Attacking,
        GettingHit,
        Dead
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        audioManager = GetComponent<AudioManager>();
        currentState = EnemyState.Patrolling;
        isGround = LayerMask.GetMask("Floor");
        isPlayer = LayerMask.GetMask("Player");
        agent = GetComponent<NavMeshAgent>();
        health = maxHealth;
        agent.speed = speed;
        lifeText.text = health.ToString() + " / " + maxHealth.ToString();
        UpdateAnimation("IsWalking", true);

        animator.SetFloat("MovementSpeed", 1.4f - 1f / speed);

        // Called only twice per second to improve performance
        InvokeRepeating(nameof(EnemyUpdate), 0.4f, 0.4f);
    }

    protected void UpdateAnimation(string parameter, bool value)
    {
        animator.SetBool(parameter, value);
    }


    protected void EnemyUpdate()
    {
        // Might be performance heavy. Need testing to verify
        if (!PhotonNetwork.IsMasterClient || currentState == EnemyState.Dead)
            return;

        // Check sight and attack range
        playerInSightRange = CheckPlayersInRange(sightRange, ref playersInSightRange);
        playerInAttackRange = CheckPlayersInRange(attackRange, ref playersInAttackRange);

        if (!playerInAttackRange && !playerInSightRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();


    }

    protected bool CheckPlayersInRange(float range, ref Player[] playerList)
    {
        // Counting not alive players
        Collider[] totalPlayersColliders = 
            Physics.OverlapSphere(transform.position, range, isPlayer).Where(player => player.gameObject.GetComponent<Player>().IsAlive).ToArray();

       auxPlayers = new List<Player>();
        foreach (Collider collider in totalPlayersColliders)
            auxPlayers.Add(collider.gameObject.GetComponent<Player>());

        playerList = auxPlayers.ToArray();
        return playerList.Length > 0;
    }


    protected void Patroling()
    {
        currentState = EnemyState.Patrolling;
        UpdateAnimation();

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
    protected virtual void ChasePlayer()
    {
        if (target == null)
            ChooseRandomTargetInList(ref playersInSightRange);
        Vector3 playerPos = target.transform.position;

        // Update if player moved
        if(Vector3.Distance(playerPos, originalDestPosition) > attackRange + 2f)
        {
            agent.SetDestination(playerPos);
            if (agent.pathStatus == NavMeshPathStatus.PathComplete)
            {
                currentState = EnemyState.Chasing;
                UpdateAnimation();
                originalDestPosition = playerPos;
            }
        }
    }

    protected virtual void AttackPlayer()
    {
        currentState = EnemyState.Attacking;
        UpdateAnimation();

        if (target == null)
        {
            ChooseRandomTargetInList(ref playersInAttackRange);
        }
      // agent.SetDestination(transform.position);

        transform.LookAt(target.transform.position);

        if (!alreadyAttacked)
        {
            UpdateAnimation("AttackReady", false);

            target.photonView.RPC("ReduceHealth", RpcTarget.All, damage);
            ChooseRandomTargetInList(ref playersInAttackRange);
            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);

        }
    }
    protected void ResetAttack()
    {
        alreadyAttacked = false;
        UpdateAnimation("AttackReady", true);
    }

    protected void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if(Physics.Raycast(walkPoint, -transform.up, 30f, isGround))
        {
            walkPointSet = true;
        }
    }


    protected void UpdateAnimation()
    {
        switch(currentState)
        {
            case EnemyState.Chasing:
                UpdateAnimation("IsAttacking", false);
                UpdateAnimation("IsChasing", true);
                UpdateAnimation("IsWalking", false);
                break;
            case EnemyState.Patrolling:
                UpdateAnimation("IsAttacking", false);
                UpdateAnimation("IsChasing", false);
                UpdateAnimation("IsWalking", true);
                break;
            case EnemyState.Attacking:
                UpdateAnimation("IsAttacking", true);
                UpdateAnimation("IsChasing", false);
                UpdateAnimation("IsWalking", false);
                break;
        }
    }


    protected void ChooseRandomTargetInList(ref Player[] playerRangeList)
    {
        target = playerRangeList[Random.Range(0, playerRangeList.Length - 1)];
    }



    protected IEnumerator WaitForAnimation(OnAnimationFinished onAnimationFinished)
    {

        yield return new WaitForEndOfFrame();

        AnimatorStateInfo animatorStateInfo =  animator.GetCurrentAnimatorStateInfo(0);
        while(animatorStateInfo.normalizedTime < 1.0f)
        {
            animatorStateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return null;

        }
        Debug.Log("Finished animation coroutine");
        onAnimationFinished();

    }

    [PunRPC]
    public void ReduceHealth(int amount)
    {
        if (currentState == EnemyState.Dead)
            return;
        health -= amount;
        if (health < 0)
        {
            health = 0;
        }
        UpdateHealthUI();
        audioManager.Play("Damage");
        CreateFloatingText("-" + amount);

        OnAnimationFinished onAnimationFinished;

        if (stunnedByHits)
        {
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
        }
        if (health <= 0)
        {
            CreateFloatingText("+" + points, false);

            currentState = EnemyState.Dead;
            RoomController.Instance.DecreaseCurrentEnemiesCount();
            Die();

        }

    }

    public virtual void Die()
    {
        IncreaseScore(points);
        OnAnimationFinished onAnimationFinished = delegate ()
        {
            UpdateAnimation("IsDead", true);
            agent.isStopped = false;
            GameObject deathAnimationObj = Instantiate(deathAnimation.gameObject);
            deathAnimationObj.transform.position = transform.position;

            if (photonView.IsMine)
                PhotonNetwork.Destroy(gameObject);
        };
        agent.isStopped = true;
        animator.Play("Die", 0, 0);
        StopCoroutine(nameof(WaitForAnimation));
        StartCoroutine(nameof(WaitForAnimation), onAnimationFinished);

    }
    public void IncreaseScore(int increase)
    {
        RoomController.Instance.IncreaseScore(increase);
    }

    private void UpdateHealthUI()
    {
        barraVida.fillAmount = (float)health / maxHealth;
        lifeText.text = health.ToString() + " / " + maxHealth.ToString();
    }

    protected void CreateFloatingText(string text, bool isDamage = true)
    {
        DamagePopupText instance = Instantiate(popupTextPrefab, transform);

        if(!isDamage)
        {
            instance.damageText.color = Color.yellow;
        }
        instance.SetText(text);
    }




    // DEBUG ///////////////////////////////////////////////////

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, walkPointRange);
        Gizmos.DrawLine(walkPoint, walkPoint + Vector3.up * 100f);
        if (target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(target.transform.position, target.transform.position + Vector3.up * 100f);
        }
        Gizmos.color = Color.white;
    }


    // ////////////////////////////////////////////////////////
}
