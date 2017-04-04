using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverWinnerStats : MonoBehaviour {
	[SerializeField]
	Transform victories;
	[SerializeField]
	GameObject victory_icon;

	public void set(PlayerInstance player) {
		VictoriesManager vmanager = VictoriesManager.getVictoriesManager();
		int victory_count = vmanager.get_player_victories(player.playerID);
		for (int i = 0; i < vmanager.get_victories_needed(); i++) {
			GameObject icon = Instantiate(victory_icon, victories, false);
			Color color_aux;
			if (i < victory_count) {
				color_aux = player.palette.color;
			}
			else {
				color_aux = Color.white;
				color_aux = HushPuppy.getColorWithOpacity(color_aux, 0.3f);
			}

			icon.GetComponent<Image>().color = color_aux;
			icon.GetComponent<Animator>().SetTrigger("sparkle");
		}
	}
}
