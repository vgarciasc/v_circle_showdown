using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleScreenManager : MonoBehaviour {
	public Image blackscreen;
	public Animator startAnimator,
				openingArtAnimator;
	
	bool hasPressedStart;
	
	void Update () {
		for (int i = 0; i < 4; i++) {
			if (Input.GetButtonDown("Submit_J" + i)) {
				callNextScene_();
			}
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
		openingArtAnimator.SetTrigger("start");

		yield return new WaitForSeconds(1f);
		blackscreen.enabled = true;
		HushPuppy.fadeIn(blackscreen.gameObject, 2f);
		yield return new WaitForSeconds(2.5f);

		SceneManager.LoadScene("PlayerSelect");
	}
}
