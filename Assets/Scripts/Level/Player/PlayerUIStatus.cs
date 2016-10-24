using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerUIStatus : MonoBehaviour {
    [Header("References")]
    [SerializeField]
    Image redCross;
    [SerializeField]
    Image carriedItem;
    [SerializeField]
    Text time;
    [SerializeField]
    Transform victoriesContainer;

    [Header("Prefabs")]
    [SerializeField]
    GameObject victoryIcon;

    Coroutine countCoroutine;
    PlayerDatabase pdatabase;
    Color playerColor;
    int playerID;

    void Awake() {
        pdatabase = (PlayerDatabase) HushPuppy.safeFindComponent("PlayerDatabase", "PlayerDatabase");
        setTime(false);
        unshowItem();
    }

    public void setUI(int playerID, SpriteRenderer playerSprite) {
        this.playerColor = playerSprite.color;
        this.playerID = playerID;

        this.GetComponentsInChildren<Text>()[0].text = "Player #" + (playerID + 1).ToString();
        this.GetComponentsInChildren<Image>()[1].color = playerColor;
        if (pdatabase != null) setVictories(playerID);
    }

    #region Victories
    void setVictories(int playerID) {
        for (int i = 0; i < pdatabase.victoriesNeeded; i++) {
            GameObject aux = (GameObject) Instantiate(victoryIcon, victoriesContainer, false);
            if (pdatabase.pprefs[playerID].victories > i)
                aux.GetComponent<Image>().color = playerColor;
            else
                aux.GetComponent<Image>().color = HushPuppy.getColorWithOpacity(aux.GetComponent<Image>().color, 0.5f);
        }
    }
    #endregion

    #region Time Shenanigans
    public void setTime(float time) {
        this.time.enabled = true;
        this.time.text = time.ToString();
    }

    public void setTime(bool value) {
        this.time.enabled = value; }
    #endregion

    public void playerKilled() {
        unshowItem();
        redCross.enabled = true;
        setTime(false);
    }

    #region Item Hsneanigans
    public void showItem(Item item) {
        carriedItem.enabled = true;
        carriedItem.sprite = item.getSprite();
    }

    public void unshowItem() {
        carriedItem.enabled = false;
    }
    #endregion
}
