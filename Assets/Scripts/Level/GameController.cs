using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {
    PlayerDatabase pdatabase;

    void Awake() {
        pdatabase = (PlayerDatabase) HushPuppy.safeFindComponent("PlayerDatabase", "PlayerDatabase");
    }

    public void checkGameOver() { StartCoroutine(checkGameOver_()); }
    IEnumerator checkGameOver_() {
        yield return new WaitForSeconds(2.0f);
        GameObject[] go = GameObject.FindGameObjectsWithTag("Player");

        if (go.Length == 1) { //apenas um jogador vivo (vitorioso)
            int winnerID = go[0].GetComponent<Player>().playerID;
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
