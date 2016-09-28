using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PlayerDatabase : MonoBehaviour {
    int currentID = 0;
    bool hasSeenAGame = false;
    public List<PlayerPrefs> pprefs = new List<PlayerPrefs>();
    List<int> playersIn = new List<int>();
    public struct PlayerPrefs {
        public string joystick; //formato "_Jk" para joystick de numero k
        public int playerID; //ordem que o player apertou pra entrar no jogo
        public Color color; //sprite que ele escolheu pra jogar
    }

    //DELETAR E FAZER ALGO MAIS BONITO
    public GameObject playerTexts;

    void Start() {
        deleteNewerCopies();
        foreach (Transform t in playerTexts.transform)
            t.gameObject.SetActive(false);
    }
    
	void Update () {
        handleInput();
	}

    void deleteNewerCopies() {
        if (!hasSeenAGame) return;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player Data"))
            if (go != this.gameObject) Destroy(go);
    }

    void handleInput() {
        for (int i = 0; i < 4; i++) {
            if (Input.GetButtonDown("Fire1_J" + i.ToString()) && !playersIn.Contains(i)) {
                Debug.Log("Joystick #" + i + " entrou no jogo!");

                PlayerPrefs aux = new PlayerPrefs();
                aux.joystick = "_J" + i.ToString();
                aux.playerID = currentID;
                aux.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

                pprefs.Add(aux);
                playersIn.Add(i);

                playerTexts.transform.GetChild(currentID).gameObject.SetActive(true);
                playerTexts.transform.GetChild(currentID).GetComponent<Text>().text = "Player #" + currentID + " has entered the game.";
                playerTexts.transform.GetChild(currentID).GetComponentInChildren<Image>().color = aux.color;

                currentID++;
            }
        }

        if (Input.GetButton("Submit")) {
            goLevelSelect();
        }

        for (int i = 0; i < 4; i++) {
            if (Input.GetButtonDown("Fire1_J" + i.ToString()) && playersIn.Contains(i)) {

            }
        }
    }

    void goLevelSelect() {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.LoadScene("LevelSelect");
        hasSeenAGame = true;
    }
}
