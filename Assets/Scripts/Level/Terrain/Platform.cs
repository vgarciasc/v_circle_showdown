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

    void Update() {
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
    }

    void updatePosition() {
        if (!enableMovement) return;

        this.transform.position += (nextPosition - lastPosition) * Time.deltaTime * speed;
        if (Vector3.Distance(this.transform.position, nextPosition) < 0.2f) {
            setNextPosition();
        }
    }
}
