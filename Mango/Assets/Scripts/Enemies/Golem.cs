using Mango.Game;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Golem : Enemy
{
    [Range(0, 100)]
    public float chanceToThrowRock;
    #region
    public float horizontalOffset = 5f;

    public float verticalOffset = 5f;
    public float enemyFireIntervalTime = 1f;
    public float projectileSpeed = 30f;
    public GameObject enemyProjectile;
    #endregion

    private bool throwing = false;

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

        if (photonView.IsMine)
            InvokeRepeating(nameof(ShootProjectile), enemyFireIntervalTime, enemyFireIntervalTime);
    }

    void ShootProjectile()
    {
        if (target == null || throwing)
            return;

        if (currentState == EnemyState.Chasing && Random.Range(0, 100) < chanceToThrowRock)
        {
            //Throw rock
            agent.isStopped = true;
            throwing = true;
            animator.Play("ThrowRock", 0, 0);
            //UpdateAnimation("IsThrowing", true);

            OnAnimationFinished onAnimationFinished = delegate ()
            {

                agent.isStopped = false;
                //UpdateAnimation("IsThrowing", false);
                throwing = false;
                photonView.RPC(nameof(EnemyGolemThrowRock), RpcTarget.All);
            };


            StopCoroutine(nameof(WaitForRockThrow));
            StartCoroutine(nameof(WaitForRockThrow), onAnimationFinished);
        }
    }

    IEnumerator WaitForRockThrow(OnAnimationFinished onAnimationFinished)
    {
        yield return null;
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("ThrowRock") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.7)
        {
            transform.LookAt(target.transform.position);
            yield return null;
        }
        onAnimationFinished();
    }

    protected override void ChasePlayer()
    {
        if (target == null)
            ChooseRandomTargetInList(ref playersInSightRange);
        Vector3 playerPos = target.transform.position;
        // Update if player moved
        if (Vector3.Distance(playerPos, originalDestPosition) > attackRange + 2f)
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

    [PunRPC]
    protected virtual void EnemyGolemThrowRock()
    {
        Vector3 targetPos = target.transform.position + Vector3.up * verticalOffset;
        transform.LookAt(targetPos);
        Vector3 createPos = transform.position + Vector3.up * verticalOffset + transform.forward * horizontalOffset;
        Vector3 direction = (targetPos - createPos).normalized;
        GameObject m_projectile = Instantiate(enemyProjectile, createPos, Quaternion.identity);
        Rigidbody rb = m_projectile.GetComponent<Rigidbody>();
        rb.AddForce(direction * projectileSpeed, ForceMode.Impulse);
        rb.AddTorque(direction * projectileSpeed, ForceMode.Impulse);
    }
}
