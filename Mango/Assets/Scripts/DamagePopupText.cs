using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopupText : MonoBehaviour
{
    public Animator animator;
    public TextMeshProUGUI damageText;
    // Start is called before the first frame update
    void Start()
    {
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        Destroy(gameObject, clipInfo[0].clip.length);

        transform.position =
            new Vector3(transform.position.x + Random.Range(-3f, 3f), 2f + transform.position.y + Random.Range(-2f, 2f), transform.transform.position.z);
    }

    public void SetText(string text)
    {
        damageText.text = text;
    }
}
