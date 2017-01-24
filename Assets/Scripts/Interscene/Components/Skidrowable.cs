using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Skidrowable : MonoBehaviour {
	Rigidbody2D rb;

	void Start() {
		rb = this.GetComponent<Rigidbody2D>();
	}

	void OnTriggerStay2D(Collider2D target) {
		if (target.tag == "Skidrow") {
			addForce(target.transform.up);
		}
	}

	void addForce(Vector2 direction) {
		rb.AddForce(direction.normalized * 30, ForceMode2D.Force);
	}
}
