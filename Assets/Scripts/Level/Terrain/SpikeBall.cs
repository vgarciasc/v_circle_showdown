using UnityEngine;
using System.Collections;

public class SpikeBall : MonoBehaviour {

    Rigidbody2D rb;
    [SerializeField]
    float maxSpeed = 10f;
    [SerializeField]
    BlackHole blackHolePrefab;

    [SerializeField]
    bool enableEventHorizon = false;
    [SerializeField]
    bool fullTransformation = false;

    Vector3 startVelocity = new Vector3(5, 5);

    void Start () {
        rb = this.GetComponent<Rigidbody2D>();
        resetVelocity();
	}

    void Update() {
        capSpeed();
    }

    void capSpeed() {
        float aux_x, aux_y;
        aux_x = rb.velocity.x;
        aux_y = rb.velocity.y;

        if (enableEventHorizon) {
            if (aux_x > maxSpeed || aux_y > maxSpeed) {
                Instantiate(blackHolePrefab, this.transform.position, Quaternion.identity);
                if (fullTransformation) Destroy(this.gameObject);
                else resetVelocity();
            }
        } else {
            if (aux_x > maxSpeed) aux_x = maxSpeed;
            if (aux_y > maxSpeed) aux_y = maxSpeed;
            rb.velocity = new Vector2(aux_x, aux_y);
        }
    }

    void resetVelocity() {
        rb.velocity = startVelocity;
    }
}
