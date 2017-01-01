using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pausable : MonoBehaviour {
	Vector3 savedVelocity;
	float savedAngularVelocity;
	RigidbodyConstraints2D savedConstraints;

	Rigidbody2D rb;

	void Start() {
		rb = this.GetComponentInChildren<Rigidbody2D>();
	}

	public void OnPause() {
		savedVelocity = rb.velocity;
		savedAngularVelocity = rb.angularVelocity;
		savedConstraints = rb.constraints;
		rb.constraints = RigidbodyConstraints2D.FreezeAll;
		rb.Sleep();
	}
 
	public void OnUnPause() {
		rb.WakeUp();
		rb.constraints = savedConstraints;
		rb.velocity = savedVelocity;
		rb.angularVelocity = savedAngularVelocity;
	}
}
