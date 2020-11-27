using Photon.Pun;
using UnityEngine;


public class Bullet : MonoBehaviourPun
{
    public float damage = 10;
    public Transform collisionEffect;
    public Transform collisionEnemyEffect;
    private AudioManager audioManager;
    private bool hit = false;
    private Renderer m_renderer;


    void Awake()
    {
        m_renderer = GetComponent<Renderer>();

    }
    void Start()
    {
        audioManager = GetComponent<AudioManager>();
        audioManager.Play("Shoot");
        Destroy(gameObject, 5.0f);
    }

    void Update()
    {
        if(hit)
        {
            if(!audioManager.GetSource("Shoot").isPlaying)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hit)
            return;
        if (collision.gameObject.CompareTag("Enemy"))
        {
            hit = true;
            if(m_renderer != null)
                m_renderer.enabled = false;
            collision.gameObject.GetComponent<Enemy>().photonView.RPC("ReduceHealth", RpcTarget.AllBufferedViaServer, (int)Random.Range(damage -2, damage + 2));
            Instantiate(collisionEnemyEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        else if (!collision.gameObject.CompareTag("Player") || !collision.gameObject.CompareTag("Bullet"))
        {
            Instantiate(collisionEffect, transform.position, Quaternion.identity);
            if (m_renderer != null)
                m_renderer.enabled = false;
            hit = true;
            Destroy(gameObject);

        }
    }
}
