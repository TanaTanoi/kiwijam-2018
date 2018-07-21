using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInput : MonoBehaviour {
	public Camera camera;

	private const float SPEED = 5;
	private const int MAX_HEALTH = 100;

	private double regenRate = 1;
	private double health;

	TimeSince timeSinceLastKicked;

	void Start() {
		health = MAX_HEALTH;
	}

	void Update() {
		ApplyRegen();
		FaceMouse();
		MovePlayer();
		Kick();
	}

	void OnTriggerStay(Collider other) { health -= 0.5 * Time.deltaTime; }

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
}
