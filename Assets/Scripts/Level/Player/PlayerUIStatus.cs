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

    Animator animator;
    Coroutine countCoroutine;
    PlayerDatabase pdatabase;
    Color playerColor;
    string playerName;
    int playerID;

    void Awake() {
        pdatabase = (PlayerDatabase) HushPuppy.safeFindComponent("PlayerDatabase", "PlayerDatabase");
        reset();
    }

    public void reset() {
        redCross.enabled = false;
        setTime(false);
        unshowItem();
    }

    public void setUI(string playerName, int playerID, Color playerColor) {
        this.playerColor = playerColor;
        this.playerID = playerID;
        this.playerName = playerName;
        this.animator = GetComponentInParent<Animator>();

        this.GetComponentsInChildren<Text>()[0].text = playerName.ToString();
        this.GetComponentsInChildren<Image>()[1].color = playerColor;
        if (pdatabase != null) setVictories(playerID);
    }

    #region Victories
    void setVictories(int playerID) {
        for (int i = 0; i < pdatabase.victoriesNeeded; i++) {
            GameObject aux = (GameObject) Instantiate(victoryIcon, victoriesContainer, false);
            if (pdatabase.players[playerID].victories > i)
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
    public void showItem(ItemData item) {
        carriedItem.enabled = true;
        carriedItem.sprite = item.sprite;
        animator.SetTrigger("get_item");
    }

    public void unshowItem() {
        carriedItem.enabled = false;
    }
    #endregion
}
