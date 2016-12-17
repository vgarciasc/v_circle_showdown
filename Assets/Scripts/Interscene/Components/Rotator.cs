using UnityEngine;

public class Rotator : MonoBehaviour {
    public bool clockwise = true,
                x_rotation = false,
                y_rotation = false,
                z_rotation = false;

    [Range(0f, 10f)]
    public float speed = 0.3f;

    int sign;

    void Awake() {
        if (clockwise) sign = 1; else sign = -1;
    }

	void Update () {
        transform.Rotate(new Vector3(speed * sign * System.Convert.ToInt32(x_rotation),
                                     speed * sign * System.Convert.ToInt32(y_rotation),
                                     speed * sign * System.Convert.ToInt32(z_rotation)));
    }
}
