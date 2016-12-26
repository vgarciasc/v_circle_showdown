using UnityEngine;
using System.Collections;

public class PlayerParticleSystems : MonoBehaviour {
    [SerializeField]
    ParticleSystem explosion;

	Player player;
	Animator animator;
	PlayerItemUser item_user;
	TrailRenderer tr;

	void Start() {
		player = (Player) HushPuppy.safeComponent(this.gameObject, "Player");
		animator = (Animator) HushPuppy.safeComponent(this.gameObject, "Animator");
		item_user = (PlayerItemUser) HushPuppy.safeComponent(this.gameObject, "PlayerItemUser");
		tr = (TrailRenderer) HushPuppy.safeComponent(this.gameObject, "TrailRenderer");

		player.death_event += death;
		
		explosion.startColor = player.color + new Color(0.3f, 0.3f, 0.3f);
		init_trail_renderer();
	}
	
	float max;
	float min;

	void Update() {
		tr.startWidth = player.transform.localScale.x - 0.2f;
		tr.endWidth = player.transform.localScale.x - 0.6f;
		float aux = Mathf.Abs(player.GetComponent<Rigidbody2D>().velocity.magnitude) / 200;

		tr.time = Mathf.Pow(aux, 2);
		// tr.startColor = HushPuppy.getColorWithOpacity(player.color,
		// 											player.velocityHitMagnitude());
	}

	void death() {
        float scale = this.transform.localScale.x * 0.15f + 0.15f;
        explosion.gameObject.transform.localScale = new Vector3(scale, scale, scale);
        explosion.gameObject.SetActive(true);
	}

	void init_trail_renderer() {
		Gradient g;
		GradientColorKey[] gck;
		GradientAlphaKey[] gak;
		g = new Gradient();
		
		gck = new GradientColorKey[2];
		gck[0].color = player.color;
		gck[0].time = 0f;
		gck[1].color = player.color;
		gck[0].time = 1f;

		gak = new GradientAlphaKey[2];
		gak[0].alpha = 0.5f;
		gak[0].time = 0f;
		gak[1].alpha = 0f;
		gak[1].time = 1f;

		g.SetKeys(gck, gak);
		tr.colorGradient = g;
	}
}
