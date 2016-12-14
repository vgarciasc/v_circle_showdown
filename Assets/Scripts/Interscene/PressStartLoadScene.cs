using UnityEngine;
using UnityEngine.SceneManagement;

public class PressStartLoadScene : MonoBehaviour {
	public string scene;
	
	void Update () {
		handleInput();
	}

	void handleInput() {
		for (int i = 0; i < 4; i++) {
			if (Input.GetButtonDown("Submit_J" + i.ToString()))
				SceneManager.LoadScene(scene);
		}
	}
}
