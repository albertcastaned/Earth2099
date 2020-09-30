using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    private new Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();    
    }

    void OnTriggerEnter(Collider other)
    {
        switch(other.gameObject.tag)
        {
            case "Player":
                renderer.enabled = false;
                break;
            case "Bullet":
                Destroy(other.gameObject);
                int direction = Random.Range(0, 3);
                switch(direction)
                {
                    case 0:
                        transform.Translate(new Vector3(-0.2f, 0.1f, 0f));
                        break;
                    case 1:
                        transform.Translate(new Vector3(0.2f, 0.1f, 0f));
                        break;

                    case 2:
                        transform.Translate(new Vector3(0f, 0.1f, -0.2f));
                        break;
                    case 3:
                        transform.Translate(new Vector3(0f, 0.1f, 0.2f));
                        break;
                }
                break;

        }
    }

    void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Player":
                renderer.enabled = true;
                break;
        }
    }
}
