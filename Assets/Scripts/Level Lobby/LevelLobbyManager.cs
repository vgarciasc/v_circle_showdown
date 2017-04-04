using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class Map {
	public Sprite sprite;
	public string title;
	public string sceneName;
}

public class LevelLobbyManager : MonoBehaviour {
	public enum Direction { LEFT, RIGHT, UP, DOWN };
	
	[SerializeField]
	Color enabledColor;
	[SerializeField]
	Color disabledColor;

	[SerializeField]
	GameObject playerSelectionUIPrefab;
	[SerializeField]
	GameObject playerSelectionUIContainer;

	int numberOfPlayersConfirmed = 0;

	List<Transform> playerSelections;
	List<bool> playersConfirmed;

	[SerializeField]
	Image mapPreview;
	[SerializeField]
	Image mapPreviewLeft;
	[SerializeField]
	Image mapPreviewRight;
	[SerializeField]
	TextMeshProUGUI mapName;

	[SerializeField]
	List<Map> maps;

	[SerializeField]
	Slider victorySlider;

	int current_map_index = 0;

	PlayerDatabase pdatabase;

	void Start() {
		pdatabase = PlayerDatabase.getPlayerDatabase();
		playerSelections = new List<Transform>();
		playersConfirmed = new List<bool>();

		if (pdatabase == null) return;

		for (int i = 0; i < pdatabase.players.Count; i++) {
			GameObject aux = Instantiate(playerSelectionUIPrefab,
										playerSelectionUIContainer.transform,
										false);
			playerSelections.Add(aux.transform);
			playersConfirmed.Add(false);
			playerSelections[i].GetComponentsInChildren<Image>()[1].color = pdatabase.players[i].palette.color;
			playerSelections[i].GetComponentsInChildren<Image>()[0].color = disabledColor;
		}

		change_map_preview();
	}

	void Update () {
		for (int i = 0; i < 4; i++) {
			string aux_horizontal = "Horizontal_J" + i;
			float horizontal = Input.GetAxis(aux_horizontal);
			if (Input.GetButtonDown(aux_horizontal) && horizontal < 0f) {
				slide_map(Direction.LEFT);
			}
			else if (Input.GetButtonDown(aux_horizontal) && horizontal > 0f) {
				slide_map(Direction.RIGHT);
			}

			string aux_vertical = "Vertical_J" + i;
			float vertical = Input.GetAxis(aux_vertical);
			if (Input.GetButtonDown(aux_vertical) && vertical < 0f) {
				slide_victory(Direction.DOWN);
			}
			else if (Input.GetButtonDown(aux_vertical) && vertical > 0f) {
				slide_victory(Direction.UP);
			}

			if (Input.GetButtonDown("Submit_J" + i)) {
				StartCoroutine(toggle_player_confirmed(pdatabase.get_player_entry_ID(i)));
			}
		}
	}
	
	IEnumerator toggle_player_confirmed(int player_index) {
		if (!playersConfirmed[player_index]) {
			playerSelections[player_index].GetComponentsInChildren<Image>()[0].color = enabledColor;
			playerSelections[player_index].GetComponent<Animator>().SetBool("active", true);
			yield return new WaitForSeconds(0.5f);
			numberOfPlayersConfirmed++;

		}	
		else {
			playerSelections[player_index].GetComponentsInChildren<Image>()[0].color = disabledColor;
			playerSelections[player_index].GetComponent<Animator>().SetBool("active", false);
			yield return new WaitForSeconds(0.5f);
			numberOfPlayersConfirmed--;

		}

		playersConfirmed[player_index] = !playersConfirmed[player_index];

		if (numberOfPlayersConfirmed == pdatabase.players.Count) {
			SceneLoader.getSceneLoader().LoadLevel(maps[current_map_index].sceneName);
		}
	}

	void disable_all_player_confirmed() {
		for (int i = 0; i < playersConfirmed.Count; i++) {
			playersConfirmed[i] = false;
			playerSelections[i].GetComponent<Animator>().SetBool("active", false);
			// playerSelections[i].GetComponentsInChildren<Image>()[0].color = disabledColor;
		}

		numberOfPlayersConfirmed = 0;
	}

	void slide_map(Direction direction) {
		if (direction == Direction.LEFT) {
			current_map_index = (current_map_index - 1 + maps.Count) % maps.Count;
		}
		else if (direction == Direction.RIGHT) {
			current_map_index = (current_map_index + 1) % maps.Count;
		}

		change_map_preview();
		disable_all_player_confirmed();
	}

	void change_map_preview() {
		if (maps.Count < 3) return;

		mapPreview.sprite = maps[current_map_index].sprite;
		mapName.text = maps[current_map_index].title;

		mapPreviewLeft.sprite = maps[(current_map_index - 1 + maps.Count) % maps.Count].sprite;
		mapPreviewRight.sprite = maps[(current_map_index + 1) % maps.Count].sprite;
	}

	void slide_victory(Direction direction) {
		if (direction == Direction.UP) {
			victorySlider.value++;
		}
		else if (direction == Direction.DOWN) {
			victorySlider.value--;
		}
	}
}
