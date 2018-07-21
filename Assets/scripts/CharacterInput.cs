using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterInput : MonoBehaviour {
	public Camera camera;
	CharacterController controller;

	void Start () {
		this.controller = this.GetComponent<CharacterController>();
	}

	void Update () {
		// stuff for mouse direction
		// RaycastHit hit;
        // Ray ray = this.camera.ScreenPointToRay(Input.mousePosition);
        // if (Physics.Raycast(ray, out hit)) {
        //     Vector3 hitPos = hit.point;
		// 	Vector3 direction = hitPos - controller.transform.position;
		// 	direction.Normalize();
        // }
		FaceMouse();

		float vertical = Input.GetAxis("Vertical");
		float horizontal = Input.GetAxis("Horizontal");
		Vector3 direction = new Vector3(-vertical, 0, horizontal);
		this.controller.SimpleMove(direction.normalized * 10);
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
