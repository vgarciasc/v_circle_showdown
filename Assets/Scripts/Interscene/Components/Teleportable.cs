﻿using UnityEngine;
using System.Collections;

public class Teleportable : MonoBehaviour {
    void OnTriggerStay2D(Collider2D coll) {
        GameObject target = coll.gameObject;

        if (target.tag == "Portal")
            target.GetComponent<Portal>().teleport(this.gameObject);
    }
}