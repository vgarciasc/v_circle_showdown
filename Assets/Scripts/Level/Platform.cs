using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour {
    [Header("Serialized Variables")]
    [SerializeField]
    float hSpeed = 5f;
    [SerializeField]
    float vSpeed = 5f;
    [SerializeField]
    float hDistance;
    [SerializeField]
    float vDistance;
    [SerializeField]
    bool hMovementEnabled = false;
    [SerializeField]
    bool startGoingRight = true;
    [SerializeField]
    bool vMovementEnabled = false;
    [SerializeField]
    bool startGoingUp = true;

    Rigidbody2D rb;
    Vector2 originalPos,
        leftRightLimit,
        upDownLimit;

    void Start() {
        /*References*/
        rb = GetComponent<Rigidbody2D>();

        /*Reset values*/
        originalPos = this.transform.localPosition;
        leftRightLimit = new Vector2(originalPos.x - hDistance, originalPos.x + hDistance);
        upDownLimit = new Vector2(originalPos.y - vDistance, originalPos.y + vDistance);
    }

    void Update() {
        if (hMovementEnabled) moveHorizontal();
        if (vMovementEnabled) moveVertical();
    }

    void moveHorizontal() {
        if ((transform.localPosition.x > leftRightLimit.y && Mathf.Sign(hSpeed) == 1) ||
            (transform.localPosition.x < leftRightLimit.x && Mathf.Sign(hSpeed) == -1))
            hSpeed *= -1;

        rb.velocity = new Vector2(hSpeed, rb.velocity.y);
    }

    void moveVertical() {
        if ((transform.localPosition.y > upDownLimit.y && Mathf.Sign(vSpeed) == 1) ||
            (transform.localPosition.y < upDownLimit.x && Mathf.Sign(vSpeed) == -1))
            vSpeed *= -1;

        rb.velocity = new Vector2(rb.velocity.x, vSpeed);
    }
}
