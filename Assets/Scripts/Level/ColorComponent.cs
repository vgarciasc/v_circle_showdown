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
		set_color();
	}

	void Update() {
		set_color();
	}

	void set_color() {
		if (is_sprite_renderer) {
			if (image_is_self) {
				this.GetComponent<SpriteRenderer>().color = controller.get_color(my_type);
			}
			else {
				image.GetComponent<SpriteRenderer>().color = controller.get_color(my_type);
			}
		}
		else {
			if (image_is_self) {
				this.GetComponent<Image>().color = controller.get_color(my_type);
			}
			else {
				image.GetComponent<Image>().color = controller.get_color(my_type);
			}
		}
	}
}
