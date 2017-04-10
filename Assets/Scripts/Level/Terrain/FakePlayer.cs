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
	ParticleSystem particleTrail;
	[SerializeField]
	GameObject chargeIndicator;
	[SerializeField]
	bool canJump = false;

	Transform current_target;
	Rigidbody2D rb;
	PlayerColor palette;

	float chargeBuildup = 0f;

	bool charging = false;

	void Start () {
		rb = GetComponent<Rigidbody2D>();
		particleTrail.Stop();

		palette = generate_random_color();
		init_colors();

		// StartCoroutine(try_reach_target());
		StartCoroutine(simulate_player());
	}

	PlayerColor generate_random_color() {
		PlayerDatabase pdatabase = PlayerDatabase.getPlayerDatabase();
		List<PlayerColor> possible_colors = new List<PlayerColor>();
		possible_colors.AddRange(pdatabase.original_colors_pool);

		PlayerColor random;

		random = possible_colors[Random.Range(0, possible_colors.Count)];

		return random;
	}

	void Update() {
		if (charging) {
			chargeBuildup = Mathf.Clamp(chargeBuildup + 1, 0f, 100f);
		}

		manage_charge();
	}

	void init_colors() {
		background.color = palette.color;
		chargeIndicator.GetComponent<SpriteRenderer>().color = new Color(palette.color.r - 0.4f,
			palette.color.g - 0.4f,
			palette.color.b - 0.4f,
			0.5f);

        var aux = particleTrail.main;
		aux.startColor = palette.gradient;

		particleTrail.Play();
	}

	void manage_charge() {
        float perc = chargeBuildup / data.maxChargeBuildup;
        perc /= 1f;

		float multiplier = Mathf.Sin(perc * Mathf.PI / 2);

        chargeIndicator.transform.localScale = new Vector3(multiplier, multiplier, multiplier);
        Color aux = chargeIndicator.GetComponent<SpriteRenderer>().color;
        chargeIndicator.GetComponent<SpriteRenderer>().color = new Color(aux.r, aux.g, aux.b, multiplier);

        rb.mass = data.mass + data.chargeWeight * perc;
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

	IEnumerator simulate_player() {
		while (true) {
			int movement = Random.Range(0, 4);

			switch (movement) {
				case 0:
					jump();
					break;
				case 1:
					move(Random.Range(-50f, 50f));
					break;
				case 2:
					start_charge();
					break;
				default:
					break;
			}

			int charge = Random.Range(0, 10);

			switch (charge) {
				case 0:
					end_charge();
					break;
				default:
					break;
			}

			yield return new WaitForSeconds(Random.Range(0.15f, 0.3f));
		}
	}

	void jump() {
        rb.AddForce(new Vector2(0, data.jumpForce));
    }

	void move(float movement) {
        rb.velocity += new Vector2(movement / 5f, 0);
    }

	void start_charge() {
		charging = true;
	}

	void end_charge() {
		charging = false;
		
        float perc = chargeBuildup / data.maxChargeBuildup;
        Vector2 direction = this.transform.up * data.chargeForce * perc;
        rb.velocity += direction;
        chargeBuildup = 0f;
	}
	#endregion
}
