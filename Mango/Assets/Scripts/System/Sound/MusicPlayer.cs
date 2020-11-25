using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAudioPlayer : MonoBehaviour
{
    public AudioClip[] clips;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = ChooseRandomClip();
        audioSource.Play();
    }

    private AudioClip ChooseRandomClip()
    {
        return clips[Random.Range(0, clips.Length)];
    }

}
