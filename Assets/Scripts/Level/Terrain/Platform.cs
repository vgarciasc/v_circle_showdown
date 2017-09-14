using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class Platform : MonoBehaviour {
    [Header("> Movement")]
    [SerializeField]
    [Range(0f, 6f)]
    float speed = 1f;
    [SerializeField]
    List<Transform> transformArray = new List<Transform>();
    [SerializeField]
    bool enableMovement = false;
    [SerializeField]
    [Range(-5f, 5f)]
    float acceleration = 0f;
    [SerializeField]
    [Range(0, 50f)]
    float maxVelocity = 50f;
    [SerializeField]
    int maxIterations = 1;
    [SerializeField]
    bool repeat = true;

    List<Vector3> positions = new List<Vector3>();
    int currentPosition = 0,
        iterations = 0;
    Vector3 lastPosition,
            nextPosition;

    [Header("> Before Movement")]
    [SerializeField]
    [Range(0f, 3f)]
    float startDelay = 0f;
    [SerializeField]
    bool enableRumble = false;

    SpriteRenderer sr;

    bool move = false,
        waiting = false;

    void Start() {
        sr = this.GetComponent<SpriteRenderer>();

        StartCoroutine(beforeMoving());
        // if (gameObject.tag == "Spikes") {
        //     StartCoroutine(pulse());
        // }
    }

    IEnumerator pulse() {
        Color originalColor = sr.color;
        int sign = 1;

        while (true) {
            sign *= -1;

            sr.DOColor(
                sr.color + sign * new Color(0.2f, 0.2f, 0.2f, 0f),
                0.5f
            );

            yield return new WaitForSeconds(0.5f);
            yield return new WaitForSeconds(0.2f);
        }
    }

    void LateUpdate() {
        if (move) updatePosition();
    }

    IEnumerator beforeMoving() {
        waiting = true;
        if (enableRumble) StartCoroutine(rumble());
        yield return wait(startDelay);
        waiting = false;

        move = true;
        initPositions();
    }

    IEnumerator wait(float duration) {
        yield return PauseManager.getPauseManager().WaitForSecondsInterruptable(duration);
    }

    IEnumerator rumble() {
        Vector3 originalPos = this.transform.position;
        float rumbleIntensity = 0.1f;

        while (waiting) {
            Vector3 aux = new Vector3(originalPos.x + Random.Range(-rumbleIntensity, rumbleIntensity),
                                      originalPos.y + Random.Range(-rumbleIntensity, rumbleIntensity));
            this.transform.position = aux;
            for (int i = 0; i < 5; i++)
                yield return new WaitForEndOfFrame();
        }

        this.transform.position = originalPos;
    }

    void initPositions() {
        check_last_iteration();
        if (!enableMovement) return;

        positions.Add(this.transform.position);
        for (int i = 0; i < transformArray.Count; i++)
            positions.Add(transformArray[i].position);

        setNextPosition();
    }

    void setNextPosition() {
        check_last_iteration();
        if (!enableMovement) return;
        iterations++;

        lastPosition = positions[currentPosition];
        currentPosition = (currentPosition + 1) % positions.Count;
        nextPosition = positions[currentPosition];

        Vector3 aux = Vector3.Normalize(nextPosition - lastPosition);
        float modifier = Mathf.Clamp(5 * speed * Mathf.Pow(1 + acceleration / 25, iterations), -maxVelocity, maxVelocity);

        this.GetComponentInChildren<Rigidbody2D>().velocity = aux * modifier;
    }

    void updatePosition() {
        check_last_iteration();
        if (!enableMovement) return;

        if (Vector3.Distance(this.transform.position, nextPosition) < 0.5f)
            setNextPosition();
    }

    void check_last_iteration() {
        if (!repeat && (iterations > maxIterations)) {
            enableMovement = false; 
            this.GetComponentInChildren<Rigidbody2D>().velocity = Vector3.zero;
        }
    }
}
