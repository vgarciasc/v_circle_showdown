﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using TMPro;

[System.Serializable]
public class PlayerColor : System.Object {
    public Color color;
    public Gradient gradient;
}

public class PlayerDatabase : MonoBehaviour {
    int currentID = 0;
    bool hasSeenAGame = false;
    PlayerDatabaseSpawner spawner;
    VictoriesManager vmanager;

    public bool titleScreen = false;

    List<int> joysticks_ingame = new List<int>();
    public List<PlayerInstance> players = new List<PlayerInstance>();
    List<string> joystick_names = new List<string>();

    public List<PlayerColor> original_colors_pool = new List<PlayerColor>();
    List<PlayerColor> current_colors_pool = new List<PlayerColor>();

    [SerializeField]
    Transform playerTexts; //textos que ficam ativos quando o joystick entra em jogo

    public static PlayerDatabase getPlayerDatabase() {
        return (PlayerDatabase) HushPuppy.safeFindComponent("PlayerDatabase", "PlayerDatabase");
    }

    void Start() {
        if (titleScreen) {
            return;
        }

        deleteNewerCopies();
        reset_available_colors();
        spawner = GetComponent<PlayerDatabaseSpawner>();
        vmanager = VictoriesManager.getVictoriesManager();

        init_joysticks();

        foreach (Transform t in playerTexts.transform)
            t.gameObject.SetActive(false);
    }
    
	void Update () {
        if (!hasSeenAGame && !titleScreen) handleInput();
	}

    #region Joysticks
    void init_joysticks() {
        joystick_names.Add("Keyboard");
        joystick_names.AddRange(Input.GetJoystickNames());
    }

    public int get_player_entry_ID(int joystickNum) {
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

                string generated_ID_name = generate_name();
                PlayerColor generated_color = get_first_random_color();

                PlayerInstance aux = new PlayerInstance("_J" + i.ToString(),
                                                i,
                                                currentID,
                                                generated_ID_name,
                                                generated_color);
                players.Add(aux);
                joysticks_ingame.Add(i);
            
                spawner.setPlayer(currentID, aux);
                spawner.activatePlayer(currentID);
                playerTexts.GetChild(currentID).gameObject.SetActive(true);
                playerTexts.GetChild(currentID).GetComponentInChildren<TextMeshProUGUI>().text = "<b>" + 
                    "<color=#" + HushPuppy.ColorToHex((Color32) generated_color.color) + ">" + 
                    generated_ID_name + "</color></b> has entered the game." +
                    "\n <color=#555555> (" + joystick_names[i] + ") </color>";

                currentID++;

            } else if (Input.GetButtonDown("Submit_J" + i.ToString()) && joysticks_ingame.Contains(i)) {
                //get another color and name
                int player_ID = get_player_entry_ID(i);
                if (getReady(player_ID)) {
                    playerTexts.GetChild(player_ID).GetComponentInChildren<TextMeshProUGUI>().text = "<b>" + 
                    "<color=#" + HushPuppy.ColorToHex((Color32) players[player_ID].palette.color) + ">" + 
                    players[player_ID].name + "</color></b> has <i>entered</i> the game." +
                    "\n <color=#555555> (" + joystick_names[i] + ") </color>";
                    return;
                }

                PlayerColor aux = get_another_random_color(players[player_ID].palette);
                string new_name = generate_name();
                players[player_ID].palette = aux;
                players[player_ID].name = new_name;
                
                spawner.setPlayer(player_ID, players[player_ID]);
                playerTexts.GetChild(player_ID).GetComponentInChildren<TextMeshProUGUI>().text = "<b>" + 
                    "<color=#" + HushPuppy.ColorToHex((Color32) aux.color) + ">" + 
                    new_name + "</color></b> has entered the game." +
                    "\n <color=#555555> (" + joystick_names[i] + ") </color>";
            }
        }
    }

    int ready = 0;
    List<bool> readyPlayers = new List<bool>(){false, false, false, false};
    public void toggleReady(int player_ID) {
        // bool value = playerTexts.GetChild(player_ID).GetChild(3).gameObject.activeSelf;
        // playerTexts.GetChild(player_ID).GetChild(3).gameObject.SetActive(!value);

        // if (value) ready--;
        // else ready++;

        // if (ready >= players.Count && players.Count > 1)
        //     goLevelSelect();
    }

    public IEnumerator setReady(int player_ID) {
        playerTexts.GetChild(player_ID).GetComponent<Animator>().SetTrigger("activate");

        if (!readyPlayers[player_ID]) {
            readyPlayers[player_ID] = true;
            ready++;
        }
        yield return new WaitForSeconds(1f);

        if (ready >= players.Count && players.Count > 1)
            goLevelSelect();
    }

    bool getReady(int player_ID) {
        return readyPlayers[player_ID];
    } 

    void goLevelSelect() {
        hasSeenAGame = true;
        DontDestroyOnLoad(this.gameObject);
        vmanager.set_players(currentID);
        SceneLoader.getSceneLoader().LevelSelect();
    }

    #region Random Color Generator
    void reset_available_colors() {
        current_colors_pool.AddRange(original_colors_pool);
    }

    PlayerColor get_first_random_color() {
        int i = Random.Range(0, current_colors_pool.Count);
        PlayerColor output = current_colors_pool[i];
        current_colors_pool.Remove(output);

        //pool of colors is empty. fill it with all original colors (may repeat colors)
        if (current_colors_pool.Count == 0) {
            reset_available_colors();
        }

        return output;
    }

    PlayerColor get_another_random_color(PlayerColor current_color) {
        int i = Random.Range(0, current_colors_pool.Count);
        PlayerColor output = current_colors_pool[i];
        current_colors_pool.Remove(output);
        current_colors_pool.Add(current_color);
        return output;
    }

    #endregion
    #endregion

    #region names
    string generate_name() {
         return "Player #" + Random.Range(0, 500);
    }

    public void generate_new_names() {
        for (int i = 0; i < players.Count; i++) {
            players[i].name = generate_name();
        }
    }
    #endregion

    #region player database access
    public Player get_player_by_ID(int ID) {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player")) {
            Player player = (Player) HushPuppy.safeComponent(go, "Player");
            if (player.ID == ID) {
                return player;
            }
        }

        return null;
    }

    #endregion
}
