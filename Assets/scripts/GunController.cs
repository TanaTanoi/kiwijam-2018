using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {
	private const string FIRE_KEY = "space";

	public ParticleSystem shot;

	void Update () {
		if (Input.GetKeyDown(FIRE_KEY)) {
			shot.Play();
		}
	}
}
