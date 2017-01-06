using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VictorySlider : MonoBehaviour {
    VictoriesManager vmanager;
    [SerializeField]
    Text victoriesIndicator;

    void Awake() {
        vmanager = VictoriesManager.getVictoriesManager();
        setText(1);
        setVictoriesNeeded();
    }

    public void setVictoriesNeeded() {
        int victoriesNeeded = (int) this.GetComponent<Slider>().value;
        vmanager.set_victories_needed(victoriesNeeded);
        setText(victoriesNeeded);
    }

    void setText(int vic) {
        victoriesIndicator.text = "vitórias necessárias: " + vic;
    }
}
