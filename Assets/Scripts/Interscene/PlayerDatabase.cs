using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PlayerDatabase : MonoBehaviour {
    int currentID = 0;
    bool hasSeenAGame = false;
    PlayerDatabaseSpawner spawner;
    VictoriesManager vmanager;

    List<int> joysticks_ingame = new List<int>();
    public List<PlayerInstance> players = new List<PlayerInstance>();
    List<string> joystick_names = new List<string>();

    [SerializeField] 
    List<Color> original_color_pool = new List<Color>();
    List<Color> current_color_pool = new List<Color>();

    [SerializeField]
    Transform playerTexts; //textos que ficam ativos quando o joystick entra em jogo

    public static PlayerDatabase getPlayerDatabase() {
        return (PlayerDatabase) HushPuppy.safeFindComponent("PlayerDatabase", "PlayerDatabase");
    }

    void Start() {
        deleteNewerCopies();
        reset_available_colors();
        spawner = GetComponent<PlayerDatabaseSpawner>();
        vmanager = VictoriesManager.getVictoriesManager();

        init_joysticks();

        foreach (Transform t in playerTexts.transform)
            t.gameObject.SetActive(false);
    }
    
	void Update () {
        if (!hasSeenAGame) handleInput();
	}

    #region Joysticks
    void init_joysticks() {
        joystick_names.Add("Keyboard");
        joystick_names.AddRange(Input.GetJoystickNames());
    }

    int get_player_entry_ID(int joystickNum) {
        for (int j = 0; j < players.Count; j++)
            if (players[j].joystickNum == joystickNum)
                return j;

        return 0;
    }
    #endregion

    #region Player Select Screen
    void deleteNewerCopies() {
        // if (!hasSeenAGame) return;
        // foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player Data"))
        //     if (go != this.gameObject) Destroy(go);
    }
        
    void handleInput() {
        for (int i = 0; i < 4; i++) {
            if (Input.GetButtonDown("Fire1_J" + i.ToString()) && !joysticks_ingame.Contains(i)) {
                //player joins the game

                string generated_ID_name = "Player #" + Random.Range(0, 200);
                PlayerInstance aux = new PlayerInstance("_J" + i.ToString(),
                                                i,
                                                currentID,
                                                generated_ID_name,
                                                get_first_random_color());
                players.Add(aux);
                joysticks_ingame.Add(i);
            
                spawner.setPlayer(currentID, aux);
                spawner.activatePlayer(currentID);
                playerTexts.GetChild(currentID).gameObject.SetActive(true);
                playerTexts.GetChild(currentID).GetComponentInChildren<Text>().text = generated_ID_name + " has entered the game." +
                "\n <color=grey> (" + joystick_names[i] + ") </color>";

                currentID++;

            } else if (Input.GetButtonDown("Fire2_J" + i.ToString()) && joysticks_ingame.Contains(i)) {
                //get another color
                int player_ID = get_player_entry_ID(i);
                Color aux = get_another_random_color(players[player_ID].color);
                players[player_ID].color = aux;
                spawner.setPlayer(player_ID, players[player_ID]);

            } else if (Input.GetButtonDown("Submit_J" + i.ToString()) && joysticks_ingame.Contains(i)) {
                //player is ready
                int player_ID = get_player_entry_ID(i);
                toggleReady(player_ID);
            }
        }
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
        hasSeenAGame = true;
        DontDestroyOnLoad(this.gameObject);
        vmanager.set_players(currentID);
        SceneLoader.getSceneLoader().LevelSelect();
    }

    #region Random Color Generator
    void reset_available_colors() {
        current_color_pool.AddRange(original_color_pool);
    }

    Color get_first_random_color() {
        int i = Random.Range(0, current_color_pool.Count);
        Color output = current_color_pool[i];
        current_color_pool.Remove(output);

        //pool of colors is empty. fill it with all original colors (may repeat colors)
        if (current_color_pool.Count == 0) {
            reset_available_colors();
        }

        return output;
    }

    Color get_another_random_color(Color current_color) {
        int i = Random.Range(0, current_color_pool.Count);
        Color output = current_color_pool[i];
        current_color_pool.Remove(output);
        current_color_pool.Add(current_color);
        return output;
    }

    #endregion
    #endregion
}
