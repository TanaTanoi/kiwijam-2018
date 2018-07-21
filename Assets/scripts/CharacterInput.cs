using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInput : MonoBehaviour {
	public Camera camera;

	private const float SPEED = 5;
	private const int MAX_HEALTH = 100;

	private double regenRate = 1;
	private double health;


	void Start() {
		health = MAX_HEALTH;
	}

	void Update() {
		ApplyRegen();
		FaceMouse();
		MovePlayer();
	}

	void OnTriggerStay(Collider other) { health -= 0.5 * Time.deltaTime; }

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
