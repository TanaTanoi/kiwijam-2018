﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour {
	public Transform target;
	protected NavMeshAgent agent;
	private Vector3 destination;
	private Rigidbody rigidbody;
	TimeSince timeSinceStunned;
	private float stunDuration = 0;
	private bool stunned = false;
	public virtual float MaxHealth { get { return 1; } }

	public float Health { get; private set; }

	void Start() {
		this.agent = this.GetComponent<NavMeshAgent>();
		this.rigidbody = this.GetComponent<Rigidbody>();
		this.rigidbody.isKinematic = true;
	}

	void OnParticleCollision(GameObject other) {
		// TODO: balance health loss per particle hit against particle amount
		this.Health--;
		Vector3 shotDirection = (other.transform.position - this.transform.position).normalized;
		this.Launch(shotDirection, 0.1f);
    }

	public void Update() {
		if (this.Health <= 0) {
			this.Kill();
		}
		if (this.agent.isOnNavMesh && Vector3.Distance(destination, target.position) > 1.0f) {
			this.UpdateBehaviour();
		}
		// debug
		if (Input.GetKeyDown(KeyCode.Space)) {
			this.Launch(Vector3.right * 100, 3);
		}

		if (this.timeSinceStunned > this.stunDuration) {
			this.stunned = false;
			this.agent.enabled = true;
			this.rigidbody.isKinematic = true;
			if (!this.agent.isOnNavMesh && !this.ReattachToNavmesh()) {
				// if we are not on the mesh and we can't get on the mesh, give up
				return;
			}
			destination = target.position;
            agent.destination = destination;
		}
	}

	public virtual void UpdateBehaviour() {
        this.destination = target.position;
        this.agent.destination = destination;
	}

	public bool Kill() {
		this.agent.enabled = false;
		this.rigidbody.isKinematic = false;
		return true;
	}

	/* Reattaches to the mesh if we can (true) and otherwise destorys this (false) */
	public bool ReattachToNavmesh() {
		NavMeshHit hit;

		if (NavMesh.SamplePosition(this.transform.position, out hit, 2.0f, NavMesh.AllAreas)) {
			this.agent.Warp(hit.position);
			return true;
		} else {
			Destroy(this.gameObject);
			return false;
		}
	}

	/* detaches it from the navmesh and sends it flying */
	public void Launch(Vector3 direction, float stunDuration) {
		this.agent.enabled = false;
		this.rigidbody.isKinematic = false;
		if (this.stunned) {
			this.stunDuration += stunDuration;
		} else {
			this.timeSinceStunned = 0;
			this.stunDuration = stunDuration;
			this.stunned = true;
		}
		this.rigidbody.AddForce(direction);
	}
}