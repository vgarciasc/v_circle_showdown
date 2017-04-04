using UnityEngine;
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
	string player_name;
	PlayerUIMarker marker;
	PlayerUIStatus status;

	void Start() {
		player = (Player) HushPuppy.safeComponent(this.gameObject, "Player");

		this.player_color = player.palette.color;
		this.player_ID = player.ID;
		this.player_name = player.playername;
		
		player.death_event += on_player_death;
		player.get_item_event += on_item_get;
		player.use_item_event += on_item_use;
		player.victory_event += get_victory;
		startUI();
	}

	void Update() {
		marker.setPosition(this.transform.position);
	}

	void startUI() {
        GameObject playerUI_container = HushPuppy.safeFind("PlayerUIContainer");
		if (playerAlreadyHasUI(playerUI_container)) return;

        status = Instantiate(playerStatusPrefab).GetComponent<PlayerUIStatus>();
        status.name = player_name + " Status";
        status.transform.SetParent(playerUI_container.transform.GetChild(0), false);
        status.setUI(player_name, player_ID, player_color);

        marker = Instantiate(playerMarkerPrefab).GetComponent<PlayerUIMarker>();
        marker.name = player_name + " Marker";
        marker.transform.SetParent(playerUI_container.transform.GetChild(1), false);
        marker.setMarker(player_color);
		
		StartCoroutine(checkOutOfScreen());
	}

	bool playerAlreadyHasUI(GameObject container) {
		foreach (Transform go in container.transform.GetChild(0)) {
			if (go.name == player.playername + " Status") {
				status = go.GetComponent<PlayerUIStatus>();
				status.reset();
				marker = GameObject.Find(player.playername + " Marker").GetComponent<PlayerUIMarker>();
				return true;
			}
		}

		return false;
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
		yield return PauseManager.getPauseManager().WaitForSecondsInterruptable(1f);
		float timeLeft = player.originalData.maxSecondsOutOfScreen;
		while (SceneManager.GetActiveScene().name != "GameOver") {
			if (player.should_be_visible) {
				if (this.GetComponentInChildren<SpriteRenderer>().isVisible) {
					timeLeft = player.originalData.maxSecondsOutOfScreen;
					marker.setTime(false);
				} else {
					marker.setTime(timeLeft--);
				}

				if (timeLeft < 0) player.timeOut();
			}

			yield return PauseManager.getPauseManager().WaitForSecondsInterruptable(1f);
		}
	}

	void get_victory() {
		status.get_victory();
	}
}
