using UnityEngine;
using System.Collections;

public class PlayerSpecialDeath : MonoBehaviour {
	Player player;

	[SerializeField]
	GameObject mini_triangle;

	bool exploded;

	void Start () {
		player = (Player) HushPuppy.safeComponent(this.gameObject, "Player");
		player.bomb_triangle_event += explode_mini_triangles;
	}

	void explode_mini_triangles(Vector3 bomb_position) {
		if (exploded) return; else exploded = true;

		Vector3 initial_direction = (bomb_position - player.transform.position).normalized;
		Debug.Log(initial_direction);

		for (int i = 0; i < 5; i++) {
			GameObject aux = (GameObject) Instantiate(mini_triangle);
			aux.transform.localScale = this.transform.localScale / 2f;
			aux.transform.position = this.transform.position;
			aux.GetComponent<SpriteRenderer>().color = player.color;
			aux.GetComponent<Rigidbody2D>().AddForce(initial_direction * 20, ForceMode2D.Impulse);
		}
	}
}
