using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleScreenManager : MonoBehaviour {
	public Image blackscreen;
	public Animator startAnimator;
	
	void Update () {
		if (Input.GetButtonDown("Submit")) {
			callNextScene_();
			//startAnimator.SetTrigger("start");
		}
	}

	public void callNextScene_() { StartCoroutine(callNextScene()); }
	IEnumerator callNextScene() {
		blackscreen.enabled = true;
		HushPuppy.fadeIn(blackscreen.gameObject, 2f);
		yield return new WaitForSeconds(2.5f);

		SceneManager.LoadScene("PlayerSelect");
	}
}
