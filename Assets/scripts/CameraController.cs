using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	public GameObject player;
	private Vector3 offset;

	// How long the object should shake for.
	private float shakeDuration = 0f;

	// Amplitude of the shake. A larger value shakes the camera harder.
	private float shakeAmount = 0.7f;
	private float decreaseFactor = 1.0f;

	Vector3 originalPos;

	public static CameraController instance;

	void Start () {
		instance = this;
		offset = transform.position - player.transform.position;
	}

	public void Shake(float duration, float amount = 0.7f) {
		this.shakeDuration = duration;
		this.shakeAmount = amount;
	}

	void LateUpdate () {
		if (shakeDuration > 0) {
			transform.localPosition = player.transform.position + offset + Random.insideUnitSphere * shakeAmount;

			shakeDuration -= Time.deltaTime * decreaseFactor;
		} else {
			shakeDuration = 0f;
			transform.localPosition = player.transform.position + offset;
		}
	}
}
