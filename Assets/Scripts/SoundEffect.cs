using UnityEngine;
using System.Collections;

public class SoundEffect : MonoBehaviour {

	public AudioClip impact;
	public AudioSource audioSource;
	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnMouseDown ()
	{
		if (!audioSource.isPlaying)
			audioSource.Play ();
	}

}
