using UnityEngine;
using System.Collections;

public class Dispenser : MonoBehaviour {
    [SerializeField]
    GameObject dispenserPosition;
    [SerializeField]
    GameObject objectDispensed;
    [SerializeField]
    Color lightOn = Color.green;
    [SerializeField]
    Color lightOff = Color.red;
    [SerializeField]
    float speed = 1f;
    [SerializeField]
    float cooldownDuration = 0.5f;

    bool cooldownOn = false;

    void OnCollisionEnter2D(Collision2D target) {
        if (target.gameObject.tag == "Player" && !cooldownOn) {
            dispenseObject(Vector3.Magnitude(target.gameObject.GetComponent<Rigidbody2D>().velocity));
        }
    }
    
    void dispenseObject(float playerSpeed) {
        float sizeModifier = playerSpeed / 5f;
        GameObject dispensed = Instantiate(objectDispensed);
        dispensed.transform.position = dispenserPosition.transform.position;
        dispensed.GetComponent<Rigidbody2D>().AddForce(dispenserPosition.transform.up * speed, ForceMode2D.Impulse);
        dispensed.transform.localScale *= sizeModifier;

        StartCoroutine(cooldown());
    }

    IEnumerator cooldown() {
        cooldownOn = true;
        toggleButton(false);

        yield return new WaitForSeconds(cooldownDuration);

        cooldownOn = false;
        toggleButton(true);
    }

    void toggleButton(bool value) {
        if (value) {
            this.GetComponent<SpriteRenderer>().color = lightOn;
        } else {
            this.GetComponent<SpriteRenderer>().color = lightOff;
        }
    }
}
