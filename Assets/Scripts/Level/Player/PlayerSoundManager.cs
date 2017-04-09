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
	AudioClip explosion;

	void Start () {
		player = this.GetComponent<Player>();
		itemUser = this.GetComponent<PlayerItemUser>();
		particleSystems = this.GetComponent<PlayerParticleSystems>();
		UI_manager = this.GetComponent<PlayerUIManager>();
		audioPlayer = this.GetComponent<AudioSource>();
	
		player.jump_event += jump_effect;
	}

	void AudioPlay(AudioClip clip) {
		audioPlayer.clip = clip;
		audioPlayer.Play();
	}

	void jump_effect() {
		AudioPlay(jump);
	}
}
