using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FakePlayer : MonoBehaviour {
	[SerializeField]
	Transform[] targets; //he will try to reach the target
	[SerializeField]
	PlayerData data;
	[SerializeField]
	SpriteRenderer background;
	[SerializeField]
	bool canJump = false;

	Transform current_target;
	Rigidbody2D rb;

	void Start () {
		rb = GetComponent<Rigidbody2D>();

		background.color = generate_random_color();
		StartCoroutine(try_reach_target());
	}

	Color generate_random_color() {
		PlayerDatabase pdatabase = PlayerDatabase.getPlayerDatabase();
		if (pdatabase == null) {
			return Color.grey;
		}

		List<Color> possible_colors = pdatabase.original_color_pool;
		possible_colors.AddRange(new List<Color> {Color.grey, Color.white});
		Color random;

		random = possible_colors[Random.Range(0, possible_colors.Count)];
		// List<Color> colors = new List<Color>() {Color.red, Color.green, Color.magenta,
		// 										Color.cyan, Color.blue, Color.yellow};
												
		// random = colors[Random.Range(0, colors.Count)];
		// float random_r, random_g, random_b;
        // float saturation = Random.Range(0.2f, 0.4f);
        // random_r = random.r += (saturation * -Mathf.Sign(random.r - 0.5f));
        // random_g = random.g += (saturation * -Mathf.Sign(random.g - 0.5f));
        // random_b = random.b += (saturation * -Mathf.Sign(random.b - 0.5f));
		// random = new Color(random_r, random_g, random_b, 1);

		return random;
	}

	#region simulate player movements
	IEnumerator try_reach_target() {
		while (true) {
			yield return new WaitForSeconds(0.5f);
			if (current_target == null) {
				foreach(Transform tar in targets) {
					if (tar != null) {
						current_target = tar;
						break;
					}
				}
			
				continue;
			}

			float movement = current_target.position.x - this.transform.position.x;
			if (this.transform.position.y < current_target.position.y) {
				if (canJump) {
					jump();
				}
			}
			if (this.transform.position.y > current_target.position.y) {
				//do nothing
			}
			if (this.transform.position.x < current_target.position.y - 0.5f ||
				this.transform.position.x > current_target.position.y + 0.5f) {
				move(movement);
			}
		}
	}

	void jump() {
        rb.AddForce(new Vector2(0, data.jumpForce));
    }

	void move(float movement) {
        rb.velocity += new Vector2(movement / 5f, 0);
    }
	#endregion
}
