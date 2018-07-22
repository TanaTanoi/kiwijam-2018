using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInput : MonoBehaviour {
	public Camera camera;
	public Slider HealthBar;

	private const float SPEED = 10000;
	private const int MAX_HEALTH = 100;

	private double regenRate = 3;
	public double RegenRate { get { return this.regenRate; } }
	private double health;
	public double Health { get { return this.health; } }

	private Animator animator;
	private int shells = 0;
	private const double SHELL_DAMAGE = 0.5;

	TimeSince timeSinceLastKicked;
	public AudioSource kickEffect;

	void Start() {
		animator = GetComponent<Animator>();
		kickEffect = GetComponent<AudioSource>();
	}

	void Update() {
		if (Alive()) {
			ApplyRegen();
			FaceMouse();
			MovePlayer();
			Kick();

			UpdateHealthBar();
		}
		else {
			// Do death things
		}
	}

    public void RestoreHealth() {
        health = MAX_HEALTH;
    }

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag("Toxic")) { shells++; }
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject.CompareTag("Toxic")) { shells--; }
	}

	public void TakeHealth(float hp) {
		this.health -= hp;
	}

	private void Kick() {
		if (this.timeSinceLastKicked > 2 && Input.GetKeyDown(KeyCode.Q)) {
			kickEffect.Play();
			CameraController.instance.Shake(0.05f, 0.5f);
			this.timeSinceLastKicked = 0;
			Collider[] hitColliders = Physics.OverlapSphere(transform.forward * 0.1f + transform.position, 3);
			for (int i = 0; i < hitColliders.Length; i++) {
				Collider collider = hitColliders[i];
				Vector3 direction = (collider.transform.position - transform.position).normalized;
				if (Vector3.Dot(direction, transform.forward) > 0.5) {
					Enemy enemy = collider.GetComponent<Enemy>();
					Rigidbody rigidbody = collider.GetComponent<Rigidbody>();
					float multiplier = UnityEngine.Random.Range(300, 450);
					Vector3 force = direction * multiplier + transform.forward * multiplier + transform.up * 150;
					if (enemy != null) {
						enemy.Launch(force, 2);
					} else if (rigidbody != null) {
						rigidbody.AddForce(force);
					}
				}
			}
		}
	}

	double getHealth() { return health; }

	private void ApplyRegen() {
		double shellDamage = shells * SHELL_DAMAGE;

		health += (regenRate - shellDamage) * Time.deltaTime;
		if (health > MAX_HEALTH) health = MAX_HEALTH;
	}

	private void FaceMouse() {
		RaycastHit hit;
		Ray ray = this.camera.ScreenPointToRay(Input.mousePosition);

		if (Physics.Raycast(ray, out hit)) {
			Vector3 hitPos = hit.point;

			transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
		}
	}

	private void MovePlayer() {
		Vector3 movement = new Vector3(
			-Input.GetAxis("Vertical"),
			0,
			Input.GetAxis("Horizontal")
		).normalized;

		animator.SetBool("moving", movement.magnitude != 0);

		Rigidbody rigidbody = GetComponent<Rigidbody>();
		rigidbody.AddForce(movement * SPEED * Time.deltaTime * rigidbody.mass);
	}

	private void UpdateHealthBar() {
		HealthBar.value = (float)(health/MAX_HEALTH);
	}

	public bool Alive() {
        return health > 0;
    }
}
