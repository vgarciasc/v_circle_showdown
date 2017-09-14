using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class CoffeeOverdrive : MonoBehaviour {
	public static CoffeeOverdrive get_coffee_overdrive() {
		return (CoffeeOverdrive) HushPuppy.safeFindComponent("CoffeeOverdrive", "CoffeeOverdrive");
	}

	TextMeshProUGUI text;

	void Start() {
		text = this.GetComponentInChildren<TextMeshProUGUI>();
	}

	public void _show() {
		if (!showing) {
			StartCoroutine(show());
		}
	}

	bool showing = false;
	IEnumerator show() {
		List<Color> colors = new List<Color>() {
			new Color(0.76f, 0.9f, 0.58f),
			new Color(0.94f, 0.59f, 0.49f),
			new Color(0.46f, 0.9f, 0.82f),
			new Color(0.85f, 0.9f, 0.55f)
		};
		colors.AddRange(colors);
		colors.AddRange(colors);

		showing = true;
		text.enabled = true;
		float time = 0.3f;

		foreach (Color c in colors) {
			text.DOColor(
				c,
				time
			);
			yield return new WaitForSeconds(time / 3);
		}

		text.DOColor(
			Color.clear,
			time
		);
		
		yield return new WaitForSeconds(time);

		text.enabled = false;
		showing = false;
	}
}
