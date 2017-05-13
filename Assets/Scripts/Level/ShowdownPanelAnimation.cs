using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowdownPanelAnimation : MonoBehaviour {

	public static bool shouldPlayAnimation = false;
	public bool playingAnimation = false;

	float original_timescale = 1f;

	public static ShowdownPanelAnimation getShowdownPanelAnimation() {
		return (ShowdownPanelAnimation) HushPuppy.safeFindComponent("ScreenAnimation", "ShowdownPanelAnimation");
	}

	void Awake() {
		if (shouldPlayAnimation) {
			StartCoroutine(init());
		}
		else {
			this.gameObject.SetActive(false);
		}
	}

	IEnumerator init() {
		playingAnimation = true;
		this.GetComponent<Animator>().enabled = false;

		yield return new WaitUntil(() => ScreenTransitionAnimation.getScreenTransitionAnimation().transition_unshow_ended);

		original_timescale = Time.timeScale;
		Time.timeScale = 0.1f;
		Time.fixedDeltaTime = 0.02F * Time.timeScale;
		this.GetComponentInChildren<Animator>().speed = 10f;
		this.GetComponent<Animator>().enabled = true;
	}

	void AnimDestroy() {
		shouldPlayAnimation = false;
		Time.timeScale = original_timescale;
		Time.fixedDeltaTime = 0.02F * Time.timeScale;
		playingAnimation = false;
	}

	public static void EnteringNewLevel() {
		shouldPlayAnimation = true;
	}
}
