using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpecialMoves : MonoBehaviour, IObserver {
    [SerializeField]
    Text mainText;

    public void onNotify(Event ev) {
        switch (ev) {
            case Event.PLAYER_KILLED:
                StartCoroutine(showcaseText("PLAYER KILLED"));
                break;
        }
    }

    IEnumerator showcaseText(string text) {
        mainText.text = text;
        mainText.color = HushPuppy.getColorWithOpacity(mainText.color, 1f);
        yield return new WaitForSeconds(1f);
        mainText.CrossFadeAlpha(0f, 1f, false);
    }
}
