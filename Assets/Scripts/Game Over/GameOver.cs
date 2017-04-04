using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;
using TMPro;

public class GameOver : MonoBehaviour {
    VictoriesManager vmanager;
    PlayerDatabase pdatabase;
    PlayerSpawner pspawner;

    [Header("References")]
    [SerializeField]
    TextMeshProUGUI player_congratulations;
    [SerializeField]
    Player player_winner_surrogate;
    [SerializeField]
    Transform player_stats_container;
    [SerializeField]
    GameObject player_stats_prefab;
    [SerializeField]
    GameObject smash_platform;
    [SerializeField]
    GameObject press_start;
    [SerializeField]
    GameOverWinnerStats gowstats; 
    [SerializeField]
    ParticleSystem confetti;

    bool can_end_scene = false; 
    PlayerInstance player_winner;

    void Start () {
        vmanager = VictoriesManager.getVictoriesManager();
        pdatabase = PlayerDatabase.getPlayerDatabase();
        //pspawner = PlayerSpawner.getPlayerSpawner();

        int game_winner = vmanager.get_game_winner();
        if (game_winner == -1) {
            //set_tie();
        }
        else {
            StartCoroutine(set_win(game_winner));
        }
    }
	
    bool end_scene_called = false;
	void Update () {
        if (player_winner != null && 
            Input.GetButtonDown("Submit_J" + player_winner.joystickNum)) {
            if (!end_scene_called && can_end_scene) {
                end_scene_called = true;
                StartCoroutine(end_scene());
            }
        }
	}

    bool can_spawn_next_stats = true;
    IEnumerator set_win(int winner_ID) {
        player_winner = pdatabase.players[winner_ID];
        player_winner_surrogate.setPlayer(player_winner);
        player_winner_surrogate.gameObject.SetActive(true);

        player_congratulations.text = player_winner.name.ToUpper() + 
                                      " \"WON THE GAME\".";
        
        yield return new WaitForSeconds(2.0f);

        // playerSprite.color = player_winner.color;
        // vmanager.reset_victories();

        gowstats.set(player_winner);
        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < pdatabase.players.Count; i++) {
            if (pdatabase.players[i].playerID == winner_ID) {
                continue;
            }

            GameObject aux = Instantiate(player_stats_prefab, player_stats_container, false);
            GameOverPlayerStats gops = aux.GetComponent<GameOverPlayerStats>();
            gops.set(pdatabase.players[i]);
            gops.animation_ended_event += spawn_next_stats;

            yield return new WaitUntil(() => can_spawn_next_stats);
            gops.GetComponent<Animator>().SetTrigger("show");
            can_spawn_next_stats = false;
        }

        can_end_scene = true;
        press_start.GetComponent<Animator>().SetBool("blink", true);
    }

    void spawn_next_stats() {
        can_spawn_next_stats = true;
    }

    IEnumerator end_scene() {
        if (confetti.isPlaying) {
            confetti.Stop();
        }

        press_start.SetActive(false);
        smash_platform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        yield return new WaitForSeconds(4.0f);
        SceneLoader.getSceneLoader().GameOverLevelSelect();
    }
}
