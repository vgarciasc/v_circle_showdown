using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SoccerGameController : MonoBehaviour {

	public GameObject soccerballPrefab,
					soccerPlayerTeamMarker;
	public Transform soccerballSpawn;
	public Text[] scores;
	public Transform[] team_markers;
	public Goal[] goals;

	int[] team_score;
	GameObject currentBall;
	PlayerSpawner playerSpawner;
	PlayerDatabase pdatabase;

	void Start () {
		team_score = new int[2] {0, 0};
		splitTeams();
		newBall();

		playerSpawner = (PlayerSpawner) HushPuppy.safeFindComponent("GameController", "PlayerSpawner");
		pdatabase = (PlayerDatabase) HushPuppy.safeFindComponent("PlayerDatabase", "PlayerDatabase");
	}
	
	void newBall() {
		currentBall = Instantiate(soccerballPrefab, soccerballSpawn);
	}

    void Update() {
        if (Input.GetKeyDown(KeyCode.L)) {
            SceneManager.LoadScene("LevelSelect");
        }
    }

	public void scorePoint(int team) {
		team_score[team]++;
		scores[team].text = team_score[team].ToString();
		// StartCoroutine(checkWin(team));
		StartCoroutine(destroyBall(currentBall));
		newBall();
	}

	// IEnumerator checkWin(int team) {
	// 	// if (team_score[team] >= VictoriesManager.getVictoriesManager()) {
	// 	// 	scores[team].text = "V";
	// 	// 	scores[(team + 1) % 2].text = "P";
	// 	// 	yield return new WaitForSeconds(2.0f);
			
	// 	// 	//nao funciona
	// 	// 	Color aux = team_markers[team].GetChild(0).GetComponent<Image>().color;
	// 	// 	GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
	// 	// 	for (int i = 0; i < players.Length; i++) {
	// 	// 		if (players[i].GetComponent<Player>().color == aux) {
	// 	// 			pdatabase.giveVictoryTo(players[i].GetComponent<Player>().ID);
	// 	// 		}
	// 	// 	}
	// 	// 	SceneManager.LoadScene("GameOver");
	// 	// }
	// }

	IEnumerator destroyBall(GameObject ball) {
		float currentLerpTime = 0f;
		Color aux = ball.GetComponent<SpriteRenderer>().color;
		
		while (currentLerpTime < 1f) {
			currentLerpTime += Time.deltaTime / 1f;
			ball.GetComponent<SpriteRenderer>().color = Color.Lerp(aux,
																	HushPuppy.getColorWithOpacity(aux, 0f),
																	currentLerpTime);
			yield return new WaitForEndOfFrame();
		}

		Destroy(ball);
	}

	void splitTeams() {
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
		for (int i = 0; i < players.Length; i++) {
			sendPlayer(players[i].GetComponent<Player>(), team_markers[i % 2]);
			goals[i % 2].colors.Add(players[i].GetComponent<Player>().color);
		}
	}

	void sendPlayer(Player player, Transform team_marker) {
		GameObject aux = Instantiate(soccerPlayerTeamMarker, team_marker, false);
		aux.GetComponent<Image>().color = player.color;
		player.id_death_event += respawnPlayer;
	}

	void respawnPlayer(PlayerInstance instance) { StartCoroutine(respawnPlayer_(instance)); }
	IEnumerator respawnPlayer_(PlayerInstance instance) {
		yield return new WaitForSeconds(3f);
		Player player = playerSpawner.spawnPlayer(instance, soccerballSpawn.position);
		player.id_death_event += respawnPlayer;
	}
}
