using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour {
    PlayerDatabase pdatabase;

    void Start() {
        pdatabase = (PlayerDatabase) HushPuppy.safeFindComponent("PlayerDatabase", "PlayerDatabase");
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player")) {
            Player player = (Player) HushPuppy.safeComponent(go, "Player");
            player.death_event += checkGameOver_;
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.L)) {
            SceneManager.LoadScene("LevelSelect");
            pdatabase.resetVictories();
        }
    }

    public void checkGameOver_() { StartCoroutine(checkGameOver()); }
    IEnumerator checkGameOver() {
        yield return new WaitForSeconds(2.0f);
        GameObject[] go = GameObject.FindGameObjectsWithTag("Player");

        if (go.Length == 1) { //apenas um jogador vivo (vitorioso)
            int winnerID = go[0].GetComponent<Player>().ID;
            getNextScene(winnerID);
        } else if (go.Length == 0) { //nenhum jogador vivo (empate)
            getNextScene(-1);
        }
    }

    void getNextScene(int matchWinnerID) {
        if (matchWinnerID == -1) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        pdatabase.giveVictoryTo(matchWinnerID);
        int gameWinnerID = pdatabase.getGameWinner();

        if (gameWinnerID == -1) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        } else {
            pdatabase.winnerID = gameWinnerID;
            pdatabase.resetVictories();
            SceneManager.LoadScene("GameOver");
        }
    }
}
