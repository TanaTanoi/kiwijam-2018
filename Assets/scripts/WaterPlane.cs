using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPlane : MonoBehaviour {
	public ParticleSystem splashEffect;
    private GameController gameController;

    void Awake() {
        gameController = GameController.Instance;
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
                if (other.CompareTag("Enemy")) {
                    gameController.IncrementKills();
                }
                Destroy(other.gameObject);
			}
		}
	}
}
