using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour {
    PlayerDatabase pdatabase;

    public void checkGameOver() { StartCoroutine(checkGameOver_()); }
    IEnumerator checkGameOver_() {
        pdatabase = (PlayerDatabase) HushPuppy.safeFindComponent("PlayerDatabase", "PlayerDatabase");
        yield return new WaitForSeconds(2.0f);
        GameObject[] go = GameObject.FindGameObjectsWithTag("Player");
        if (go.Length == 1) {
            int winnerID = go[0].GetComponent<Player>().playerID;
            pdatabase.winnerID = winnerID;
            SceneManager.LoadScene("GameOver");
        } else if (go.Length == 0) {
            SceneManager.LoadScene("GameOver");
        }
    }
}
