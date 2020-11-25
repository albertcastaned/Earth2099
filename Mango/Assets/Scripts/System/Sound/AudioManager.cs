using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{


	public Sound[] sounds;


	void Awake()
	{
		foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.source.clip = s.clip;
			s.source.loop = s.loop;
			s.source.spatialBlend = s.spatialBlend;
			s.source.outputAudioMixerGroup = s.mixerGroup;
			s.source.minDistance = s.minDistance;
			s.source.maxDistance = s.maxDistance;
			s.source.rolloffMode = s.audioRolloffMode;
		}
	}

	public void Play(string sound)
	{
		Debug.Log("Audio playing");
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));
		s.source.spatialBlend = s.spatialBlend;
		s.source.minDistance = s.minDistance;
		s.source.maxDistance = s.maxDistance;
		s.source.rolloffMode = s.audioRolloffMode;
		s.source.Play();
	}

	public void Stop(string sound)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}
		s.source.Stop();
	}

	public AudioSource GetSource(string sound)
    {
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return null;
		}
		return s.source;

	}
}
