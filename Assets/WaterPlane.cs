﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPlane : MonoBehaviour {
	public ParticleSystem splashEffect;

	void Update() {
	}

	void OnTriggerEnter(Collider other) {
		Rigidbody otherRigidbody = other.GetComponent<Rigidbody>();
		if (otherRigidbody != null) {
			ParticleSystem splash = Instantiate<ParticleSystem>(splashEffect);
			splash.transform.position = other.gameObject.transform.position;
			splash.Play();
			Destroy(splash, splash.main.duration);
			// if its falling, remove it
			if (Vector3.Dot(otherRigidbody.velocity.normalized, Vector3.down) > 0) {
				Destroy(other.gameObject);
			}
		}
	}
}
