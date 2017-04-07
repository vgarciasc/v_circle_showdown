using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowdownPanelAnimation : MonoBehaviour {

	public static bool playerMustWaitUntilSpawn = false;

	void Awake() {
		if (!playerMustWaitUntilSpawn) {
			this.gameObject.SetActive(false);
		}
	}

	void AnimDestroy() {
		playerMustWaitUntilSpawn = false;
	}

	public static void EnteringNewLevel() {
		playerMustWaitUntilSpawn = true;
	}
}
