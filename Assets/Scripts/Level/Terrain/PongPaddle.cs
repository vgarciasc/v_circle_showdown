using UnityEngine;
using System.Collections;

public class PongPaddle : MonoBehaviour {
    [SerializeField]
    Transform spikeball;

    [SerializeField]
    Transform upperTransform;
    [SerializeField]
    Transform lowerTransform;

    float upperExtreme,
        lowerExtreme;

    void Start() {
        upperExtreme = upperTransform.position.y;
        lowerExtreme = lowerTransform.position.y;
    }

	void Update() {
        changePosition();
	}

    void changePosition() {
        float threshold = 15f;

        if (spikeball == null || spikeball.position.y > upperExtreme || spikeball.position.y < lowerExtreme) return;

        if ((Mathf.Abs(spikeball.position.x - this.transform.position.x) < threshold)) { //close enough
            this.transform.position = new Vector3(this.transform.position.x,
                                                spikeball.position.y,
                                                this.transform.position.z);
        } else {
            this.transform.position += new Vector3(0f, Random.Range(-0.05f, 0.05f), 0f);
        }
    }
}
