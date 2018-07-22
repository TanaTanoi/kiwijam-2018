using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor.Animations;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour {
	protected const float BASE_DAMAGE = 5;
	protected const float SHELL_DAMAGE_BOOST = 2;

	protected const double BASE_SPEED = 2;
	protected const double SHELL_SPEED_BOOST = 1;

	public ParticleSystem deathEffect;

	public Transform Target;

	private TimeSince timeSinceHit;
	private const float InvulTime = 0.1f;

	protected NavMeshAgent agent;
	protected Vector3 destination;
	private Animator animator;
	private Rigidbody rigidbody;
	TimeSince timeSinceStunned;
	private float stunDuration = 0;
	protected bool stunned = false;
	public virtual float MaxHealth { get { return 2; } }

	public float Health { get; private set; }

	public float Damage = BASE_DAMAGE;

	protected int shells;
	private AudioSource attackSound;

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
		attackSound = attackSound ?? GetComponent<AudioSource>();
		this.rigidbody.isKinematic = true;
		this.Health = this.MaxHealth;
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.CompareTag("Toxic")) { UpdateShellEffects(1); }
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject.CompareTag("Toxic")) { UpdateShellEffects(-1); }
	}

	void UpdateShellEffects(int delta) {
		shells+=delta;

		agent.speed = (float)(BASE_SPEED + shells * SHELL_SPEED_BOOST);
		Damage = BASE_DAMAGE + shells * SHELL_DAMAGE_BOOST;
	}

	public void OnCollisionEnter(Collision other) {
		this.HitSomething(other.collider);
	}

	public virtual void HitSomething(Collider other) {
		CharacterInput player = other.GetComponent<CharacterInput>();
		if (player != null) {
			Launch(-transform.forward * 200 + transform.up * 50, 0.3f);
			attackSound.Play();
			player.TakeHealth(this.Damage);
		}
	}

	/* Hit by bullet thing */
	void OnParticleCollision(GameObject other) {
		// TODO: balance health loss per particle hit against particle amount
		if (this.timeSinceHit < InvulTime) {
			return;
		}
		this.timeSinceHit = 0;
		this.Health--;
		Debug.Log("Taking HP " + this.Health);
		if (this.Health < 0) {
			Debug.Log("DED");
			ParticleSystem swirl = Instantiate<ParticleSystem>(deathEffect);
			swirl.transform.position = gameObject.transform.position;
			swirl.transform.localScale = Vector3.one * 5;
			swirl.transform.parent = this.transform;
			// swirl.Play();
			Destroy(swirl, swirl.main.duration);
			if (alive) {
				this.Kill(after: swirl.main.duration);
			}
		}
		Vector3 shotDirection = (this.transform.position - other.transform.position).normalized * 100;
		this.Launch(shotDirection, 0.1f); // TODO magic numbers are fun
  }

	public void Update() {
        if (!alive) {return;}

		if (this.Health <= 0) {
			// this.Kill();
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

	public bool Kill(float after = 0) {
        alive = false;
		this.agent.enabled = false;
		this.rigidbody.isKinematic = false;
        gameObject.tag = "Untagged";
        gameController.IncrementKills(gameObject);
		Destroy(this.gameObject, after);
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
