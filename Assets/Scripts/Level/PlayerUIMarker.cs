using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerUIMarker : MonoBehaviour {
    RectTransform canvasRt;
    enum Border { None, Right, Left, Top, Bottom };

    void Start() {
        canvasRt = HushPuppy.findGameObject("Canvas").GetComponent<RectTransform>();
    }

    public void setMarker(Color playerColor) {
        this.GetComponent<Image>().color = playerColor;
    }

    public void setPosition(Vector3 playerPos) {
        if (canvasRt == null)
            canvasRt = HushPuppy.findGameObject("Canvas").GetComponent<RectTransform>();

        Vector2 screenPos = Camera.main.WorldToScreenPoint(playerPos);
        Vector2 viewportPos = Camera.main.WorldToViewportPoint(playerPos);
        this.transform.position = new Vector2(screenPos.x, screenPos.y);
        placeBorder(outOfScreen(viewportPos));
    }

    Border outOfScreen(Vector2 pos) {
        if (pos.x < 0) return Border.Left;
        else if (pos.x > 1) return Border.Right;
        else if (pos.y > 1) return Border.Top;
        else if (pos.y < 0) return Border.Bottom;
        return Border.None;
    }

    void placeBorder(Border pp) {
        Vector2 pos = this.transform.localPosition;
        float posx = pos.x; float posy = pos.y;
        if (posx < - canvasRt.sizeDelta.x / 2) posx = - canvasRt.sizeDelta.x / 2;
        if (posx > canvasRt.sizeDelta.x / 2) posx = canvasRt.sizeDelta.x / 2;
        if (posy < - canvasRt.sizeDelta.y / 2) posy = - canvasRt.sizeDelta.y / 2;
        if (posy > canvasRt.sizeDelta.y / 2) posy = canvasRt.sizeDelta.y / 2;
        pos = new Vector2(posx, posy);

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
