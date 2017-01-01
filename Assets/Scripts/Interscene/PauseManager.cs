using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour {
	public GameObject pauseMenu;
	bool pause;
	public bool thisScreenCanPause = true;

	Transform root;

	void Start() {
		root = this.transform;
		while (root.parent != null) {
			root = root.parent;
		}
	}

	void Update () {
		if (thisScreenCanPause) {
			for (int i = 0; i < 4; i++) {
				if (Input.GetButtonDown("Submit_J" + i)) {
					pauseGame();
				}
			}
		}
	}

	void pauseGame() {
		if (!pause) {
			Time.timeScale = 0f;
			// HushPuppy.BroadcastAll("OnPause");
			pauseMenu.SetActive(true);
		}
		else {
			Time.timeScale = 1f;
			// HushPuppy.BroadcastAll("OnUnPause");		
			pauseMenu.SetActive(false);
		}

		pause = !pause;
	}

    public IEnumerator WaitForSecondsInterruptable(float duration) {
        float time = 0;
        while (time < duration) {
            if (!pause) {
                time += Time.deltaTime;
            }
            yield return new WaitForEndOfFrame();
        }
    }

	public static PauseManager getPauseManager() {
		PauseManager pause;
		pause = (PauseManager) HushPuppy.safeFindComponent("GameController", "PauseManager");
		return pause;
	}
}
