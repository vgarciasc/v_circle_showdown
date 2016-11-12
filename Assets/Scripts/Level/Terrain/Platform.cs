using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Platform : MonoBehaviour {
    [SerializeField]
    [Range(0f, 3f)]
    float speed = 1f;
    [SerializeField]
    List<Transform> transformArray = new List<Transform>();
    [SerializeField]
    bool enableMovement = false;

    List<Vector3> positions = new List<Vector3>();
    int currentPosition = 0;
    Vector3 lastPosition,
            nextPosition;

    void Start() {
        initPositions();
    }

    void LateUpdate() {
        updatePosition();
    }

    void initPositions() {
        if (!enableMovement) return;

        positions.Add(this.transform.position);
        for (int i = 0; i < transformArray.Count; i++)
            positions.Add(transformArray[i].position);

        setNextPosition();
    }

    void setNextPosition() {
        if (!enableMovement) return;

        lastPosition = positions[currentPosition];
        currentPosition = (currentPosition + 1) % positions.Count;
        nextPosition = positions[currentPosition];

        Vector3 aux = Vector3.Normalize(nextPosition - lastPosition);
        this.GetComponent<Rigidbody2D>().velocity = aux * 5 * speed;
    }

    void updatePosition() {
        if (!enableMovement) return;

        if (Vector3.Distance(this.transform.position, nextPosition) < 0.5f)
            setNextPosition();
    }
}
