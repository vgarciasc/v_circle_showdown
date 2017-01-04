using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PlayerDatabase : MonoBehaviour {
    int currentID = 0;
    bool hasSeenAGame = false;
    PlayerDatabaseSpawner spawner;

    public int victoriesNeeded = 1;
    public int winnerID = -1;

    List<int> playersIn = new List<int>();
    public List<PlayerInstance> players = new List<PlayerInstance>();
    List<string> joystickNames = new List<string>();

    [SerializeField] 
    List<Color> playerColors = new List<Color>();
    List<Color> currentColorArray = new List<Color>();

    [SerializeField]
    Transform playerTexts; //textos que ficam ativos quando o joystick entra em jogo

    void Start() {
        deleteNewerCopies();
        initBaseColors();
        spawner = GetComponent<PlayerDatabaseSpawner>();

        joystickNames.Add("Keyboard");
        joystickNames.AddRange(Input.GetJoystickNames());

        foreach (Transform t in playerTexts.transform)
            t.gameObject.SetActive(false);
    }
    
	void Update () {
        if (!hasSeenAGame) handleInput();
	}

    #region Handle Victories
    public void resetVictories() {
        for (int i = 0; i < players.Count; i++)
            players[i].victories = 0;
    }

    public void giveVictoryTo(int playerID) {
        players[playerID].victories++;
    }

    public int getGameWinner() {
        for (int i = 0; i < players.Count; i++)
            if (players[i].victories >= victoriesNeeded)
                return i;

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

                string generated_ID_name = "Player #" + Random.Range(0, 200);
                PlayerInstance aux = new PlayerInstance("_J" + i.ToString(),
                                                i,
                                                currentID,
                                                generated_ID_name,
                                                getRandomPlayerColor(Color.clear));

                players.Add(aux);
                playersIn.Add(i);
            
                spawner.setPlayer(currentID, aux);
                spawner.activatePlayer(currentID);
                playerTexts.GetChild(currentID).gameObject.SetActive(true);
                playerTexts.GetChild(currentID).GetComponentInChildren<Text>().text = generated_ID_name + " has entered the game." +
                "\n <color=grey> (" + joystickNames[i] + ") </color>";
                // playerTexts.transform.GetChild(currentID).GetComponentInChildren<Image>().color = aux.color;

                currentID++;

            } else if (Input.GetButtonDown("Fire2_J" + i.ToString()) && playersIn.Contains(i)) {
                int player_ID = get_player_entry_ID(i);
                Color aux = getRandomPlayerColor(players[player_ID].color);
                players[player_ID].color = aux;
                spawner.setPlayer(player_ID, players[player_ID]);

            } else if (Input.GetButtonDown("Submit_J" + i.ToString()) && playersIn.Contains(i)) {
                int player_ID = get_player_entry_ID(i);
                toggleReady(player_ID);

            }
        }

        if (Input.GetKeyDown(KeyCode.L))
            goLevelSelect();
    }

    int get_player_entry_ID(int joystickNum) {
        for (int j = 0; j < players.Count; j++)
            if (players[j].joystickNum == joystickNum)
                return j;

        return 0;
    }

    int ready = 0;
    void toggleReady(int player_ID) {
        bool value = playerTexts.GetChild(player_ID).GetChild(2).gameObject.activeSelf;
        playerTexts.GetChild(player_ID).GetChild(2).gameObject.SetActive(!value);

        if (value) ready--;
        else ready++;

        if (ready >= players.Count)
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

    Color getRandomPlayerColor(Color currentColor) {
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
        if (currentColor == Color.clear)
            currentColorArray.Remove(output);
        else
            currentColorArray.Add(currentColor);

        if (currentColorArray.Count == 0)
            initBaseColors();
        return output;
    }

    #endregion
    #endregion
}
