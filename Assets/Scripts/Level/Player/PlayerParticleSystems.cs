using UnityEngine;
using System.Collections;

public class PlayerParticleSystems : MonoBehaviour {
    [SerializeField]
    ParticleSystem explosion;
    [SerializeField]
    ParticleSystem heal;
	[SerializeField]
    ParticleSystem full_charge;

	Player player;
	Animator animator;
	PlayerItemUser item_user;
	TrailRenderer tr;

	public float trail_length_modifier = 1f;
	bool heal_tr_bug = false;

	void Start() {
		player = (Player) HushPuppy.safeComponent(this.gameObject, "Player");
		animator = (Animator) HushPuppy.safeComponent(this.gameObject, "Animator");
		item_user = (PlayerItemUser) HushPuppy.safeComponent(this.gameObject, "PlayerItemUser");
		tr = (TrailRenderer) HushPuppy.safeComponent(this.gameObject, "TrailRenderer");

		player.death_event += death;
		player.charge_event += chargeParticles;
		item_user.healStart += heal_explosion_start;
		item_user.healEnd += heal_explosion_end;
		
		explosion.startColor = player.color + new Color(0.3f, 0.3f, 0.3f);
		full_charge.startColor = player.color - new Color(0.15f, 0.15f, 0.15f);
		full_charge.startColor = HushPuppy.getColorWithOpacity(full_charge.startColor, 0.3f);
		init_trail_renderer_color(player.color);
	}
	
	float max;
	float min;

	void Update() {
		if (heal_tr_bug) {
			tr.enabled = true;
			heal_tr_bug = false;
		}
		set_trail_renderer_width();
		float aux = Mathf.Abs(trail_length_modifier * player.GetComponent<Rigidbody2D>().velocity.magnitude) / 200;
		tr.time = Mathf.Pow(aux, 2);
		// tr.startColor = HushPuppy.getColorWithOpacity(player.color,
		// 											player.velocityHitMagnitude());
	}

	void death() {
        explosion.gameObject.SetActive(true);
	}

	void heal_explosion_start() {
        heal.gameObject.SetActive(true);
	}

	void heal_explosion_end() { StartCoroutine(heal_explosion_end_()); }
	IEnumerator heal_explosion_end_() {
        heal.Stop();

        yield return PauseManager.getPauseManager().WaitForSecondsInterruptable(heal.startLifetime);
        heal.gameObject.SetActive(false);

		//bug da unity?
		tr.enabled = false;
		heal_tr_bug = true;
	}

	#region charge
	void chargeParticles(int perc) {
		if (perc > 70) {
			full_charge.emissionRate = -1/3f * Mathf.Pow(perc, 2) + (175/3f) * perc - 2450;
		}
		else {
			full_charge.emissionRate = 0;
		}
	}
	#endregion

	void init_trail_renderer_color(Color target) {
		Gradient g;
		GradientColorKey[] gck;
		GradientAlphaKey[] gak;
		g = new Gradient();
		
		gck = new GradientColorKey[2];
		gck[0].color = target;
		gck[0].time = 0f;
		gck[1].color = target;
		gck[0].time = 1f;

		gak = new GradientAlphaKey[2];
		gak[0].alpha = 0.5f;
		gak[0].time = 0f;
		gak[1].alpha = 0f;
		gak[1].time = 1f;

		g.SetKeys(gck, gak);
		tr.colorGradient = g;
	}

	void set_trail_renderer_width() {
		tr.startWidth = player.transform.localScale.x - 0.2f;
		tr.endWidth = player.transform.localScale.x - 0.6f;
	}
}
