using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour {

	Player player;
	PlayerItemUser itemUser;
	PlayerParticleSystems particleSystems;
	PlayerUIManager UI_manager;

	AudioSource audioPlayer;

	[SerializeField]
	AudioClip jump;
	[SerializeField]
	AudioClip charge;
	[SerializeField]
	AudioClip item_get;
	[SerializeField]
	AudioClip item_use;
	[SerializeField]
	AudioClip explosion;
	[SerializeField]
	AudioClip porrada;
	[SerializeField]
	AudioClip victory_get;

	void Start () {
		player = this.GetComponent<Player>();
		itemUser = this.GetComponent<PlayerItemUser>();
		particleSystems = this.GetComponent<PlayerParticleSystems>();
		UI_manager = this.GetComponent<PlayerUIManager>();
		audioPlayer = this.GetComponent<AudioSource>();
	
		// player.jump_event += jump_effect;
		player.death_event += death_effect;
		player.release_charge_event += charge_effect;
		player.get_item_event += get_item_effect;
		player.use_item_event += use_item_effect;
		player.player_hit_event += give_hit_effect;
	}

	void AudioPlay(AudioClip clip) {
		audioPlayer.PlayOneShot(clip);
	}

	void jump_effect() {
		AudioPlay(jump);
	}

	void death_effect() {
		audioPlayer.PlayOneShot(explosion, 0.7f);
	}

	void get_item_effect(ItemData data) {
		AudioPlay(item_get);
	}

	void use_item_effect(ItemData data) {
		AudioPlay(item_use);
	}

	void charge_effect(int buildup) {
		audioPlayer.PlayOneShot(charge, (buildup / 300f));
	}

	void give_hit_effect(float hit_magnitude) {
		audioPlayer.PlayOneShot(porrada, (hit_magnitude / 2.5f));
	}

	public void victory_get_effect() {
		audioPlayer.PlayOneShot(victory_get, 0.4f);
	}
}
