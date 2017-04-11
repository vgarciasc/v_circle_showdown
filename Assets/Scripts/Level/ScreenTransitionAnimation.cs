using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScreenTransitionAnimation : MonoBehaviour {
	Transform container;

	public bool transition_show_ended = false;
	public bool transition_unshow_ended = false;

	static Color temporary_color;

	public static ScreenTransitionAnimation getScreenTransitionAnimation() {
		return (ScreenTransitionAnimation) HushPuppy.safeFindComponent("ScreenAnimation", "ScreenTransitionAnimation");
	}

	void Start () {
		container = this.transform.GetChild(0);
		container.gameObject.SetActive(true);
		set_color(temporary_color);

		StartCoroutine(unshow());
	}

	void set_color(Color color) {
		if (color == Color.clear) {
			return;
		}

		foreach (Transform t in container) {
			Color aux1 = new Color(t.GetComponent<Image>().color.r,
				t.GetComponent<Image>().color.g,
				t.GetComponent<Image>().color.b);
			Color aux2 = new Color(aux1.r + color.r * 0.15f,
				aux1.g + color.g * 0.15f,
				aux1.b + color.b * 0.15f);
			t.GetComponent<Image>().color = aux2;
		}

		temporary_color = Color.clear;
	}

	public void start_animation(Color palette) {
		set_color(palette);
		temporary_color = palette;
		StartCoroutine(show());
	}	

	IEnumerator show() {
		transition_show_ended = false;
		
		Vector3 original_scale = this.transform.GetChild(0).localScale;

		foreach (Transform t in container) {
			t.DOScale(original_scale, 0.25f);

			yield return HushPuppy.WaitForEndOfFrames(1);
		}

		yield return new WaitForSeconds(0.25f);
		transition_show_ended = true;
	}

	IEnumerator unshow() {
		Time.timeScale = 1f;
		
		transition_unshow_ended = false;

		yield return new WaitForSeconds(0.25f);

		foreach (Transform t in container) {
			t.DOScale(Vector3.zero, 0.8f);
			t.DOLocalRotate(new Vector3(0f, 0f, 180f), 0.8f, RotateMode.LocalAxisAdd);
		}

		yield return new WaitForSeconds(0.8f);
		transition_unshow_ended = true;

		foreach (Transform t in container) {
			t.localScale = Vector3.zero;
		}

		Time.timeScale = 1f;
	}
}
