using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInput : MonoBehaviour {
	public Camera camera;

	// private Rigidbody rb;
	private const float speed = 5;

	// void Start() {
	// 	rb = GetComponent<Rigidbody>();
	// }

	void Update() {
		FaceMouse();

		Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

		transform.Translate(movement * speed * Time.deltaTime);
	}

	private void FaceMouse() {
		RaycastHit hit;
		Ray ray = this.camera.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit)) {
			Vector3 hitPos = hit.point;

			transform.LookAt(new Vector3(hit.point.x, transform.position.y, hit.point.z));
		}
	}
}
