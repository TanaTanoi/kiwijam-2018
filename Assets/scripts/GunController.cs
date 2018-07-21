using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {
	private const string FIRE_KEY = "space";

	public ParticleSystem shot;
	public Transform bullet;

	void Update () {
		if (Input.GetKeyDown(FIRE_KEY)) {
			Transform clone = Instantiate(bullet, transform.position, Quaternion.identity);

			shot.Play();
		}
	}
}
