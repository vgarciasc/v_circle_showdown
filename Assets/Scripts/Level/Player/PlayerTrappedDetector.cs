using UnityEngine;
using System.Collections;

public class PlayerTrappedDetector : MonoBehaviour {
    Player player;
    public enum Direction { Top, Bottom, Left, Right };
    public Direction myDirection;
    bool colliding;

	void Start () {
        player = this.transform.root.GetComponent<Player>();
        if (player == null)
            Debug.Log("Player not found by PlayerTrappedDetector.");
    }
	
	void OnCollisionEnter2D(Collision2D target) {
        if (target.gameObject.layer == LayerMask.NameToLayer("Common Floor")) {
            colliding = true;
            StartCoroutine(waitAndSee());
        }
    }

    void OnCollisionStay2D(Collision2D target) {
        if (target.gameObject.layer == LayerMask.NameToLayer("Common Floor")) {
            colliding = true;
            StartCoroutine(waitAndSee());
        }
    }

    void OnCollisionExit2D(Collision2D target) {
        if (target.gameObject.layer == LayerMask.NameToLayer("Common Floor")) {
            colliding = false;
        }
    }

    IEnumerator waitAndSee() {
        yield return new WaitForSeconds(1.0f);
        player.corneredDetection((int) myDirection, colliding);
    }
}
