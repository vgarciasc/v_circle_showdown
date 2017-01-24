using UnityEngine;
using System.Collections;

public class PlayerDatabaseSpawner : MonoBehaviour {
	public GameObject[] players;
	public GameObject[] doors;
	public GameObject[] player_UI_panels;

	void Start() {
		foreach (GameObject go in player_UI_panels) {
			go.SetActive(false);
		}
	}

	public GameObject activatePlayer(int id) {
		GameObject player = players[id];
		player_UI_panels[id].SetActive(true);
		player.SetActive(true);
		player.GetComponent<Rigidbody2D>().velocity = new Vector3(20f * Mathf.Pow(-1, id), 0, 0);
		closeDoor(doors[id]);
		return player;
	}

	void closeDoor(GameObject door) {
		door.GetComponent<Platform>().enabled = true;
	}

	public void setPlayer(int id, PlayerInstance instance) {
		players[id].transform.GetChild(1).GetComponent<SpriteRenderer>().color = instance.color;
		players[id].GetComponent<Player>().setPlayer(instance);
		players[id].GetComponent<PlayerParticleSystems>().Start();
	}
}
