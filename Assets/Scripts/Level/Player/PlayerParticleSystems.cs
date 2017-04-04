using UnityEngine;
using System.Collections;

public class PlayerParticleSystems : MonoBehaviour {
    [SerializeField]
    ParticleSystem explosion;
    [SerializeField]
    ParticleSystem heal;
	[SerializeField]
    ParticleSystem full_charge;
	[SerializeField]
    ParticleSystem item_get;
	[SerializeField]
	ParticleSystem mushroom_item_using_trail;
	[SerializeField]
	ParticleSystem coffee_item_using_trail;
	[SerializeField]
	ParticleSystem player_particle_trail;

	Player player;
	Animator animator;
	PlayerItemUser item_user;
	TrailRenderer tr;

	public float trail_length_modifier = 1f;

	bool heal_trail_renderer_bug = false;
	Coroutine trail_coroutine;

	public void Start() {
		player = (Player) HushPuppy.safeComponent(this.gameObject, "Player");
		animator = (Animator) HushPuppy.safeComponent(this.gameObject, "Animator");
		item_user = (PlayerItemUser) HushPuppy.safeComponent(this.gameObject, "PlayerItemUser");
		tr = (TrailRenderer) HushPuppy.safeComponent(this.gameObject, "TrailRenderer");

		//EVENTS
		player.death_event += death;
		player.charge_event += chargeParticles;
		player.visible_event += toggle_visible;
		player.get_item_event += get_item_effect;
		
		item_user.healStart += heal_explosion_start;
		item_user.healEnd += heal_explosion_end;
		
		item_user.mushroom += play_mushroom_trail;
		
		item_user.coffee_start += start_coffee_trail;
		item_user.coffee_start += end_particle_trail;
		item_user.coffee_end += start_particle_trail;
		item_user.coffee_end += end_coffee_trail;

		item_user.triangle_start += end_particle_trail;
		item_user.triangle_end += start_particle_trail;
		//\EVENTS

		explosion.startColor = player.palette.color + new Color(0.3f, 0.3f, 0.3f);
		full_charge.startColor = player.palette.color - new Color(0.15f, 0.15f, 0.15f);
		full_charge.startColor = HushPuppy.getColorWithOpacity(full_charge.startColor, 0.3f);

		tr.sortingLayerName = "Default";
		tr.sortingOrder = 1;

		var aux = mushroom_item_using_trail.main;
		aux.startColor = new Color(player.palette.color.r + 0.2f,
			player.palette.color.g + 0.2f,
			player.palette.color.b + 0.2f,
			0.6f);

		init_trail_renderer_color(player.palette.color);
		StartCoroutine(init_particle_trail_color(player.palette.color));
		tr.enabled = false;
	}
	
	void FixedUpdate() {
		float aux = Mathf.Abs(trail_length_modifier * player.GetComponent<Rigidbody2D>().velocity.magnitude) / 200;
		tr.time = Mathf.Pow(aux, 2);
		set_trail_renderer_width();
		if (heal_trail_renderer_bug && aux > 0.125f) {
			tr.enabled = true;
			heal_trail_renderer_bug = false;
		}
	}

	void death() {
        explosion.gameObject.SetActive(true);
	}

	void heal_explosion_start() {
		tr.enabled = false;
        heal.gameObject.SetActive(true);
	}

	void heal_explosion_end() { StartCoroutine(heal_explosion_end_()); }
	IEnumerator heal_explosion_end_() {
        heal.Stop();

        yield return PauseManager.getPauseManager().WaitForSecondsInterruptable(heal.startLifetime);
        heal.gameObject.SetActive(false);
		heal_trail_renderer_bug = true;
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

	public void init_trail_renderer_color(Color target) {
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
		if (heal_trail_renderer_bug) return;
		
		tr.startWidth = player.transform.localScale.x - 0.2f;
		tr.endWidth = player.transform.localScale.x - 0.6f;
	}

	//save last state of trail renderer
	void toggle_visible(bool value) {
		tr.enabled = value;
	}

	void get_item_effect(ItemData item) {
		item_get.Play();
	}

	void play_mushroom_trail() {
		mushroom_item_using_trail.Play();
	}

	void start_coffee_trail() {
		coffee_item_using_trail.Play();
	}
	
	void end_coffee_trail() {
		coffee_item_using_trail.Stop();
	}

	bool particle_trail_active = true;

	IEnumerator init_particle_trail_color(Color color) {
        var aux2 = player_particle_trail.main;
		aux2.startColor = player.palette.gradient;

		end_particle_trail();

		//just in the start of the level, gambiarra
		yield return new WaitForSeconds(2.0f);

		start_particle_trail();
	}

	void start_particle_trail() {
		particle_trail_active = true;
		player_particle_trail.Play();
	}
	
	void end_particle_trail() {
		particle_trail_active = false;
		player_particle_trail.Stop();
	}
}
