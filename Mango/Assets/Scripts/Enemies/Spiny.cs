using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class Spiny : Enemy
{
    public float enemyFireIntervalTime = 1f;
    public float projectileSpeed = 0.2f;
    public GameObject enemyProjectile;

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
        if (target == null)
            return;

        if (currentState == EnemyState.Chasing)
        {
            photonView.RPC(nameof(SpinyShootProjectiles), RpcTarget.All);
        }
    }

    [PunRPC]
    protected virtual void SpinyShootProjectiles()
    {

        // Shoot 4 cardinal directions

        Vector3 direction = transform.forward;
        GameObject m_projectile = Instantiate(enemyProjectile, transform.position + Vector3.up * 2f + direction * 5f, Quaternion.identity);
        m_projectile.transform.LookAt(-direction * 100);
        m_projectile.GetComponent<Rigidbody>().AddForce(direction * projectileSpeed, ForceMode.Impulse);

        direction = -transform.right;
        m_projectile = Instantiate(enemyProjectile, transform.position + Vector3.up * 2f + direction * 5f, Quaternion.identity);
        m_projectile.transform.LookAt(-direction * 100);
        m_projectile.GetComponent<Rigidbody>().AddForce(direction * projectileSpeed, ForceMode.Impulse);

        direction = -transform.forward;
        m_projectile = Instantiate(enemyProjectile, transform.position + Vector3.up * 2f + direction * 5f, Quaternion.identity);
        m_projectile.transform.LookAt(-direction * 100);
        m_projectile.GetComponent<Rigidbody>().AddForce(direction * projectileSpeed, ForceMode.Impulse);

        direction = transform.right;
        m_projectile = Instantiate(enemyProjectile, transform.position + Vector3.up * 2f + direction * 5f, Quaternion.identity);
        m_projectile.transform.LookAt(-direction * 100);
        m_projectile.GetComponent<Rigidbody>().AddForce(direction * projectileSpeed, ForceMode.Impulse);
    }
}
