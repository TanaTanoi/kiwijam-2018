using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour {
	public Transform target;
	protected NavMeshAgent agent;
	private Vector3 destination;

	void Start() {
		agent = this.GetComponent<NavMeshAgent>();
	}

	public void Update() {
		if (Vector3.Distance(destination, target.position) > 1.0f) {
            destination = target.position;
            agent.destination = destination;
        }
	}
}
