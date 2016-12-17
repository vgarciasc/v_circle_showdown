using UnityEngine;
using System.Collections;

public class NebulaVulnerable : MonoBehaviour {
	[SerializeField]
	float size_rate = 0.005f;
	[SerializeField]
	float max_size = 5f;

    void OnTriggerStay2D(Collider2D target) {
        switch (target.gameObject.tag) {
            case "Nebula":
                changeSize(size_rate);
                break;
            case "Inverse Nebula":
                changeSize(-size_rate);
                break;
        }
    }

	void changeSize(float ratio) {
		if (this.transform.localScale.x > max_size) return;
		this.transform.localScale += new Vector3(ratio, ratio, ratio);
	}
}
