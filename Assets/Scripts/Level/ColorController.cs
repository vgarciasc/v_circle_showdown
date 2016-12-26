using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ColorType { BACKGROUND, PLATFORM, SPIKE, CHARGEBALL, PLATFORM_ALT }

[System.Serializable]
public class ColorScheme : System.Object {
	public ColorType type;
	public Color color;
}

public class ColorController : MonoBehaviour {
	public List<ColorScheme> colors = new List<ColorScheme>();

	public Color get_color(ColorType type) {
		Color aux = Color.magenta; //default
		for (int i = 0; i < colors.Count; i++) {
			if (colors[i].type == type) {
				aux = colors[i].color;
			}
		}
		
		return aux;
	}
}
