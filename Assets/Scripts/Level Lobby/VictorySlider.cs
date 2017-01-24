using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class VictorySlider : MonoBehaviour {
    VictoriesManager vmanager;
    [SerializeField]
    Text victoriesIndicatorNumber;
    [SerializeField]
    Text victoriesIndicatorText;

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
        victoriesIndicatorNumber.text = vic.ToString();
        if (vic > 1) {
            victoriesIndicatorText.text = "ROUNDS";
        }
        else {
            victoriesIndicatorText.text = "ROUND";
        }
    }
}
