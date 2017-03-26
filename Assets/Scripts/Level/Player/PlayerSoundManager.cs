using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundManager : MonoBehaviour {

	AudioSource audio;
	Player player;

	[SerializeField]
	AudioClip getItemEffect;
	[SerializeField]
	AudioClip jumpEffect;

	void Start () {
		audio = this.GetComponent<AudioSource>();
		player = this.GetComponent<Player>();
		
		player.get_item_event += (ItemData item) => PlayClip(getItemEffect);
		player.jump_event += () => PlayClip(jumpEffect);
	}

	void PlayClip(AudioClip clip) {
		audio.clip = clip;
		audio.Play();
	}

	void getItem(ItemData item) {
		PlayClip(getItemEffect);
	}

	void jump() {
		PlayClip(jumpEffect);
	}
}
