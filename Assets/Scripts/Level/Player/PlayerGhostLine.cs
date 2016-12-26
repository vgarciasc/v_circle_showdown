using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGhostLine : MonoBehaviour {
	Transform origin,
			direction;
	LineRenderer line;

	public void init(Transform origin, Transform direction) {
		this.origin = origin;
		this.direction = direction;
		this.line = (LineRenderer) HushPuppy.safeComponent(this.gameObject, "LineRenderer");
	}

	void Update() {
		line.SetPosition(0, origin.position);
		line.SetPosition(1, direction.position);
	}
}
