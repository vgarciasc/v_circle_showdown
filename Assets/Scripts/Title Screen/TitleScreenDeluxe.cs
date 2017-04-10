using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class TitleScreenDeluxe : MonoBehaviour {

	Transform camera;
	[SerializeField]
	GameObject prisonPrefab;
	[SerializeField]
	GameObject startingPrison;
	[SerializeField]
	GameObject whiteScreen;
	[SerializeField]
	AudioClip startGameSoundEffect;

	List<GameObject> prisons = new List<GameObject>();

	Vector3 last_checkpoint;
	AudioSource audioPlayer;

	void Start () {
		camera = this.transform;
		last_checkpoint = this.transform.position;
		audioPlayer = this.GetComponent<AudioSource>();

		prisons.Add(startingPrison);
		createNewPrison();
		createNewPrison();

		StartCoroutine(moveCamera());
		// StartCoroutine(managePrisons());
	}

	bool gameStarted = false;

	void Update() {
		if (this.transform.position.y - last_checkpoint.y > 13f) {
			createNewPrison();
			destroyOldestPrison();
			last_checkpoint = this.transform.position;
		}

		for (int i = 0; i < 4; i++) {
			if (Input.GetButtonDown("Submit_J" + i) && !gameStarted) {
				StartCoroutine(startGame());
			}
		}
	}

	IEnumerator moveCamera() {
		while (true) {
			var aux = camera.DOMove(new Vector3(camera.position.x,
				camera.position.y + 3,
				camera.position.z),
				2.5f,
				false);
			aux.SetEase(Ease.Linear);
			yield return new WaitForSeconds(2.5f);
		}
	}

	IEnumerator managePrisons() {
		while (true) {
			yield return new WaitForSeconds(11.0f);
			createNewPrison();
			destroyOldestPrison();
		}
	}

	void createNewPrison() {
		GameObject aux = Instantiate(prisonPrefab);
		aux.transform.position = prisons[prisons.Count - 1].transform.position + new Vector3(0, 12, 0);
		prisons.Add(aux);
	}

	void destroyOldestPrison() {
		GameObject oldest = prisons[0];
		prisons.Remove(prisons[0]);
		Destroy(oldest);
	}

	IEnumerator startGame() {
		gameStarted = true;

		whiteScreen.SetActive(true);
		whiteScreen.GetComponent<Image>().color = HushPuppy.getColorWithOpacity(whiteScreen.GetComponent<Image>().color, 0f);
		var tween = DOTween.To(()=> whiteScreen.GetComponent<Image>().color,
			x=> whiteScreen.GetComponent<Image>().color = x,
			Color.white,
			1f);
		tween.SetEase(Ease.InCirc);
		var tween2 = DOTween.To(()=> audioPlayer.volume,
			x=> audioPlayer.volume = x,
			0f,
			1f);
		tween2.SetEase(Ease.InCirc);

		audioPlayer.PlayOneShot(startGameSoundEffect);

		yield return new WaitForSeconds(1f);

		SceneManager.LoadScene("PlayerSelect");
	}
}
