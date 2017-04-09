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
	Animator mapPreviewAnimator;
	[SerializeField]
	GameObject mapPreview;
	[SerializeField]
	Image mapPreviewImage;
	[SerializeField]
	GameObject mapPreviewGhost;
	[SerializeField]
	Image mapPreviewLeft;
	[SerializeField]
	Image mapPreviewLeftLeft;
	[SerializeField]
	Image mapPreviewRight;
	[SerializeField]
	Image mapPreviewRightRight;
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

		change_map_preview(0);
		mapPreview.gameObject.SetActive(true);
		mapPreviewGhost = Instantiate(mapPreview.gameObject, HushPuppy.safeFind("Canvas").transform, false);
		mapPreview.gameObject.SetActive(false);
	}

	bool in_cooldown = false;

	void Update () {
		for (int i = 0; i < 4; i++) {
			string aux_horizontal = "Horizontal_DPad_J" + i;
			
			float horizontal = Input.GetAxis(aux_horizontal);

			if (!in_cooldown) {
				if (horizontal < 0f) {
					slide_map(Direction.LEFT);
				}
				else if (horizontal > 0f) {
					slide_map(Direction.RIGHT);
				}
			}

			string aux_vertical = "Vertical_DPad_J" + i;
			
			float vertical = Input.GetAxis(aux_vertical);

			if (!in_cooldown) {
				if (vertical < 0f) {
					slide_victory(Direction.DOWN);
				}
				else if (vertical > 0f) {
					slide_victory(Direction.UP);
				}
			}

			if (Input.GetButtonDown("Submit_J" + i)) {
				StartCoroutine(toggle_player_confirmed(pdatabase.get_player_entry_ID(i)));
			}
		}
	}

	IEnumerator handle_cooldown() {
		in_cooldown = true;

		yield return new WaitForSeconds(0.25f);

		in_cooldown = false;
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

	bool in_animation = false;

	void slide_map(Direction direction) {
		if (in_animation) {
			return;
		}

		if (direction == Direction.LEFT) {
			// current_map_index = (current_map_index - 1 + maps.Count) % maps.Count;
			if (mapPreviewGhost != null) Destroy(mapPreviewGhost);
			mapPreview.gameObject.SetActive(true);
			mapPreviewGhost = Instantiate(mapPreview.gameObject, HushPuppy.safeFind("Canvas").transform, false);
			mapPreview.gameObject.SetActive(false);
			mapPreviewGhost.GetComponentInChildren<Animator>().SetTrigger("left");
			in_animation = true;
			change_map_preview(-1);
		}
		else if (direction == Direction.RIGHT) {
			// current_map_index = (current_map_index + 1) % maps.Count;
			if (mapPreviewGhost != null) Destroy(mapPreviewGhost);
			mapPreview.gameObject.SetActive(true);
			mapPreviewGhost = Instantiate(mapPreview.gameObject, HushPuppy.safeFind("Canvas").transform, false);
			mapPreview.gameObject.SetActive(false);
			mapPreviewGhost.GetComponentInChildren<Animator>().SetTrigger("right");
			in_animation = true;
			change_map_preview(+1);
		}

		StartCoroutine(handle_cooldown());
		disable_all_player_confirmed();
	}

	public void change_map_preview(int new_current_map_offset) {
		if (maps.Count < 5) return;
	
		current_map_index = (current_map_index + new_current_map_offset + maps.Count) % maps.Count;

		mapPreview.gameObject.SetActive(true);
		mapPreviewImage.sprite = maps[current_map_index].sprite;
		mapName.text = maps[current_map_index].title;

		mapPreviewLeft.sprite = maps[(current_map_index - 1 + maps.Count) % maps.Count].sprite;
		mapPreviewRight.sprite = maps[(current_map_index + 1) % maps.Count].sprite;
		mapPreviewLeftLeft.sprite = maps[(current_map_index - 2 + maps.Count) % maps.Count].sprite;
		mapPreviewRightRight.sprite = maps[(current_map_index + 2) % maps.Count].sprite;
		mapPreview.gameObject.SetActive(false);
	}

	public void end_animation() {
		Destroy(mapPreviewGhost);
		in_animation = false;

		mapPreview.gameObject.SetActive(true);
		mapPreviewGhost = Instantiate(mapPreview.gameObject, HushPuppy.safeFind("Canvas").transform, false);
		mapPreview.gameObject.SetActive(false);
	}

	void slide_victory(Direction direction) {
		if (direction == Direction.UP) {
			victorySlider.value++;
		}
		else if (direction == Direction.DOWN) {
			victorySlider.value--;
		}

		disable_all_player_confirmed();
		StartCoroutine(handle_cooldown());
	}
}
