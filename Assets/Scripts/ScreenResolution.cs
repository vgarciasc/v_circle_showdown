using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenResolution : MonoBehaviour {
    [Header("Aspect Ratio"), SerializeField]
    private float aspectWidth;
    [SerializeField]
    private float aspectHeight;
    [Header("Minimum Resolution"), SerializeField]
    private int minWidth;
    [SerializeField]
    private int minHeight;
    
    /// <summary>
    /// Usada para conferir se a resolucao foi modificada desde o ultimo frame.
    /// Permite que so rode o codigo de SetResolution quando o usuario escalar a janela.
    /// </summary>
    private float pastWidth, pastHeight;

    //Setta a resolucao minima pra tela nao ficar pequena demais
    void minResolution(int width, int height) {
        if (Screen.width < width || Screen.height < height) {
            Screen.SetResolution(width, height, Screen.fullScreen, 0);
            pastWidth = Screen.width;
            pastHeight = Screen.height;
        }
    }

    void Start () {
        pastWidth = Screen.width;
        pastHeight = Screen.height;

        if ((float) Screen.width / (float) Screen.height > aspectWidth / aspectHeight) {
            var heightAccordingToWidth = (float) Screen.width / aspectWidth * aspectHeight;
            Screen.SetResolution(Screen.width, (int) Mathf.Round(heightAccordingToWidth), Screen.fullScreen, 0);
        }
        else {
            var widthAccordingToHeight = (float) Screen.height / aspectHeight * aspectWidth;
            Screen.SetResolution((int) Mathf.Round(widthAccordingToHeight), Screen.height, Screen.fullScreen, 0);
        }

        minResolution(minWidth, minHeight);
        DontDestroyOnLoad(this.gameObject);
    }

    void Update () {
        if ((float) Screen.width / (float) Screen.height != aspectWidth / aspectHeight) {
            float heightAccordingToWidth;
            float widthAccordingToHeight;

            if (Screen.width != pastWidth) {
                heightAccordingToWidth = (float) Screen.width / aspectWidth * aspectHeight;
                Screen.SetResolution(Screen.width, (int) Mathf.Round(heightAccordingToWidth), Screen.fullScreen, 0);
            }
            else if (Screen.height != pastHeight) {
                widthAccordingToHeight = (float) Screen.height / aspectHeight * aspectWidth;
                Screen.SetResolution((int) Mathf.Round(widthAccordingToHeight), Screen.height, Screen.fullScreen, 0);
            }
            else {
                heightAccordingToWidth = (float)Screen.width / aspectWidth * aspectHeight;
                Screen.SetResolution(Screen.width, (int)Mathf.Round(heightAccordingToWidth), Screen.fullScreen, 0);
            }
            pastWidth = Screen.width;
            pastHeight = Screen.height;
        }

        minResolution(minWidth, minHeight);
    }
}
