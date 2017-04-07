using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntedGhost : MonoBehaviour {

	bool move_right, move_left, move_up, move_down;

	Rigidbody2D rb;

	void Start () {
		rb = this.GetComponentInChildren<Rigidbody2D>();

		StartCoroutine(move());
	}

	void Update() {
		set_aimbot();

		Vector2 aux = Vector2.zero;

		if (move_right) {
			aux.x = 1;
		}
		if (move_left) {
			aux.x = -1;
		}
		if (move_up) {
			aux.y = 1;
		}
		if (move_down) {
			aux.y = -1;
		}

		rb.velocity = aux * 5;
	}

	IEnumerator move() {
		while (true) {

			int f = Random.Range(0, 20);

			move_right = f % 4 == 0;
			move_left = f % 4 == 1;
			move_up = f % 4 == 2;
			move_down = f % 4 == 3;

			yield return new WaitForSeconds(1f);
		}
	}

	void set_aimbot() {
        int closest_player_index = -1;
        Vector2 closest_player_distance = new Vector2(100, 100);

        GameObject[] players = Player.getAllPlayers();
        if (players.Length <= 1) return;

		for (int i = 0; i < players.Length; i++) {
            Vector2 aux = players[i].transform.position - this.transform.position;
            if (aux.magnitude < closest_player_distance.magnitude &&
                aux.magnitude != 0) { //careful, can target itself
                closest_player_distance = aux;
                closest_player_index = i;
            }
        }

        this.transform.up = closest_player_distance;
	}
}
