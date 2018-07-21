using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour {
	public AudioSource normalMusic;
	public AudioSource darkMusic;
	public CharacterInput character;

	// Update is called once per frame
	void FixedUpdate () {
		float ratio = MusicRatio();
		normalMusic.volume = ratio;
		darkMusic.volume = 1 - ratio;
	}

	/* 1 for good music 0 for dark */
	public float MusicRatio() {
		return Mathf.Clamp((float)character.RegenRate, 0, 1);
	}
}
