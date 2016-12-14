﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerUIManager : MonoBehaviour {
    [SerializeField]
    GameObject playerStatusPrefab;
    [SerializeField]
    GameObject playerMarkerPrefab;

	Player player;
	Color player_color;
	int player_ID;
	PlayerUIMarker marker;
	PlayerUIStatus status;

	void Start() {
		player = (Player) HushPuppy.safeComponent(this.gameObject, "Player");
		
		this.player_color = player.color;
		this.player_ID = player.ID;
		player.death_event += on_player_death;
		player.get_item_event += on_item_get;
		player.use_item_event += on_item_use;
		startUI();
	}

	void Update() {
		marker.setPosition(this.transform.position);
	}

	void startUI() {
        GameObject playerUI_container = HushPuppy.safeFind("PlayerUIContainer");

        status = Instantiate(playerStatusPrefab).GetComponent<PlayerUIStatus>();
        status.name = "Player #" + (player_ID + 1) + " Status";
        status.transform.SetParent(playerUI_container.transform.GetChild(0), false);
        status.setUI(player_ID, player_color);

        marker = Instantiate(playerMarkerPrefab).GetComponent<PlayerUIMarker>();
        marker.name = "Player #" + (player_ID + 1) + " Marker";
        marker.transform.SetParent(playerUI_container.transform.GetChild(1), false);
        marker.setMarker(player_color);
		
		StartCoroutine(checkOutOfScreen());
	}

	void on_player_death() {
		marker.playerKilled_();
		status.playerKilled();
	}

	void on_item_use(ItemData item_data) {
		status.unshowItem();
	}

	void on_item_get(ItemData item_data) {
		status.showItem(item_data);
	}

	IEnumerator checkOutOfScreen() {
		yield return new WaitForSeconds(1f);
		float timeLeft = player.data.maxSecondsOutOfScreen;
		while (SceneManager.GetActiveScene().name != "GameOver") {
			if (this.GetComponent<SpriteRenderer>().isVisible) {
				timeLeft = player.data.maxSecondsOutOfScreen;
				status.setTime(false);
			} else {
				status.setTime(timeLeft--);
			}

			if (timeLeft < 0) player.timeOut();

			yield return new WaitForSeconds(1f);
		}
	}
}