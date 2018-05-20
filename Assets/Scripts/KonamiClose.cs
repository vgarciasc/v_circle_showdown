using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KonamiClose : MonoBehaviour {

	enum Buttons { JUMP, FIRE1, FIRE2, FIRE3, SUBMIT, SUBMIT2};

	void Start() {
		StartCoroutine(TrySecretOrder(
			new List<Buttons>() {
				Buttons.SUBMIT,
				Buttons.SUBMIT2,
				Buttons.SUBMIT,
				Buttons.SUBMIT2,
				Buttons.SUBMIT,
				Buttons.SUBMIT2,
				Buttons.JUMP,
				Buttons.FIRE1,
				Buttons.FIRE2,
			}, 
			() => {
				print("Fechando!");
				Application.Quit();
			}));

		StartCoroutine(TrySecretOrder(
			new List<Buttons>() {
				Buttons.SUBMIT2,
				Buttons.SUBMIT,
				Buttons.SUBMIT2,
				Buttons.SUBMIT,
				Buttons.SUBMIT2,
				Buttons.SUBMIT,
				Buttons.JUMP,
				Buttons.FIRE1,
				Buttons.FIRE2,
			}, 
			() => {
				print("Level select!");
				SceneLoader.getSceneLoader().LoadScene("LevelSelect 1");
			}));
	}

	IEnumerator TrySecretOrder(List<Buttons> secretOrder, System.Action del) {
		for (int i = 0; i < secretOrder.Count; i++) {
			yield return new WaitForSeconds(0.5f);
			yield return WaitForButtonPress();

			if (!ButtonPressed(secretOrder[i])) {
				StartCoroutine(TrySecretOrder(secretOrder, del));
				yield break;
			}
		}

		del.Invoke();
	}

	IEnumerator WaitForButtonPress() {
		yield return new WaitUntil(() => 
			ButtonPressed(Buttons.FIRE1)  || 
			ButtonPressed(Buttons.FIRE2)  || 
			ButtonPressed(Buttons.FIRE3)  || 
			ButtonPressed(Buttons.JUMP)   || 
			ButtonPressed(Buttons.SUBMIT) || 
			ButtonPressed(Buttons.SUBMIT2));
	}

	bool ButtonPressed(Buttons button) {
		switch (button) {
			case Buttons.JUMP:    return AnyPlayerPressed("Jump_J"); 
			case Buttons.FIRE1:   return AnyPlayerPressed("Fire1_J");
			case Buttons.FIRE2:   return AnyPlayerPressed("Fire2_J");
			case Buttons.FIRE3:   return AnyPlayerPressed("Fire3_J");
			case Buttons.SUBMIT:  return AnyPlayerPressed("Submit_J");
			case Buttons.SUBMIT2: return AnyPlayerPressed("Submit2_J");
			default: return false;
		}
	}

	bool AnyPlayerPressed(string buttonName) {
		bool pressed = false;
		for (int i = 0; i < 4; i++) {
			if (Input.GetButtonUp(buttonName + i)) {
				pressed = true;
			}
		}

		return pressed;
	}
}
