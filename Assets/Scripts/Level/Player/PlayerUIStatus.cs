using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class PlayerUIStatus : MonoBehaviour {
    [Header("References")]
    [SerializeField]
    Image redCross;
    [SerializeField]
    Image carriedItem;
    [SerializeField]
    Transform victoriesContainer;

    [Header("Prefabs")]
    [SerializeField]
    GameObject victoryIcon;

    Animator animator;
    Coroutine countCoroutine;
    PlayerDatabase pdatabase;
    VictoriesManager vmanager;
    Color playerColor;
    string playerName;
    int playerID;

    void Awake() {
        pdatabase = PlayerDatabase.getPlayerDatabase();
        vmanager = VictoriesManager.getVictoriesManager();
        reset();
    }

    public void reset() {
        redCross.enabled = false;
        unshowItem();
    }

    public void setUI(string playerName, int playerID, Color playerColor) {
        this.playerColor = playerColor;
        this.playerID = playerID;
        this.playerName = playerName;
        this.animator = GetComponentInParent<Animator>();

        this.GetComponentsInChildren<TextMeshProUGUI>()[0].text = "<color=#" + 
            HushPuppy.ColorToHex(playerColor) + ">" +
            playerName.ToString() + "</color>";
        this.GetComponentsInChildren<Image>()[1].color = playerColor;
        if (pdatabase != null) setVictories();
    }

    #region Victories
    void setVictories() {
        for (int i = 0; i < vmanager.get_victories_needed(); i++) {
            GameObject aux = (GameObject) Instantiate(victoryIcon, victoriesContainer, false);
            if (vmanager.get_player_victories(playerID) > i) {
                aux.GetComponent<Image>().color = playerColor;
            }
            else {
                aux.GetComponent<Image>().color = HushPuppy.getColorWithOpacity(aux.GetComponent<Image>().color, 0.7f);
            }
        }
    }

    public void get_victory() { StartCoroutine(get_victory_()); }
    IEnumerator get_victory_() {
        animator.SetTrigger("popup");
        yield return new WaitForSeconds(0.15f);

        GameObject victory_icon = victoriesContainer.GetChild(vmanager.get_player_victories(playerID)).gameObject;
        victory_icon.GetComponent<Image>().color = playerColor;
        victory_icon.GetComponent<Animator>().SetTrigger("show");
    }
    #endregion

    public void playerKilled() {
        unshowItem();
        redCross.enabled = true;
    }

    #region Item Hsneanigans
    public void showItem(ItemData item) {
        if (carriedItem.sprite.name == item.sprite.name) {
            return;
        }
        carriedItem.enabled = true;
        carriedItem.sprite = item.sprite;
        animator.SetTrigger("get_item");
    }

    public void unshowItem() {
        carriedItem.enabled = false;
    }
    #endregion
}
