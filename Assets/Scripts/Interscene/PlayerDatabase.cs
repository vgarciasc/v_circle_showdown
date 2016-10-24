using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PlayerData {
    public string joystick; //formato "_Jk" para joystick de numero k
    public int joystickNum; //numero do joystick do jogador
    public int playerID; //ordem que o player apertou pra entrar no jogo
    public Color color; //sprite que ele escolheu pra jogar
    public int victories = 0;

    public PlayerData(string joystick, int joystickNum, int playerID, Color color) {
        this.joystick = joystick;
        this.joystickNum = joystickNum;
        this.playerID = playerID;
        this.color = color;
    }
}

public class PlayerDatabase : MonoBehaviour {
    int currentID = 0;
    bool hasSeenAGame = false;
    public List<PlayerData> pprefs = new List<PlayerData>();
    List<int> playersIn = new List<int>();

    public int victoriesNeeded = 1;
    public int winnerID = -1;

    [SerializeField] 
    List<Color> playerColors = new List<Color>();
    List<Color> currentColorArray = new List<Color>();

    [SerializeField]
    GameObject playerTexts; //textos que ficam ativos quando o joystick entra em jogo

    void Start() {
        deleteNewerCopies();
        initBaseColors();

        foreach (Transform t in playerTexts.transform)
            t.gameObject.SetActive(false);
    }
    
	void Update () {
        if (!hasSeenAGame) handleInput();
	}

    #region Handle Victories
    public void resetVictories() {
        for (int i = 0; i < pprefs.Count; i++)
            pprefs[i].victories = 0;
    }

    public void giveVictoryTo(int playerID) {
        pprefs[playerID].victories++;
    }

    public int getGameWinner() {
        for (int i = 0; i < pprefs.Count; i++) {
            if (pprefs[i].victories >= victoriesNeeded)
                return i;
        }

        return -1; //no winner yet
    }
    #endregion

    #region Player Select Screen
    void deleteNewerCopies() {
        if (!hasSeenAGame) return;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player Data"))
            if (go != this.gameObject) Destroy(go);
    }
        
    void handleInput() {
        for (int i = 0; i < 4; i++) {
            if (Input.GetButtonDown("Fire1_J" + i.ToString()) && !playersIn.Contains(i)) {
                Debug.Log("Joystick #" + i + " entrou no jogo!");

                PlayerData aux = new PlayerData("_J" + i.ToString(),
                                                i,
                                                currentID,
                                                getRandomPlayerColor(true));

                pprefs.Add(aux);
                playersIn.Add(i);

                playerTexts.transform.GetChild(currentID).gameObject.SetActive(true);
                playerTexts.transform.GetChild(currentID).GetComponent<Text>().text = "Player #" + (currentID + 1) + " has entered the game.";
                playerTexts.transform.GetChild(currentID).GetComponentInChildren<Image>().color = aux.color;

                currentID++;
            } else if (Input.GetButtonDown("Fire1_J" + i.ToString()) && playersIn.Contains(i)) {
                for (int j = 0; j < pprefs.Count; j++) {
                    if (pprefs[j].joystickNum == i) {
                        Color aux = getRandomPlayerColor(false);
                        pprefs[j].color = aux;
                        playerTexts.transform.GetChild(pprefs[j].playerID).GetComponentInChildren<Image>().color = aux;
                        break;
                    }
                }
            }
        }

        if (Input.GetButton("Submit"))
            goLevelSelect();
    }

    void goLevelSelect() {
        resetVictories();
        DontDestroyOnLoad(this.gameObject);
        SceneManager.LoadScene("LevelSelect");
        hasSeenAGame = true;
    }

    #region Random Color Generator
    void initBaseColors() {
        //baseColors = new Dictionary<Color, bool>{
        //                { Color.green, false },
        //                { Color.gray, false },
        //                { Color.cyan, false },
        //                { Color.blue, false },
        //                { Color.red, false },
        //                { Color.magenta, false },
        //                { Color.yellow, false } };

        /*baseColors = new List<Color> {
            Color.green, Color.yellow,
            Color.cyan, Color.blue,
            Color.red, Color.magenta};*/

        currentColorArray.AddRange(playerColors);
    }

    Color getRandomPlayerColor(bool firstTime) {
        int i = Random.Range(0, currentColorArray.Count);
        /*Color aux = baseColors[i];
        float aux_r, aux_g, aux_b;
        float saturation = Random.Range(0.2f, 0.4f);

        aux_r = aux.r += (saturation * -Mathf.Sign(aux.r - 0.5f));
        aux_g = aux.g += (saturation * -Mathf.Sign(aux.g - 0.5f));
        aux_b = aux.b += (saturation * -Mathf.Sign(aux.b - 0.5f));

        Color output = new Color(aux_r, aux_g, aux_b);
        baseColors.RemoveAt(i);
        if (baseColors.Count == 0) {
            initBaseColors();
        }
        return output;*/

        Color output = currentColorArray[i];
        currentColorArray.Remove(output);
        if (firstTime)
            playerColors.Remove(output);

        if (currentColorArray.Count == 0)
            initBaseColors();
        return output;
    }

    #endregion
    #endregion
}
