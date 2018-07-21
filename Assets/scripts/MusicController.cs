using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour {
	public AudioSource normalMusic;
	public AudioSource darkMusic;
	public CharacterInput character;

	public float Volume = 1;

	// Update is called once per frame
	void FixedUpdate () {
		float ratio = MusicRatio();
		normalMusic.volume = ratio * this.Volume;
		darkMusic.volume = 1 - ratio * this.Volume;
	}

	/* 1 for good music 0 for dark */
	public float MusicRatio() {
		return Mathf.Clamp((float)character.RegenRate, 0, 1);
	}
}
