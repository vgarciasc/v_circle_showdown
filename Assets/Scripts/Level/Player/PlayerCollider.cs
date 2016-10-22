using UnityEngine;
using System.Collections;

public class PlayerCollider : MonoBehaviour {
    Player player;
    bool started = false;

    void Start() {
        player = this.transform.parent.GetComponentInParent<Player>();
        if (player == null)
            Debug.Log("A PlayerCollider has not found his respective Player.");
        started = true;
    }

	void OnTriggerEnter2D(Collider2D target) {
        if (!started) return;
        player.signalTriggerEnter(target);
    }

    void OnTriggerExit2D(Collider2D target) {
        if (!started) return;
        player.signalTriggerExit(target);
    }

    void OnColliderEnter2D(Collision2D target) {
        if (!started) return;
        player.signalColliderEnter(target);
    }
}
