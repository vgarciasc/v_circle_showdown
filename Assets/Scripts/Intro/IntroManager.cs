using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour {
	public Text intro_text;
	public Image intro_image;
	public Image blackscreen;
	
	public List<Sprite> images = new List<Sprite>();

	List<string> text = new List<string>();
	bool player_pressed = false,
		end_text = false,
		text_can_end = false;

	void Start () {
		text.Add("congratulations.");
		text.Add("you have been diagnosed with the Mad Circle disease.");
		text.Add("per the Great Circle King's orders, you will be sent to confinement.");
		text.Add("eliminate your friends and make this easier for us.");

		StartCoroutine(start_text());
	}
	
	void Update () {
		for (int i = 0; i < 4; i++) {
			if (Input.GetButtonDown("Submit_J" + i)) {
				StartCoroutine(next_scene());
			}
			if (Input.GetButtonDown("Fire1_J" + i)) {
				player_pressed = true;
				if (text_can_end) {
					text_can_end = false;
					player_pressed = false;
					end_text = true;
				}
			}
		}
	}

	IEnumerator start_text() {
		Coroutine display;

		for (int i = 0; i < text.Count; i++) {
			display = StartCoroutine(display_text(text[i]));

			intro_image.sprite = images[i];

			yield return new WaitUntil(() => end_text);
			end_text = false;
			if (display != null) {
				StopCoroutine(display);
			}
		}

		StartCoroutine(next_scene());
	}

	IEnumerator display_text(string text) {
		int current_character;
		for (current_character = 0; current_character < text.Length; current_character++) {
			if (player_pressed) {
				player_pressed = false;
				break;
			}
			intro_text.text = text.Substring(0, current_character + 1);
			yield return HushPuppy.WaitForEndOfFrames(2);
		}
		
		intro_text.text = text;
		text_can_end = true;
	}

	IEnumerator next_scene() {
		blackscreen.enabled = true;
		HushPuppy.fadeIn(blackscreen.gameObject, 1f);
		yield return new WaitForSeconds(1.5f);
		SceneManager.LoadScene("TitleScreen");
	}
}
