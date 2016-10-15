using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerUIStatus : MonoBehaviour {
    [Header("References")]
    [SerializeField]
    Image redCross;
    [SerializeField]
    Text time;

    Coroutine countCoroutine;

    void Start() {
        time.enabled = false; }

    public void setUI(int playerID, SpriteRenderer playerSprite) {
        this.GetComponentsInChildren<Text>()[0].text = "Player #" + playerID.ToString();
        this.GetComponentsInChildren<Image>()[1].color = playerSprite.color;
    }

    public void setTime(float time) {
        this.time.enabled = true;
        this.time.text = time.ToString();
    }

    public void setTime(bool value) {
        this.time.enabled = value; }

    public void playerKilled() { 
        redCross.enabled = true;
        setTime(false);
    }
}
