﻿using UnityEngine;
using System.Collections;

public class SpecialCamera : MonoBehaviour {
    enum Movement { None, Right, Left, Top, Bottom };
    GameObject[] players;
    float originalSize;
    Vector3 originalPos;
    Camera cam;

    [SerializeField]
    GameObject camBorders;

    void Start() {
        originalPos = this.transform.localPosition;
        cam = this.GetComponent<Camera>();
        originalSize = cam.orthographicSize;
        //StartCoroutine(restoreOriginalPosition());
    }

    void Update() {
        //updatePlayerList();
        //updateCameraPosition();
    }

    #region Player Framing
    IEnumerator restoreOriginalPosition() {
        while (true) {
            this.transform.localPosition += (originalPos - this.transform.localPosition) / 20f;
            for (int i = 0; i < 2; i++)
                yield return new WaitForEndOfFrame();
        }
    }

    void updatePlayerList() {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    void updateCameraPosition() {
        for (int i = 0; i < players.Length; i++) {
            Vector3 viewportPos = cam.WorldToViewportPoint(players[i].transform.position);
            if ((viewportPos.x > 1 || viewportPos.x < 0 || viewportPos.y > 1 || viewportPos.y < 0) && players[i].GetComponent<Player>().isInArena()) {
                follow(viewportPos);
            }
        }
    }

    void follow(Vector3 pos) {
        if (pos.x < 0) {
            this.transform.localPosition += new Vector3(-0.2f, 0.0f);
        } else if (pos.x > 1) {
            this.transform.localPosition += new Vector3(0.2f, 0.0f);
        } else if (pos.y < 0) {
            this.transform.localPosition += new Vector3(0.0f, -0.2f);
        } else if (pos.y > 1) {
            this.transform.localPosition += new Vector3(0.0f, 0.2f);
        } 
    }
    #endregion

    #region Screen Shake
    public void screenShake_(float power) { StartCoroutine(screenShake(power)); }
    IEnumerator screenShake(float power) {
        Vector3 originalPos = this.transform.localPosition;
        for (int i = 0; i < 10; i++) {
            yield return new WaitForEndOfFrame();
            this.transform.localPosition = new Vector3(originalPos.x + Random.Range(-power / 2, power / 2),
                                                       originalPos.y + Random.Range(-power / 2, power / 2),
                                                       originalPos.z);
        }

        this.transform.localPosition = originalPos;
    }
    #endregion
}
