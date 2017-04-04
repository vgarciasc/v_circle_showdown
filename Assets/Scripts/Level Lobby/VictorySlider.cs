using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class VictorySlider : MonoBehaviour {
    VictoriesManager vmanager;
    [SerializeField]
    TextMeshProUGUI victoriesIndicatorNumber;
    [SerializeField]
    TextMeshProUGUI victoriesIndicatorText;

    void Awake() {
        vmanager = VictoriesManager.getVictoriesManager();
        
        int cookies = PlayerPrefs.GetInt("Victories Needed");
        setText(cookies);
        this.GetComponent<Slider>().value = cookies;

        setVictoriesNeeded();
    }

    public void setVictoriesNeeded() {
        int victoriesNeeded = (int) this.GetComponent<Slider>().value;
        vmanager.set_victories_needed(victoriesNeeded);
        PlayerPrefs.SetInt("Victories Needed", victoriesNeeded);
        setText(victoriesNeeded);
    }

    void setText(int vic) {
        victoriesIndicatorNumber.text = vic.ToString();
        if (vic > 1) {
            victoriesIndicatorText.text = "rounds";
        }
        else {
            victoriesIndicatorText.text = "round";
        }
    }
}
