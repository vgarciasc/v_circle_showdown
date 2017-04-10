using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using DG.Tweening;

public class TitleScreenManager : MonoBehaviour {
	public Image blackscreen;
	public Animator startAnimator;
	
	bool hasPressedStart;
	Transform camera;

	void Update () {
		for (int i = 0; i < 4; i++) {
			if (Input.GetButtonDown("Submit_J" + i)) {
				callNextScene_();
			}
		}
	}

	void Start() {
		camera = Camera.main.transform;
		StartCoroutine(moveCamera());
	}

	IEnumerator moveCamera() {
		while (true) {
			camera.DOMove(new Vector3(camera.position.x,
				camera.position.y + 10,
				camera.position.z),
				0.5f);
			yield return new WaitForSeconds(0.5f);
		}
	}

	public void Anim_startIdle() {
		startAnimator.SetBool("idle", true); }

	public void callNextScene_() {
		if (hasPressedStart) return;
		else hasPressedStart = true;
		StartCoroutine(callNextScene());
	}

	IEnumerator callNextScene() {
        SpecialCamera scamera = Camera.main.GetComponent<SpecialCamera>();
		scamera.screenShake_(1f);

		startAnimator.SetTrigger("start");

		yield return new WaitForSeconds(1f);
		blackscreen.enabled = true;
		HushPuppy.fadeIn(blackscreen.gameObject, 2f);
		yield return new WaitForSeconds(2.5f);

		SceneManager.LoadScene("PlayerSelect");
	}
}
