using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChargeEnemy : Enemy {
	List<Vector3> movePositions = new List<Vector3>();
	int currentMovePositionIndex = 0;
	private bool isCharging = false;
	private bool IsCharging {
		get { return this.isCharging; }
		set {
			this.isCharging = value;
			if (this.isCharging) {
				this.agent.speed = 40;
			} else {
				this.agent.speed = 1;
			}
		}
	}

	public virtual float MaxHealth { get { return 10; } }

	private bool FinishedCharging { get { return this.IsCharging && this.currentMovePositionIndex >= this.movePositions.Count; } }
	public const float CHARGE_DISTANCE = 30;

	public override void HitSomething(Collider collider) {
		Vector3 direction = (collider.transform.position - transform.position).normalized;
		Enemy enemy = collider.GetComponent<Enemy>();
		Rigidbody rigidbody = collider.GetComponent<Rigidbody>();
		float multiplier = UnityEngine.Random.Range(200, 400);
		Vector3 force = direction * multiplier + transform.up * 100;
		if (enemy != null) {
			enemy.Launch(force, 1);
		} else if (rigidbody != null) {
			rigidbody.AddForce(force);
		}
		base.HitSomething(collider);
	}

	public override void UpdateBehaviour() {
		if (this.stunned || !this.agent.isOnNavMesh) {
			return;
		}
		if (this.ShouldCharge() && !this.IsCharging && Vector3.Distance(this.transform.position, this.Target.transform.position) < CHARGE_DISTANCE) {
			this.CalculateSwoopPath();
			this.agent.velocity = Vector3.zero;
			this.IsCharging = true;
			this.currentMovePositionIndex = 0;
		} else if (this.FinishedCharging) {
			// if we have finished swooping and are within swooping distance, set us to not swooping
			this.IsCharging = false;
			this.currentMovePositionIndex = 0;
		} else if (this.IsCharging) {
			this.destination = this.movePositions[this.currentMovePositionIndex];
			this.agent.destination = this.destination;
			Debug.DrawLine(transform.position, agent.destination, Color.red);
			if (this.agent.remainingDistance < 3) {
				this.currentMovePositionIndex++;
			}
			for (int i = 0; i < this.movePositions.Count - 1; i++) {
				Debug.DrawLine(this.movePositions[i], this.movePositions[i + 1], Color.blue);
			}
		} else {
			// else, do the regular behaviour until we are withinin swooping distance
			base.UpdateBehaviour();
		}
	}

	public bool ShouldCharge() {
		return UnityEngine.Random.Range(0, 1f) > 0.8f;
	}

	public override void Launch(Vector3 direction, float stunDuration) {
		if (this.Health <= 0) {
			base.Launch(direction, stunDuration);
		}
		// can't launch things
	}

	public void CalculateSwoopPath() {
		Vector3 direction = this.Target.transform.position - this.transform.position;
		Vector3 bisector = new Vector3(-direction.z, direction.y, direction.x).normalized;
		float multiplier = UnityEngine.Random.Range(-1f, 1f);
		bisector *= multiplier >= 0 ? 1 : -1;
		this.movePositions.Clear();
		this.movePositions.Add(this.transform.position);
		this.movePositions.Add(this.Target.transform.position);
		this.movePositions.Add(this.Target.transform.position + direction * 0.4f + direction.normalized * 10);
	}
}