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
		RaycastHit hit;
        Ray ray = this.camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {
            Vector3 hitPos = hit.point;
			Vector3 direction = hitPos - controller.transform.position;
			direction.Normalize();
			this.controller.SimpleMove(direction);
        }
	}
}
