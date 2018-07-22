using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor.Animations;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour {

	public Transform Target;

	protected NavMeshAgent agent;
	protected Vector3 destination;
	private Animator animator;
	private Rigidbody rigidbody;
	TimeSince timeSinceStunned;
	private float stunDuration = 0;
	protected bool stunned = false;
	public virtual float MaxHealth { get { return 1; } }

	public float Health { get; private set; }

	public virtual float Damage { get { return 1; } }

    private GameController gameController;
    private bool alive = true;

	public void Spawn(Transform target) {
        this.Target = target;
		this.agent = this.GetComponent<NavMeshAgent>();
		this.rigidbody = this.GetComponent<Rigidbody>();
		this.rigidbody.isKinematic = true;
		this.Health = this.MaxHealth;
		this.animator = GetComponent<Animator>();
        this.animator.SetFloat("speed", -1);
		this.destination = Target.position;
		this.agent.destination = destination;
	}

	void Awake() {
        gameController = GameController.Instance;

		this.agent = this.agent ?? this.GetComponent<NavMeshAgent>();
		this.rigidbody = this.rigidbody ?? this.GetComponent<Rigidbody>();
		this.animator = this.animator ?? GetComponent<Animator>();
		this.rigidbody.isKinematic = true;
		this.Health = this.MaxHealth;
	}

	public void OnCollisionEnter(Collision other) {
		this.HitSomething(other.collider);
	}

	public virtual void HitSomething(Collider other) {
		CharacterInput player = other.GetComponent<CharacterInput>();
		if (player != null) {
			Debug.Log("du hurt me ");
			player.TakeHealth(this.Damage);
		}
	}

	/* Hit by bullet thing */
	void OnParticleCollision(GameObject other) {
		// TODO: balance health loss per particle hit against particle amount
		this.Health--;
		Vector3 shotDirection = (this.transform.position - other.transform.position).normalized * 100;
		this.Launch(shotDirection, 0.1f); // TODO magic numbers are fun
    }

	public void Update() {
        if (!alive) {return;}

		if (this.Health <= 0) {
			this.Kill();
		}
		this.UpdateBehaviour();
		this.animator.SetFloat("speed", this.agent.velocity.magnitude);

		if (this.stunned && this.timeSinceStunned > this.stunDuration) {
			this.stunned = false;
			this.agent.enabled = true;
			this.rigidbody.isKinematic = true;
			if (!this.agent.isOnNavMesh && !this.ReattachToNavmesh()) {
				// if we are not on the mesh and we can't get on the mesh, give up
				return;
			}
			this.UpdateBehaviour();
		}
	}

	public virtual void UpdateBehaviour() {
		if (this.agent.isOnNavMesh && Vector3.Distance(destination, Target.position) > 1.0f) {
	        this.destination = Target.position;
	        this.agent.destination = destination;
		}
	}

	public bool Kill() {
        alive = false;
		this.agent.enabled = false;
		this.rigidbody.isKinematic = false;
        gameObject.tag = "Untagged";
        gameController.IncrementKills(gameObject);
		return true;
	}

	/* Reattaches to the mesh if we can (true) and otherwise destorys this (false) */
	public bool ReattachToNavmesh() {
		NavMeshHit hit;

		if (NavMesh.SamplePosition(this.transform.position, out hit, 2.0f, NavMesh.AllAreas)) {
			this.agent.Warp(hit.position);
			return true;
		} else {
			// Destroy(this.gameObject);
			return false;
		}
	}

	/* detaches it from the navmesh and sends it flying */
	public virtual void Launch(Vector3 direction, float stunDuration) {
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
