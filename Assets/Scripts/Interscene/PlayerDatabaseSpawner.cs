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
		StartCoroutine(closeDoor(doors[id]));
		return player;
	}

	IEnumerator closeDoor(GameObject door) {
		yield return new WaitForSeconds(0.5f);
		door.GetComponentInChildren<Rigidbody2D>().velocity = Vector3.up * 3;
		yield return new WaitForSeconds(1f);
		door.GetComponentInChildren<Rigidbody2D>().velocity = Vector3.zero;
	}

	public void setPlayer(int id, PlayerInstance instance) {
		players[id].transform.GetChild(1).GetComponent<SpriteRenderer>().color = instance.color;
		players[id].GetComponent<Player>().setPlayer(instance);
	}
}
