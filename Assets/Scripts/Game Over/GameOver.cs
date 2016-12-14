using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameOver : MonoBehaviour {
    GameObject player;

    [Header("References")]
    [SerializeField]
    Text victoryMessage;
    [SerializeField]
    Text playerCongratulations;
    [SerializeField]
    Image playerSprite;
    [SerializeField]
    Sprite redcross;
    [SerializeField]
    GameObject playerPrefab;

    void Start () {
        PlayerDatabase pd = (PlayerDatabase) HushPuppy.safeFindComponent("PlayerDatabase", "PlayerDatabase");
        if (pd.winnerID == -1) {
            playerSprite.sprite = redcross;
            victoryMessage.text = "IT'S A TIE!";
            playerCongratulations.text = "YOU ARE WEAK!";
            return;
        }

        playerCongratulations.text = "CONGRATULATIONS, PLAYER " + (pd.players[pd.winnerID].playerID + 1).ToString() + "!";
        playerSprite.color = pd.players[pd.winnerID].color;
        pd.winnerID = -1;
    }

    void Update() {
        playerSprite.transform.Rotate(playerSprite.transform.up, 1f);
    }
}
