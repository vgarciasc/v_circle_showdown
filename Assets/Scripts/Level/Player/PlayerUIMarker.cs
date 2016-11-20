using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerUIMarker : MonoBehaviour, IObserver {
    RectTransform canvasRt;
    Vector3 originalScale;
    enum Border { None, Right, Left, Top, Bottom };

    void Awake() {
        originalScale = this.transform.localScale;
    }

    public void onNotify(Event ev) {
        switch (ev) {
            case Event.PLAYER_KILLED:
                StartCoroutine(playerKilled());
                break;
        }
    }

    public void setMarker(Color playerColor) {
        this.GetComponent<Image>().color = playerColor;
    }

    IEnumerator playerKilled() {
        yield return new WaitForSeconds(1.0f);
        HushPuppy.fadeImgOut(this.gameObject, 0.5f);
    }

    public void setPosition(Vector3 playerPos) {
        if (canvasRt == null) getCanvasRect();

        Vector2 screenPos = Camera.main.WorldToScreenPoint(playerPos);
        Vector2 viewportPos = Camera.main.WorldToViewportPoint(playerPos);
        this.transform.position = new Vector2(screenPos.x, screenPos.y);
        placeBorder(outOfScreen(viewportPos), distanceFromScreen(viewportPos));
    }

    void getCanvasRect() {
        canvasRt = (RectTransform) HushPuppy.safeFindComponent("Canvas", "RectTransform");
    }

    Border outOfScreen(Vector2 pos) {
        if (pos.x < 0) return Border.Left;
        else if (pos.x > 1) return Border.Right;
        else if (pos.y < 0) return Border.Bottom;
        else if (pos.y > 1) return Border.Top;
        return Border.None;
    }

    float distanceFromScreen(Vector2 pos) {
        if (pos.x < 0) return -pos.x;
        else if (pos.x > 1) return pos.x - 1;
        else if (pos.y < 0) return -pos.y;
        else if (pos.y > 1) return pos.y - 1;
        else return -1;
    }

    void placeBorder(Border pp, float distance) {
        Vector2 pos = this.transform.localPosition;
        float posx = pos.x; float posy = pos.y;
        if (posx < - canvasRt.sizeDelta.x / 2) posx = - canvasRt.sizeDelta.x / 2;
        if (posx > canvasRt.sizeDelta.x / 2) posx = canvasRt.sizeDelta.x / 2;
        if (posy < - canvasRt.sizeDelta.y / 2) posy = - canvasRt.sizeDelta.y / 2;
        if (posy > canvasRt.sizeDelta.y / 2) posy = canvasRt.sizeDelta.y / 2;
        pos = new Vector2(posx, posy);

        float sizeModifier = (-0.6f * distance + 1);
        this.transform.localScale = originalScale * sizeModifier;
        this.transform.localScale = new Vector2(Mathf.Clamp(this.transform.localScale.x, 0.2f, 1f),
                                                Mathf.Clamp(this.transform.localScale.y, 0.2f, 1f));

        switch (pp) {
            case Border.Right:
                toggleMarker(true);
                this.transform.localEulerAngles = new Vector3(0, 0, 270f);
                this.transform.localPosition = new Vector2(canvasRt.sizeDelta.x / 2, pos.y);
                break;
            case Border.Left:
                toggleMarker(true);
                this.transform.localEulerAngles = new Vector3(0, 0, 90f);
                this.transform.localPosition = new Vector2(-canvasRt.sizeDelta.x / 2, pos.y);
                break;
            case Border.Top:
                toggleMarker(true);
                this.transform.localEulerAngles = new Vector3(0, 0, 0f);
                this.transform.localPosition = new Vector2(pos.x, canvasRt.sizeDelta.y / 2);
                break;
            case Border.Bottom:
                toggleMarker(true);
                this.transform.localEulerAngles = new Vector3(0, 0, 180f);
                this.transform.localPosition = new Vector2(pos.x, - canvasRt.sizeDelta.y / 2);
                break;
            case Border.None:
                toggleMarker(false);
                break;
        }
    }

    void toggleMarker(bool value) {
        this.GetComponent<Image>().enabled = value;
    }
}
