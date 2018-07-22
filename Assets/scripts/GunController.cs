using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {
	private const string FIRE_KEY = "space";
	public AudioSource laserSound;
	public const float SHOOT_COOLDOWN = 0.5f;

	public ParticleSystem shot;
	public Transform bullet;
	TimeSince timeSinceLastShot;

	void Start() {
		this.laserSound = this.laserSound ?? this.GetComponent<AudioSource>();
	}

	void Update () {
		if (this.timeSinceLastShot > SHOOT_COOLDOWN && Input.GetKeyDown(FIRE_KEY)) {
			this.timeSinceLastShot = 0;
			Transform clone = Instantiate(bullet, transform.position, Quaternion.identity);
			this.laserSound.pitch = UnityEngine.Random.Range(2.1f, 2.3f);
			this.laserSound.Play();

			shot.Play();
		}
	}
}
