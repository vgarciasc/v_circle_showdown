using UnityEngine;
using System.Collections;

public class PongPaddle : MonoBehaviour {
    [SerializeField]
    Transform spikeball;

    [SerializeField]
    Transform upperTransform;
    [SerializeField]
    Transform lowerTransform;

    float nextPosition_y, lastPosition_y,
        speed = 1f,
        upperExtreme,
        lowerExtreme;

    void Start() {
        upperExtreme = upperTransform.position.y;
        lowerExtreme = lowerTransform.position.y;
        nextPosition_y = lastPosition_y = this.transform.position.y;
    }

    void FixedUpdate() {
        this.transform.position += new Vector3(0f,
                                             (nextPosition_y - lastPosition_y) * Time.deltaTime * speed);
        changePosition();
	}

    void changePosition() {
        float threshold = 10f;
        bool ballInCourt = false;

        if (spikeball == null || spikeball.position.y > upperExtreme || spikeball.position.y < lowerExtreme) return;

        if ((Mathf.Abs(spikeball.position.x - this.transform.position.x) < threshold)) { //close enough
            if (!ballInCourt) {
                ballInCourt = true;
                lastPosition_y = this.transform.position.y;
            }
            nextPosition_y = spikeball.position.y;
        } else {
            ballInCourt = false;
        }
    }

    void OnCollisionEnter2D(Collision2D coll) {
        GameObject target = coll.gameObject;
        if (target.tag == "Spikes") {
            speed += 1.4f;
        }
    }
}
