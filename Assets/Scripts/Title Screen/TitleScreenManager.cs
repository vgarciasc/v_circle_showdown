using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleScreenManager : MonoBehaviour {
	public Image blackscreen;
	public Animator startAnimator,
				openingArtAnimator;
	
	void Update () {
		if (Input.GetButtonDown("Submit")) {
			callNextScene_();
		}
	}

	public void Anim_startIdle() {
		startAnimator.SetBool("idle", true); }

	public void callNextScene_() { StartCoroutine(callNextScene()); }
	IEnumerator callNextScene() {
        SpecialCamera scamera = Camera.main.GetComponent<SpecialCamera>();
		scamera.screenShake_(2f);

		startAnimator.SetTrigger("start");
		openingArtAnimator.SetTrigger("start");

		yield return new WaitForSeconds(0.5f);
		blackscreen.enabled = true;
		HushPuppy.fadeIn(blackscreen.gameObject, 2f);
		yield return new WaitForSeconds(2.5f);

		SceneManager.LoadScene("PlayerSelect");
	}
}
