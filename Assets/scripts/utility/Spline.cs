using System;
using System.Collections.Generic;
using UnityEngine;

public class Spline {
	private System.Collections.Generic.List<Vector2> controlPoints = new System.Collections.Generic.List<Vector2>();

	private float totalLength = 0f;

	public enum Type {
		Casteljau,
		CatmullRom,
	}

	private Type splineType;
	private bool looping = false;

	public Spline(Type splineType = Type.Casteljau, bool looping = false) {
		this.splineType = splineType;
		this.looping = looping;
	}

	public void AddControlPoint(Vector2 controlPoint) {
		if (this.IsCatmullRom && this.ControlPointCount > 0) {
			Vector2 lastPoint = this.controlPoints[this.ControlPointCount - 1];
			this.totalLength += Vector2.Distance(lastPoint, controlPoint);
		}
		this.controlPoints.Add(controlPoint);
	}

	public void AddControlPoint(Vector3 controlPointParam) {
        Vector2 controlPoint = new Vector2(controlPointParam.x, controlPointParam.z);
		if (this.IsCatmullRom && this.ControlPointCount > 0) {
			Vector2 lastPoint = this.controlPoints[this.ControlPointCount - 1];
			this.totalLength += Vector2.Distance(lastPoint, controlPoint);
		}
		this.controlPoints.Add(controlPoint);
	}

	public Vector2 GetControlPoint(int index) {
		return this.controlPoints[index];
	}

	public void SetControlPoint(int index, Vector2 controlPoint) {
		if (this.IsCatmullRom && this.ControlPointCount > 0) {
			this.RecalculateTotalLength();
		}
		this.controlPoints[index] = controlPoint;
	}

	public int ControlPointCount {
		get { return this.controlPoints.Count; }
	}

	public Vector2 GetPointAt(float t) {
		System.Diagnostics.Debug.Assert(ControlPointCount > 1);
		if (splineType == Type.Casteljau) {
			return CastelJauGetPointAt(t);
		} else if (splineType == Type.CatmullRom) {
			return CatmullRomGetPointAt(t);
		}

		throw new System.ArgumentException("Unspecified type!");
	}

	private Vector2 CastelJauGetPointAt(float t) {
		Vector2[] casteljauPoints = new Vector2[this.ControlPointCount];
		this.controlPoints.CopyTo(casteljauPoints);

		for (int i = 0; i < ControlPointCount - 1; ++i) {
			for (int j = 0; j < ControlPointCount - i - 1; ++j)
				casteljauPoints[j] = casteljauPoints[j + 1] * t + casteljauPoints[j] * (1 - t);
		}

		return casteljauPoints[0];
	}

	private Vector2 CatmullRomGetPointAt(float t) {
		// first, evaluate between what points t is,
		// as well as the distance between those nodes and the distance at the start node
		int startIndex = 0;
		float distanceBetweenIndexAndNext = 0;
		float currentDistance = 0;
		for (int i = 0; i < this.ControlPointCount - 1; i++) {
			Vector2 currentPoint = this.controlPoints[i];
			Vector2 nextPoint = this.controlPoints[i + 1];
			float distanceBetweenPoints = Vector2.Distance(currentPoint, nextPoint);
			if (distanceBetweenPoints == 0) {
				continue;
			}
			if (currentDistance + distanceBetweenPoints > t * this.totalLength) {
				distanceBetweenIndexAndNext = distanceBetweenPoints;
				startIndex = i;
				break;
			}
			currentDistance += distanceBetweenPoints;
		}

		// Then get the four points we want to use
		Vector2 p0;
		if (startIndex == 0 && !this.looping) {
			p0 = this.controlPoints[0];
		} else {
			int newIndex = startIndex - 1 < 0 ? this.ControlPointCount - 1 : startIndex - 1;
			p0 = this.controlPoints[newIndex];
		}
		Vector2 p1 = this.controlPoints[startIndex];
		// startIndex will always be the second to last point or less, meaning this will at most be
		// the last point
		Vector2 p2 = this.controlPoints[(startIndex + 1) % this.ControlPointCount];
		Vector2 p3;
		// extrapolate if we aren't looping and we're at the end of the points
		if (startIndex + 1 >= this.ControlPointCount -1 && !this.looping) {
			p3 = (p2 - p1) + p1;
		} else {
			p3 = this.controlPoints[(startIndex + 2) % this.ControlPointCount];
		}

		// then calculate the local t value based on the length at p1,
		// the length of |p1 - p2|, and the t value's length along the spline

	 	float localTValue = ((t * this.totalLength) - currentDistance) / distanceBetweenIndexAndNext;
		return CatmullRom(p0, p1, p2, p3, localTValue);
	}

	public int IndexOfPointAtDistance(float distance) {
		if (distance > this.totalLength) {
			return 0;
		}
		float currentDistance = this.totalLength;
		for (int i = 0; i < this.ControlPointCount - 1; i++) {
			Vector2 currentPoint = this.controlPoints[i];
			Vector2 nextPoint = this.controlPoints[i + 1];
			float distanceBetweenPoints = Vector2.Distance(currentPoint, nextPoint);
			if (currentDistance < distanceBetweenPoints) {
				return i;
			}
			currentDistance -= distanceBetweenPoints;
		}
		return 0;
	}

	public Vector2 CatmullRom(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t) {
		return 	((2.0f * p1) +
				(-p0 + p2) * t +
				((2.0f * p0) - (5.0f * p1) + (4.0f * p2) - p3) * t * t +
				(-p0 + (3.0f * p1) - (3.0f * p2) + p3) * t * t * t ) * 0.5f;
	}

	public List<Vector2> GetPoints(int resolution) {
		List<Vector2> points = new List<Vector2>(resolution);
		points.Add(this.GetControlPoint(0));

		for (int i = 1; i < resolution - 1; ++i) {
			float t = (float) i / (float) (resolution - 1);
			points.Add(this.GetPointAt(t));
		}

		points.Add(this.GetControlPoint(this.ControlPointCount - 1));

		return points;
	}

	public override string ToString() {
		string points = "";
		for (int i = 0; i < this.controlPoints.Count; ++i) {
			points += this.controlPoints[i];
			if (i < this.controlPoints.Count - 1)
				points += ", ";
		}

		return String.Format("[Spline: Points={0}]", points);
	}

	private void RecalculateTotalLength() {
		this.totalLength = 0f;
		for (int i = 1; i < this.ControlPointCount; i++) {
			Vector2 current = this.controlPoints[i];
			Vector2 previous = this.controlPoints[i - 1];
			this.totalLength += Vector2.Distance(current, previous);
		}
	}

	private bool IsCatmullRom {
		get { return this.splineType == Type.CatmullRom; }
	}
}
