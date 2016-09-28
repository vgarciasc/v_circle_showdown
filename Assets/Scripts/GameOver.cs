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

    void Start () {
        player = HushPuppy.findGameObject("Player");
        if (player == null) {
            playerSprite.sprite = redcross;
            victoryMessage.text = "IT'S A TIE!";
            playerCongratulations.text = "YOU ARE WEAK!";
            return;
        }

        playerCongratulations.text = "CONGRATULATIONS, PLAYER " + player.GetComponent<Player>().playerID + "!";
        playerSprite.sprite = player.GetComponent<SpriteRenderer>().sprite;
        playerSprite.color = player.GetComponent<SpriteRenderer>().color;
        Destroy(player);
    }

    void Update() {
        playerSprite.transform.Rotate(playerSprite.transform.up, 1f);
    }
}
