using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

[System.Serializable]
public class Jukebox : System.Object {
	public List<string> scenes = new List<string>();
	public AudioClip song;
	[RangeAttribute(0f, 1f)]
	public float volume;
	public bool loop = true;
}

public class LevelSoundManager : MonoBehaviour {
	[SerializeField]
	AudioClip victory_get;

	AudioSource audioPlayer;
	static bool isPlaying = false;
	
	AudioClip previousSong = null,
			currentSong = null;

	public List<Jukebox> juke = new List<Jukebox>();

	public static LevelSoundManager getLevelSoundManager() {
		return (LevelSoundManager) HushPuppy.safeFindComponent("SoundManager", "LevelSoundManager");
	}

	void Awake() {
		if (isPlaying) {
			Destroy(this.gameObject);
			return;
		}

		audioPlayer = this.GetComponent<AudioSource>();
		DontDestroyOnLoad(this.gameObject);

		isPlaying = true;
		SceneManager.sceneLoaded += OnLevelLoad;
	}

	void OnLevelLoad(Scene scene, LoadSceneMode mode) {
		previousSong = currentSong;

		foreach (Jukebox jk in juke) {
			foreach (string sc in jk.scenes) {
				if (sc == scene.name) {
					currentSong = jk.song;
					audioPlayer.volume = jk.volume;
					audioPlayer.loop = jk.loop;
					break;
				}			
			}
		}

		if ((previousSong == null) ||
			currentSong.name != previousSong.name) {
			PlayClip(currentSong);
		}
	}

	void PlayClip(AudioClip clip) {
		audioPlayer.clip = clip;
		audioPlayer.Play();
	}

	void PlayOneShot(AudioClip clip) {
		audioPlayer.PlayOneShot (clip);
	}

	void deactivateSoundManager() {
		isPlaying = false;

		Destroy(this.gameObject);
	}

	public void fadeOut(float duration) {
		var tween = DOTween.To(()=> audioPlayer.volume,
			x=> audioPlayer.volume = x,
			0f,
			duration);
		tween.SetEase(Ease.InCirc);
	}

	public void victory_sound() {
		PlayOneShot (victory_get);
	}
}
