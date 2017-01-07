using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ColorComponent : MonoBehaviour {
	ColorController controller;
	public ColorType my_type;
	public SpriteRenderer image;
	public bool image_is_self;
	public bool is_sprite_renderer = true;

	void Start () {
		controller = (ColorController) HushPuppy.safeFindComponent("GameController", "ColorController");
		if (controller != null) {
			set_color();
		}
	}

	void Update() {
		//DEBUG, PODE SER RETIRADO
		if (controller != null) {
			set_color();
		}
	}

	void set_color() {
		if (is_sprite_renderer) {
			if (image_is_self) {
				SpriteRenderer sr = this.GetComponent<SpriteRenderer>();
				if (sr != null) {
					sr.color = controller.get_color(my_type);
				}
			}
			else {
				SpriteRenderer sr = image.GetComponent<SpriteRenderer>();
				if (sr != null) {
					sr.color = controller.get_color(my_type);
				}
			}
		}
		else {
			if (image_is_self) {
				Image img = this.GetComponent<Image>();
				if (img != null) {
					img.color = controller.get_color(my_type);
				}
			}
			else {
				Image img = image.GetComponent<Image>();
				if (img != null) {
					img.color = controller.get_color(my_type);
				}
			}
		}
	}
}
