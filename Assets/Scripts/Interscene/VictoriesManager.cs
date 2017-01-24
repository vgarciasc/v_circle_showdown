using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoriesManager : MonoBehaviour {
	List<int> player_victories = new List<int>();
	int victories_needed = 0;
	int current_winner = -1;

	public static VictoriesManager getVictoriesManager() {
		return (VictoriesManager) HushPuppy.safeFindComponent("PlayerDatabase", "VictoriesManager");
	}

	public void set_players(int player_count) {
		current_winner = -1;
		for (int i = 0; i < player_count; i++) {
			player_victories.Add(0);
		}
	}

	public void reset_victories() {
		current_winner = -1;
		for (int i = 0; i < player_victories.Count; i++) {
			player_victories[i] = 0;
		}
	}

	public void give_victory(int playerID) {
		player_victories[playerID]++;
	}

	public void set_victories_needed(int victories_needed) {
		this.victories_needed = victories_needed;
	}

	public int get_victories_needed() {
		return this.victories_needed; 
	}

	public int get_player_victories(int player_ID) {
		return player_victories[player_ID];
	}

	public int get_match_winner() {
        GameObject[] go = GameObject.FindGameObjectsWithTag("Player");

        if (go.Length == 1) { //apenas um jogador vivo (vitorioso)
            return go[0].GetComponent<Player>().ID;
        } else if (go.Length == 0) { //nenhum jogador vivo (empate)
            return -2;
        }

		return -1;
	}

	public int get_game_winner() {
		//game winner has already been decided
		if (current_winner != -1) {
			return current_winner;
		}

		//lets decide the game winner
		for (int i = 0; i < player_victories.Count; i++) {
			if (player_victories[i] >= victories_needed) {
				current_winner = i;
				return i;
			}
		}

		return -1; //no winner yet
	}
}
