using UnityEngine;
using System.Collections;

public class SmashDetection : MonoBehaviour {
    ISmashable toSmash;
    [SerializeField]
    bool colliding;

    void Start() {
        Transform target = this.transform;

        while (toSmash == null && target != transform.root) {
            toSmash = target.parent.GetComponentInChildren<ISmashable>();
            target = target.parent;
        }

        if (toSmash == null) {
            Debug.Log("GameObject '" + this.gameObject.name + "' has not found a parent with ISmashable interface.");
            Debug.Break();
        }
    }

    void OnCollisionEnter2D(Collision2D target) {
        if (target.gameObject.layer == LayerMask.NameToLayer("CommonTerrain")) {
            colliding = true;
            StartCoroutine(waitAndSee());
        }
    }

    void OnCollisionExit2D(Collision2D target) {
        if (target.gameObject.layer == LayerMask.NameToLayer("CommonTerrain")) {
            colliding = false;
        }
    }

    IEnumerator waitAndSee() {
        for (int i = 0; i < 10; i++)
            yield return new WaitForFixedUpdate();
        if (colliding) {
            toSmash.smashedDetected();
        }
    }
}
