using UnityEngine;
using System.Collections;

public class PlayerSpecialDeath : MonoBehaviour {
	Player player;

	[SerializeField]
	GameObject mini_triangle;
	[SerializeField]
	GameObject rainbowExplosionFragment;
	[SerializeField]
	GameObject rainbowExplosionMain;

	bool exploded;

	void Start () {
		player = (Player) HushPuppy.safeComponent(this.gameObject, "Player");
		
		player.bomb_triangle_event += explode_mini_triangles;
		player.GetComponent<PlayerItemUser>().double_coffee += explode_rainbow;
	}

	void explode_mini_triangles(Vector3 bomb_position) {
		if (exploded) return; else exploded = true;

		Vector3 initial_direction = (bomb_position - player.transform.position).normalized;

		for (int i = 0; i < 20; i++) {
			GameObject aux = (GameObject) Instantiate(mini_triangle);
			aux.transform.localScale = this.transform.localScale / 8f;
			aux.transform.position = this.transform.position;
			aux.GetComponent<SpriteRenderer>().color = player.palette.color;
			aux.GetComponent<Rigidbody2D>().AddForce(initial_direction * 20, ForceMode2D.Impulse);
		}
	}

	void explode_rainbow() {
		GameObject main = Instantiate(rainbowExplosionMain, this.transform.position, Quaternion.identity);
		main.transform.localScale = player.transform.localScale;

		int total = 15;
		float angle = Mathf.Deg2Rad * 360f / total;

		for (int i = 0; i < total; i++) {
			GameObject aux = (GameObject) Instantiate(rainbowExplosionFragment);
			aux.transform.position = this.transform.position;
			aux.GetComponent<SpriteRenderer>().color = player.palette.color;
			Vector3 direction = new Vector3(Mathf.Cos(i * angle),
				Mathf.Sin(i * angle),
				0f);

			// Vector3 direction = new Vector3(1, 1, 0);

			aux.GetComponent<Rigidbody2D>().AddForce(direction * 7, ForceMode2D.Impulse);
		}
	}
}
