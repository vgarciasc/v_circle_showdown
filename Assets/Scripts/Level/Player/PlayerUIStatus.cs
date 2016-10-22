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

    Coroutine countCoroutine;

    void Start() {
        setTime(false);
        unshowItem();
    }

    public void setUI(int playerID, SpriteRenderer playerSprite) {
        this.GetComponentsInChildren<Text>()[0].text = "Player #" + playerID.ToString();
        this.GetComponentsInChildren<Image>()[1].color = playerSprite.color;
    }

    #region Time Shenanigans
    public void setTime(float time) {
        this.time.enabled = true;
        this.time.text = time.ToString();
    }

    public void setTime(bool value) {
        this.time.enabled = value; }
    #endregion

    public void playerKilled() { 
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
