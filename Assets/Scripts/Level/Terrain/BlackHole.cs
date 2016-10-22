using UnityEngine;
using System.Collections;

public class BlackHole : MonoBehaviour {
    void Update() {
        attractPlayers();
    }

    void attractPlayers() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < players.Length; i++) {
            Vector3 offset = transform.position - players[i].transform.position;
            offset.z = 0;
            float magsqr = offset.sqrMagnitude;
            if (magsqr > 0.0001f)
                players[i].GetComponent<Rigidbody2D>().AddForce(100f * offset.normalized / magsqr, ForceMode2D.Force);
        }
    }
}
