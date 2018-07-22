using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {
    private Transform rubbishBin;
	private const string FIRE_KEY = "space";
	public AudioSource laserSound;
	public const float SHOOT_COOLDOWN = 0.75f;
	private Rigidbody rb;
  private Animator animator;

	public ParticleSystem shot;
	public Transform bullet;
	TimeSince timeSinceLastShot;

	void Start() {
    animator = transform.parent.GetComponent<Animator>();
		this.laserSound = this.laserSound ?? this.GetComponent<AudioSource>();
		this.rb = transform.parent.GetComponent<Rigidbody>();
        this.rubbishBin = GameController.Instance.GetSpawnerTransform();
	}

	void Update () {
		if (this.timeSinceLastShot > SHOOT_COOLDOWN && Input.GetKeyDown(FIRE_KEY)) {
			this.timeSinceLastShot = 0;
			Transform clone = Instantiate(bullet, transform.position + transform.right, Quaternion.identity, rubbishBin);
			this.laserSound.pitch = UnityEngine.Random.Range(2.1f, 2.3f);
			this.laserSound.Play();
			this.rb.AddForce(-rb.transform.forward * 1000);
			CameraController.instance.Shake(0.01f, 0.3f);
      animator.SetTrigger("shoot");

			shot.Play();
		}
	}
}
