using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

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
    [SerializeField]
    bool randomRotation = false;
    [SerializeField]
    bool trianglePlayerMultipliesOutput = false;

    bool cooldownOn = false;
    bool pressed = false;
    bool playerPressing = false;

    void OnTriggerEnter2D(Collider2D target) {
        if (target.gameObject.tag == "Player" && !pressed) {
            Player player = (Player) HushPuppy.safeComponent(target.gameObject, "Player");
            if (player.isTriangle && trianglePlayerMultipliesOutput) {
                for (int i = 0; i < 50; i++) {
                    dispenseObject(Vector3.Magnitude(target.gameObject.GetComponent<Rigidbody2D>().velocity));
                }
            } else {
                dispenseObject(Vector3.Magnitude(target.gameObject.GetComponent<Rigidbody2D>().velocity));
            }
        }

        if (target.gameObject.tag == "Bomb" && !pressed) {
            dispenseObject(Vector3.Magnitude(target.gameObject.GetComponent<Rigidbody2D>().velocity));
        }        
    }

    void Update() {
        if (playerIsPressing.ContainsKey(0)) {
            bool aux = playerIsPressing[0];
            foreach (KeyValuePair<int, bool> key in playerIsPressing) {
                aux = aux || key.Value;
            }
            playerPressing = aux;
        }

        if (pressed && !cooldownOn && !playerPressing) {
            toggleButton(true);
        }
    }

    Dictionary<int, bool> playerIsPressing = new Dictionary<int, bool>();

    void Start() {
        foreach (PlayerInstance player in PlayerDatabase.getPlayerDatabase().players) {
            playerIsPressing.Add(player.playerID, false);
        }
    }

    void OnTriggerStay2D(Collider2D target) {
        if (target.gameObject.tag == "Player") {
            playerIsPressing[target.gameObject.GetComponentInChildren<Player>().ID] = true;
        }
    }

    void OnTriggerExit2D(Collider2D target) {
        if (target.gameObject.tag == "Player") {
            playerIsPressing[target.gameObject.GetComponentInChildren<Player>().ID] = false;
            if (!cooldownOn) {
                toggleButton(true);
            }
        }

        if (target.gameObject.tag == "Bomb") {
            if (!cooldownOn) {
                toggleButton(true);
            }
        }
    }

    // void OnCollisionStay2D(Collision2D target) {
    //     if (target.gameObject.tag == "Player" && !cooldownOn) {
    //         toggleButton(false);
    //     }
    // }

    IEnumerator levantarBotao() {
        yield return new WaitUntil(() => !cooldownOn);

        toggleButton(true);
    }
    
    void dispenseObject(float playerSpeed) {
        StartCoroutine(cooldown(playerSpeed));

        Transform dispenser = getRandomDispenserPosition();
        float sizeModifier = playerSpeed / 5f;
        GameObject dispensed = Instantiate(objectDispensed);
        dispensed.transform.position = dispenser.position;

        if (randomRotation) {
            dispensed.transform.Rotate(new Vector3(0f, 0f, Random.Range(0f, 360f)));
        }        
        
        // dispensed.GetComponent<Rigidbody2D>().AddForce(dispenser.up * speed, ForceMode2D.Impulse);
        
        //doesnt make triangles smol
        if (sizeModifier > 1f)
            dispensed.transform.localScale *= sizeModifier;

        growDispensedObject(dispensed.transform);
    }

    void growDispensedObject(Transform dispensed) {
        Vector3 originalScale = dispensed.transform.localScale;
        dispensed.transform.localScale = Vector3.zero;
        
        dispensed.transform.DOScale(originalScale, 0.5f);
    }

    IEnumerator cooldown(float hitStrength) {
        cooldownOn = true;
        toggleButton(false);

        yield return PauseManager.getPauseManager().WaitForSecondsInterruptable(0.15f);
        yield return PauseManager.getPauseManager().WaitForSecondsInterruptable(hitStrength / 20f);

        cooldownOn = false;
    }

    void toggleButton(bool value) {
        if (value && !playerPressing) {
            this.GetComponent<Animator>().SetBool("active", false);
            this.GetComponent<SpriteRenderer>().color = lightOn;
            pressed = !value;            
        }
        else if (!value && !playerPressing) {
            this.GetComponent<Animator>().SetBool("active", true);
            this.GetComponent<SpriteRenderer>().color = lightOff;
            pressed = !value;            
        }
    }

    Transform getRandomDispenserPosition() {
        return transform.GetChild(Random.Range(0, transform.childCount));
        //return dispenserPosition[0].transform;
    }
}
