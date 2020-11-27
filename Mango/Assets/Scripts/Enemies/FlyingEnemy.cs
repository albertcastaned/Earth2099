using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;

public class FlyingEnemy : Enemy
{ 
        public Transform modelTransform;
        public float offsetVertical = 0f;
        public float offsetHorizontal = 0f;

        #region
        public float enemyFireIntervalTime = 1f;
        public float projectileSpeed = 0.2f;
        public GameObject enemyProjectile;

        #endregion


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

        if(photonView.IsMine)
            InvokeRepeating(nameof(ShootProjectile), enemyFireIntervalTime, enemyFireIntervalTime);
    }
    public override void Die()
    {
        OnAnimationFinished onAnimationFinished = delegate ()
        {
            UpdateAnimation("IsDead", true);
            agent.isStopped = false;
            GameObject deathAnimationObj = Instantiate(deathAnimation.gameObject);
            deathAnimationObj.transform.position = modelTransform.position;

            if (photonView.IsMine)
                PhotonNetwork.Destroy(gameObject);
        };
        agent.isStopped = true;
        animator.Play("Die", 0, 0);
        StopCoroutine(nameof(WaitForAnimation));
        StartCoroutine(nameof(WaitForAnimation), onAnimationFinished);
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
    
    void ShootProjectile()
    {
        if (target == null)
            return;

        if (currentState == EnemyState.Chasing)
        {
            photonView.RPC(nameof(EnemyFlyingShootProjectile), RpcTarget.All);
        }

    }

    [PunRPC]
    protected virtual void EnemyFlyingShootProjectile()
    {
        Vector3 targetPos = target.transform.position + new Vector3(Random.Range(-0.2f, 0.2f), 0f, Random.Range(-0.2f, 0.2f));

        Vector3 direction = (targetPos - (modelTransform.position + Vector3.up * offsetVertical + transform.forward * offsetHorizontal)).normalized;
        GameObject m_projectile = Instantiate(enemyProjectile, modelTransform.position + Vector3.up * offsetVertical + transform.forward * offsetHorizontal, Quaternion.identity);
        m_projectile.GetComponent<Rigidbody>().AddForce(direction * projectileSpeed, ForceMode.Impulse);
    }

}
