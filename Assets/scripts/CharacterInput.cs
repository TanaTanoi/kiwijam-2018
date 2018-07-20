using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterInput : MonoBehaviour {
	public Camera camera;
	CharacterController controller;
	// Use this for initialization
	void Start () {
		this.controller = this.GetComponent<CharacterController>();
	}

	// Update is called once per frame
	void Update () {
		// stuff for mouse direction
		// RaycastHit hit;
        // Ray ray = this.camera.ScreenPointToRay(Input.mousePosition);
        // if (Physics.Raycast(ray, out hit)) {
        //     Vector3 hitPos = hit.point;
		// 	Vector3 direction = hitPos - controller.transform.position;
		// 	direction.Normalize();
        // }

		float vertical = Input.GetAxis("Vertical");
		float horizontal = Input.GetAxis("Horizontal");
		Vector3 direction = new Vector3(-vertical, 0, horizontal);
		this.controller.SimpleMove(direction.normalized * 10);
	}
}
