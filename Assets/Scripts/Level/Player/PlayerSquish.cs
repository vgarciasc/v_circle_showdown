using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSquish : MonoBehaviour {
	Rigidbody2D rb;
	Vector3 last_velocity,
			current_velocity;

	void Start() {
		rb = this.GetComponent<Rigidbody2D>();
		last_velocity = current_velocity = rb.velocity;
	}

	void Update () {
		last_velocity = current_velocity;
		current_velocity = rb.velocity;

		checkForSquish();
	}

	void checkForSquish() {
		float difference = current_velocity.magnitude - last_velocity.magnitude;
		if (difference > 10) {
			StartCoroutine(squish(difference));
		}
	}

	IEnumerator squish(float modifier) {
		int times = 5;
		float increment = 0.075f;
		increment = increment * modifier / 50f;

		for (int i = 0; i < times; i++) {
			this.transform.localScale = new Vector3(this.transform.localScale.x,
													this.transform.localScale.y - increment);
			yield return new WaitForEndOfFrame();
		}

		for (int i = 0; i < times; i++) {
			yield return new WaitForEndOfFrame();
		}

		for (int i = 0; i < times; i++) {
			this.transform.localScale = new Vector3(this.transform.localScale.x,
													this.transform.localScale.y + increment);	
			yield return new WaitForEndOfFrame();
		}
	}
}
