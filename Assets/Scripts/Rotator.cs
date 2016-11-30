using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {
    public bool clockwise = true;

    [Range(0f, 10f)]
    public float speed = 0.3f;

    int sign;

    void Awake() {
        if (clockwise) sign = 1; else sign = -1;
    }

	void Update () {
        transform.Rotate(new Vector3(0f, 0f, speed * sign));
    }
}
