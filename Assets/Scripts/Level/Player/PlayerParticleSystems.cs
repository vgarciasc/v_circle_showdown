using UnityEngine;
using System.Collections;

public class PlayerParticleSystems : MonoBehaviour {
    [SerializeField]
    ParticleSystem explosion;

	Player player;
	Animator animator;
	PlayerItemUser item_user;

	void Start() {
		player = (Player) HushPuppy.safeComponent(this.gameObject, "Player");
		animator = (Animator) HushPuppy.safeComponent(this.gameObject, "Animator");
		item_user = (PlayerItemUser) HushPuppy.safeComponent(this.gameObject, "PlayerItemUser");

		player.death_event += death;
		
		explosion.startColor = player.color + new Color(0.3f, 0.3f, 0.3f);
	}
	
	void death() {
        float scale = this.transform.localScale.x * 0.15f + 0.15f;
        explosion.gameObject.transform.localScale = new Vector3(scale, scale, scale);
        explosion.gameObject.SetActive(true);
	}
}
