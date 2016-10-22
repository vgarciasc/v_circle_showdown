using UnityEngine;
using System.Collections;

public class SpikeBall : MonoBehaviour {

    Rigidbody2D rb;
    [SerializeField]
    float maxSpeed = 10f;
    [SerializeField]
    BlackHole blackHolePrefab;

	// Use this for initialization
	void Start () {
        rb = this.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector3(5, 5);
	}

    void Update() {
        capSpeed();
    }

    void capSpeed() {
        //float aux_x = rb.velocity.x;
        //float aux_y = rb.velocity.y;

        //if (aux_x > maxSpeed) aux_x = maxSpeed;
        //if (aux_y > maxSpeed) aux_y = maxSpeed;
        //rb.velocity = new Vector2(aux_x, aux_y);

        float aux_x = rb.velocity.x;
        float aux_y = rb.velocity.y;
        if (aux_x > maxSpeed || aux_y > maxSpeed) {
            Instantiate(blackHolePrefab, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame

    void OnTriggerEnter2D(Collider2D coll) {
        GameObject target = coll.gameObject;

        if (target.tag == "Portal")
            target.GetComponent<Portal>().teleport(this.gameObject);
    }
}
