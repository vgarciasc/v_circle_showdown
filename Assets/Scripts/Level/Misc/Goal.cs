using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {
	public int team;
	public List<Color> colors = new List<Color>();
	public GameObject[] goal_section;

	SoccerGameController sgc;
	bool cooldown = false;

	void Start () {
		sgc = (SoccerGameController) HushPuppy.safeFindComponent("GameController", "SoccerGameController");
	}

	void OnTriggerExit2D(Collider2D target) {
		if (target.gameObject.tag == "Soccerball") {
			if (!cooldown) {
				sgc.scorePoint(team);
				StartCoroutine(cooldownOff());
			}
		}
	}

	void Update() {
		if (colors.Count == 1) {
			goal_section[0].GetComponent<SpriteRenderer>().color = 
				goal_section[1].GetComponent<SpriteRenderer>().color =
				colors[0];
		}
		else if (colors.Count == 2) {
			goal_section[0].GetComponent<SpriteRenderer>().color = colors[0];
			goal_section[1].GetComponent<SpriteRenderer>().color = colors[1];
		}
	}

	IEnumerator cooldownOff() {
		cooldown = true;

		yield return new WaitForSeconds(1f);

		cooldown = false;
	}
}
