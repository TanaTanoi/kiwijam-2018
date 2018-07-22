using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {
	private const string FIRE_KEY = "space";
	public AudioSource laserSound;
	public const float SHOOT_COOLDOWN = 0.5f;
	private const float RECOIL = 0.2f;

	public ParticleSystem shot;
	public Transform bullet;
	TimeSince timeSinceLastShot;

	public Transform parent;

	void Start() {
		parent = transform.parent;
		this.laserSound = this.laserSound ?? this.GetComponent<AudioSource>();
	}

	void Update () {
		if (this.timeSinceLastShot > SHOOT_COOLDOWN && Input.GetKeyDown(FIRE_KEY)) {
			this.timeSinceLastShot = 0;
			Transform clone = Instantiate(bullet, transform.position, Quaternion.identity);
			this.laserSound.pitch = UnityEngine.Random.Range(2.1f, 2.3f);
			this.laserSound.Play();

			ApplyRecoil();
			shot.Play();
		}
	}

	private void ApplyRecoil() {
		parent.position -= parent.forward*RECOIL;
	}
}
