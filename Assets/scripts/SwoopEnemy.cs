using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SwoopEnemy : Enemy {
	Spline path = new Spline();
	List<Vector3> movePositions = new List<Vector3>();
	int currentMovePositionIndex = 0;
	private bool isSwooping = false;
	private bool IsSwooping {
		get { return this.isSwooping; }
		set {
			this.isSwooping = value;
			if (this.isSwooping) {
				this.agent.speed = 15;
				this.agent.angularSpeed = 360;
			} else {
				this.agent.speed = 3;
				this.agent.angularSpeed = 120;
			}
		}
	}

	public override float MaxHealth { get { return 2; } }

	private bool FinishedSwooping { get { return this.isSwooping && this.currentMovePositionIndex >= this.movePositions.Count; } }
	public float swoopOffset = 10;
	public const float SWOOPING_DISTANCE = 20;

	public override void UpdateBehaviour() {
		if (this.stunned || !this.agent.isOnNavMesh) {
			return;
		}
		if (this.ShouldSwoop() && !this.IsSwooping && Vector3.Distance(this.transform.position, this.Target.transform.position) < SWOOPING_DISTANCE) {
			this.CalculateSwoopPath();
			this.agent.velocity = Vector3.zero;
			this.IsSwooping = true;
			this.currentMovePositionIndex = 0;
		} else if (this.FinishedSwooping) {
			// if we have finished swooping and are within swooping distance, set us to not swooping
			this.IsSwooping = false;
			this.currentMovePositionIndex = 0;
		} else if (this.IsSwooping) {
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

	public bool ShouldSwoop() {
		return UnityEngine.Random.Range(0, 1f) > 0.8f;
	}

	public void CalculateSwoopPath() {
		this.path = new Spline();
		Vector3 direction = this.Target.transform.position - this.transform.position;
		Vector3 bisector = new Vector3(-direction.z, direction.y, direction.x).normalized;
		float multiplier = UnityEngine.Random.Range(-1f, 1f);
		bisector *= multiplier >= 0 ? 1 : -1;
		this.path.AddControlPoint(this.transform.position);
		// this.path.AddControlPoint(this.transform.position + this.transform.forward * 100 + this.transform.right * 100);
		this.path.AddControlPoint(this.Target.transform.position + bisector * swoopOffset);
		this.path.AddControlPoint(this.Target.transform.position + direction);
		this.movePositions.Clear();
		for (float t = 0; t < 1; t += 0.2f) {
			Vector2 pos = this.path.GetPointAt(t);
			this.movePositions.Add(new Vector3(pos.x, direction.y, pos.y));
		}
	}
}