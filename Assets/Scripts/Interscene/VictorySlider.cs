using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VictorySlider : MonoBehaviour {
    PlayerDatabase pdatabase;
    [SerializeField]
    Text victoriesIndicator;

    void Awake() {
        pdatabase = (PlayerDatabase) HushPuppy.safeFindComponent("PlayerDatabase", "PlayerDatabase");
        setText(1);
    }

    public void setVictoriesNeeded() {
        int victoriesNeeded = (int) this.GetComponent<Slider>().value;
        pdatabase.victoriesNeeded = victoriesNeeded;
        setText(victoriesNeeded);
    }

    void setText(int vic) {
        victoriesIndicator.text = "vitórias necessárias: " + vic;
    }
}
