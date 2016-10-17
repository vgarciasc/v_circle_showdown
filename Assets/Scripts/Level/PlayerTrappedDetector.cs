﻿using UnityEngine;
using System.Collections;

public class PlayerTrappedDetector : MonoBehaviour {
    Player player;
    public enum Direction { Top, Bottom, Left, Right };
    public Direction myDirection;
    bool colliding;

	void Start () {
        player = this.transform.parent.parent.GetComponent<Player>();
        if (player == null)
            Debug.Log("Player not found.");
    }
	
	void OnCollisionEnter2D(Collision2D target) {
        if (target.gameObject.tag == "Floor") {
            colliding = true;
            StartCoroutine(waitAndSee());
        }
    }

    void OnCollisionExit2D(Collision2D target) {
        if (target.gameObject.tag == "Floor") {
            colliding = false;
        }
    }

    IEnumerator waitAndSee() {
        yield return new WaitForSeconds(1.0f);
        player.corneredDetection((int) myDirection, colliding);
    }
}
