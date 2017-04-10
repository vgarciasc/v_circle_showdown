using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class IntroManager : MonoBehaviour {
	public TextMeshProUGUI intro_text;
	public TextMeshProUGUI skip_text;
	public Image intro_image;
	public Image blackscreen;
	public Image whitescreen;
	public Animator animator;
	public AudioSource audio_text;
	
	public List<Sprite> images = new List<Sprite>();

	List<string> text = new List<string>();
	bool player_pressed = false,
		end_text = false,
		text_can_end = false,
		skip_pressed_one_time = false;

	void Start () {
		text.Add("congratulations.");
		text.Add("you have been diagnosed with the Mad Circle disease.");
		text.Add("as the Circle King commanded, you have been sentenced to confinement.");
		text.Add("eliminate your friends and make this easier for us.");

		StartCoroutine(start_text());
		skip_text.text = "press start to skip";
	}
	
	void Update () {
		for (int i = 0; i < 4; i++) {
			if (Input.GetButtonDown("Submit_J" + i)) {
				if (!skip_pressed_one_time) {
					skip_pressed_one_time = true;
					skip_text.text = "press start again to confirm";
				}
				else {
					StartCoroutine(next_scene());
				}
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
		// blackscreen.enabled = true;
		// yield return new WaitForSeconds(0.5f);
		// HushPuppy.fadeImgOut(blackscreen.gameObject, 1f);
		// yield return new WaitForSeconds(1.5f);
		
		animator.SetTrigger("enter");
		yield return new WaitForSeconds(1f);

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
			audio_text.Play();
			if (player_pressed) {
				player_pressed = false;
				break;
			}
			intro_text.text = text.Substring(0, current_character) + "<color=#0000>" + text.Substring(current_character) + "</color>";
			yield return HushPuppy.WaitForEndOfFrames(2);
		}
		
		intro_text.text = text;
		text_can_end = true;
	}

	IEnumerator next_scene() {
		// whitescreen.enabled = true;
		// HushPuppy.fadeIn(whitescreen.gameObject, 1f);
		// yield return new WaitForSeconds(1.5f);
		// SceneManager.LoadScene("TitleScreen");

		animator.SetTrigger("exit");
		yield return new WaitForSeconds(1.5f);
		SceneManager.LoadScene("TitleScreen 2");
	}
}
