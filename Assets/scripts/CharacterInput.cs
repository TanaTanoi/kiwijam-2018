using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterInput : MonoBehaviour {
	public Camera camera;
	public Slider HealthBar;

	private const float SPEED = 5;
	private const int MAX_HEALTH = 100;

	private double regenRate = 3;
	public double RegenRate { get { return this.regenRate; } }
	private double health;
	public double Health { get { return this.health; } }


	TimeSince timeSinceLastKicked;

	void Start() {
		health = MAX_HEALTH;
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

	void OnTriggerEnter(Collider other) {
		regenRate -= 0.05f;
	}

	void OnTriggerExit(Collider other) {
		regenRate += 0.05f;
	}

	public void TakeHealth(float hp) {
		this.health -= hp;
	}

	private void Kick() {
		if (this.timeSinceLastKicked > 2 && Input.GetKeyDown(KeyCode.Q)) {
			this.timeSinceLastKicked = 0;
			Collider[] hitColliders = Physics.OverlapSphere(transform.forward + transform.position, 2);
			for (int i = 0; i < hitColliders.Length; i++) {
				Collider collider = hitColliders[i];
				Vector3 direction = (collider.transform.position - transform.position).normalized;
				if (Vector3.Dot(direction, transform.forward) > 0.5) {
					Enemy enemy = collider.GetComponent<Enemy>();
					Rigidbody rigidbody = collider.GetComponent<Rigidbody>();
					float multiplier = UnityEngine.Random.Range(450, 600);
					Vector3 force = direction * multiplier + transform.forward * multiplier + transform.up * 300;
					if (enemy != null) {
						enemy.Launch(force, 3);
					} else if (rigidbody != null) {
						rigidbody.AddForce(force);
					}
				}
			}
		}
	}

	void OnTriggerStay(Collider collider) {
		if (collider.gameObject.CompareTag("Toxic")) {
			health -= 0.5 * Time.deltaTime;
		}
  }

	double getHealth() { return health; }

	private void ApplyRegen() {
		health += regenRate * Time.deltaTime;
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
		);

		transform.position += movement * SPEED * Time.deltaTime;
	}

	private void UpdateHealthBar() {
		HealthBar.value = (float)(health/MAX_HEALTH);
	}

	private bool Alive() { return health > 0; }
}
