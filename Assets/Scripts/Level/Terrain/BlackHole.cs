using UnityEngine;
using System.Collections;

public class BlackHole : MonoBehaviour {
    [SerializeField]
    [Range(50f, 300f)]
    float gravityForce = 100f;

    void Update() {
        this.transform.Rotate(new Vector3(0f, 0f, 1f));
        attractPlayers();
    }

    void attractPlayers() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++) {
            Vector3 offset = transform.position - players[i].transform.position;
            offset.z = 0;
            float magsqr = offset.sqrMagnitude;
            if (magsqr > 0.0001f) {
                players[i].GetComponent<Rigidbody2D>().AddForce(gravityForce * offset.normalized / magsqr, ForceMode2D.Force);
            }
        }
    }
}
