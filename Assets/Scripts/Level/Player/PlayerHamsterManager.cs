using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHamsterManager : MonoBehaviour {
	Rigidbody2D player_rb;
	public Player player;
	public FixRotation fix_rotation;
	public Animator animator;
	public SpriteRenderer hamsterSprite;

	void Start() {
		player_rb = GetComponent<Rigidbody2D>();
		player = this.GetComponent<Player>();
		fix_rotation = GetComponentInChildren<FixRotation>();
	}

	void Update () {
		// Debug.Log("Magnitude: " + player_rb.velocity.magnitude);
		// Debug.Log("Velocity: " + player_rb.velocity);
		if (player_rb.velocity.magnitude <= 0.7f) {
			// Debug.Log("Not moving");
			animator.SetBool("move", false);
			// animator.SetBool("idle", true);
		}
		else {
			// Debug.Log("Moving");
			animator.SetBool("move", true);
			// animator.SetBool("idle", false);		
		}

		if (player_rb.velocity.x > 0.5f) {
			// Debug.Log("Moving right");
			flipSprite(false);
		}
		else if (player_rb.velocity.x < -0.5f) {
			// Debug.Log("Moving left");
			flipSprite(true);
		}

		fix_rotation.enabled = player.is_on_ground;
	}

	void flipSprite(bool value) {
		hamsterSprite.GetComponent<SpriteRenderer>().flipX = value;
	}
}
