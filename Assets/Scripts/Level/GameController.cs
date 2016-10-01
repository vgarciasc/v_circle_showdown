using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour {
    public void checkGameOver() { StartCoroutine(checkGameOver_()); }
    IEnumerator checkGameOver_() {
        yield return new WaitForSeconds(2.0f);
        GameObject[] go = GameObject.FindGameObjectsWithTag("Player");
        if (go.Length == 1) {
            int winnerID = go[0].GetComponent<Player>().playerID;
            HushPuppy.findGameObject("Player Data").GetComponent<PlayerDatabase>().winnerID = winnerID;
            SceneManager.LoadScene("GameOver");
        } else if (go.Length == 0) {
            SceneManager.LoadScene("GameOver");
        }
    }
}
