using UnityEngine;
using System.Collections;

public class RotatingPlatform : MonoBehaviour {
    Color originalColor;
    Rigidbody2D rb;
    bool hasItem;

    [SerializeField]
    Color itemActivatedColor;
    [SerializeField]
    Transform itemPosition;

    //TODO: make be affected by black hole gravitational pulse

	void Start () {
        originalColor = this.GetComponent<SpriteRenderer>().color;
        rb = this.GetComponent<Rigidbody2D>();
        StartCoroutine(checkRotation());
	}

    IEnumerator checkRotation() {
        float threshold = 190f;
        bool aboveThreshold = false;

        while (true) {
            yield return new WaitForSeconds(2.0f);
            if (!hasItem) continue;

            if (Mathf.Abs(rb.angularVelocity) >= threshold) {
                if (!aboveThreshold) aboveThreshold = true;
                else spawnItem();
            } else {
                aboveThreshold = false;
            }
        }
    }

    void spawnItem() {
        switchItem(false);
        ItemSpawner itemSpawner = (ItemSpawner) HushPuppy.safeFindComponent("GameController", "ItemSpawner");
        itemSpawner.spawnItem(itemPosition.position);
    }

    public void switchItem(bool value) {
        hasItem = value;
        if (value) {
            this.GetComponent<SpriteRenderer>().color = itemActivatedColor;
        } else {
            this.GetComponent<SpriteRenderer>().color = originalColor;
        }
    }
}
