using UnityEngine;
using System.Collections;

public class PlayerDatabaseSpawner : MonoBehaviour {
	public GameObject[] players;

	public GameObject activatePlayer(int id) {
		players[id].SetActive(true);
		return players[id];
	}

	public void setPlayer(int id, PlayerInstance instance) {
		players[id].transform.GetChild(1).GetComponent<SpriteRenderer>().color = instance.color;
		players[id].GetComponent<Player>().setPlayer(instance);
	}
}
