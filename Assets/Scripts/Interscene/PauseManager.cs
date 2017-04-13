using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour {
	public GameObject pauseMenu;
	bool pause;
	public bool thisScreenCanPause = true;
	public bool canPauseNow = true;

	Transform root;

	public static PauseManager getPauseManager() {
		PauseManager pause;
		pause = (PauseManager) HushPuppy.safeFindComponent("GameController", "PauseManager");
		return pause;
	}

	void Start() {
		root = this.transform;
		while (root.parent != null) {
			root = root.parent;
		}
	}

	ShowdownPanelAnimation showd;
	ScreenTransitionAnimation screent;

	void Awake() {
		init_showd();
		init_screent();
	}

	void init_showd() {
		GameObject aux = GameObject.FindGameObjectWithTag("ScreenAnimation");
		if (aux == null) {
			Debug.Log("Showdown Panel Animation does not exist in this scene.");
			showd = null;
			return;
		}

		showd = aux.GetComponentInChildren<ShowdownPanelAnimation>();
	}

	void init_screent() {
		GameObject aux = GameObject.FindGameObjectWithTag("ScreenAnimation");
		if (aux == null) {
			Debug.Log("Screen Transition Animation does not exist in this scene.");
			screent = null;
			return;
		}

		screent = aux.GetComponentInChildren<ScreenTransitionAnimation>();
	}

	void Update () {
		// if (showd != null) {
		// 	canPauseNow = !showd.playingAnimation;
		// } else if (showd == null) {
		// 	canPauseNow = true;
		// }

		if (showd != null && screent != null) {
			canPauseNow = (!showd.playingAnimation && !screent.inside_transition);
		}

		if (thisScreenCanPause && canPauseNow) {
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

	public void setCanPauseNow(bool value) {
		canPauseNow = value;
	}
}
