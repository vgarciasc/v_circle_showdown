using UnityEngine;
using System.Collections;

public class PongPaddle : MonoBehaviour {
    [SerializeField]
    Transform spikeball;

    [SerializeField]
    Transform upperTransform;
    [SerializeField]
    Transform lowerTransform;
    [SerializeField]
    PongPaddle brotherPaddle;
    [SerializeField]
    Color spikeColor;
    [SerializeField]
    Color floorColor;

    bool spikesOn;

    float originalPosY,
        nextPosY,
        currentPosY,
        speed = 5f,
        upperExtreme,
        lowerExtreme;

    void Start() {
        upperExtreme = upperTransform.position.y;
        lowerExtreme = lowerTransform.position.y;
        originalPosY = nextPosY = currentPosY = this.transform.position.y;
        switchSpikes(false);
    }

    void FixedUpdate() {
        changePosition();
        checkPosition();
	}

    void setVelocity() {
        Vector3 aux = this.GetComponent<Rigidbody2D>().velocity;
        this.GetComponent<Rigidbody2D>().velocity = new Vector2(aux.x, Mathf.Sign(nextPosY - originalPosY) * speed);
    }

    void checkPosition() {
        if (Mathf.Abs(currentPosY - nextPosY) < 0.02f) {
            currentPosY = nextPosY;
        } else if ((this.transform.position.y > upperExtreme) || (this.transform.position.y < lowerExtreme)) {
            nextPosY = originalPosY;
        }
    }

    void changePosition() {
        float threshold = 10f;

        if (spikeball == null || spikeball.position.y > upperExtreme || spikeball.position.y < lowerExtreme) return;

        if ((Mathf.Abs(spikeball.position.x - this.transform.position.x) < threshold)) { //close enough
            currentPosY = this.transform.position.y;
            nextPosY = spikeball.position.y;
            setVelocity();
        }
    }

    public void switchSpikes(bool value) {
        if (value) {
            spikesOn = true;
            this.tag = "Spikes";
            this.GetComponent<SpriteRenderer>().color = spikeColor;
        } else {
            spikesOn = false;
            this.tag = "Floor";
            this.GetComponent<SpriteRenderer>().color = floorColor;
        }
    }

    void OnCollisionEnter2D(Collision2D coll) {
        GameObject target = coll.gameObject;
        if (target.transform == spikeball) {
            speed += 1.4f;
            if (!spikesOn) switchSpikes(true);
            if (brotherPaddle.spikesOn) brotherPaddle.switchSpikes(false);
        }
    }
}
