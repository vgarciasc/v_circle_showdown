﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowdownPanelAnimation : MonoBehaviour {

	public static bool shouldPlayAnimation = false;

	float original_timescale = 1f;

	void Awake() {
		if (shouldPlayAnimation) {
			StartCoroutine(init());
		}
		else {
			this.gameObject.SetActive(false);
		}
	}

	IEnumerator init() {
		this.GetComponent<Animator>().enabled = false;

		yield return new WaitUntil(() => ScreenTransitionAnimation.getScreenTransitionAnimation().transition_unshow_ended);

		original_timescale = Time.timeScale;
		Time.timeScale = 0.001f;
		Time.fixedDeltaTime = 0.02F * Time.timeScale;
		this.GetComponent<Animator>().enabled = true;
		this.GetComponentInChildren<Animator>().speed = 1200f;
	}

	void AnimDestroy() {
		shouldPlayAnimation = false;
		Time.timeScale = original_timescale;
		Time.fixedDeltaTime = 0.02F * Time.timeScale;
	}

	public static void EnteringNewLevel() {
		shouldPlayAnimation = true;
	}
}