using UnityEngine;
using System.Collections;

public class Dispenser : MonoBehaviour {
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
            Player player = (Player) HushPuppy.safeComponent(target.gameObject, "Player");
            if (player.isTriangle) {
                for (int i = 0; i < 50; i++) {
                    dispenseObject(Vector3.Magnitude(target.gameObject.GetComponent<Rigidbody2D>().velocity));
                }
            } else {
                dispenseObject(Vector3.Magnitude(target.gameObject.GetComponent<Rigidbody2D>().velocity));
            }
        }
    }
    
    void dispenseObject(float playerSpeed) {
        Transform dispenser = getRandomDispenserPosition();
        float sizeModifier = playerSpeed / 5f;
        GameObject dispensed = Instantiate(objectDispensed);
        dispensed.transform.position = dispenser.position;
        dispensed.GetComponent<Rigidbody2D>().AddForce(dispenser.up * speed, ForceMode2D.Impulse);
        
        //doesnt make triangles smol
        if (sizeModifier > 1f)
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
        if (value) this.GetComponent<SpriteRenderer>().color = lightOn;
        else this.GetComponent<SpriteRenderer>().color = lightOff;
    }

    Transform getRandomDispenserPosition() {
        return transform.GetChild(Random.Range(0, transform.childCount));
        //return dispenserPosition[0].transform;
    }
}
