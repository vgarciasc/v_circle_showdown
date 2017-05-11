using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour {
    PlayerDatabase pdatabase;
    VictoriesManager vmanager;

    void Start() {
        pdatabase = PlayerDatabase.getPlayerDatabase();
        vmanager = VictoriesManager.getVictoriesManager();

        PlayerSpawner.getPlayerSpawner().spawnAllPlayers();

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Player")) {
            Player player = (Player) HushPuppy.safeComponent(go, "Player");
            player.death_event += checkGameOver;
        }
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.L)) {
            SceneLoader.getSceneLoader().LevelSelect();
        }
    }

    bool got_victory = false;

    public void checkGameOver() {
        StartCoroutine(checkGameOver_());
    }

    IEnumerator checkGameOver_() {
        yield return PauseManager.getPauseManager().WaitForSecondsInterruptable(2.0f);

        int match_winner = vmanager.get_match_winner();
        if (match_winner >= 0) {
            Player winning_player = pdatabase.get_player_by_ID(match_winner);
			winning_player.get_victory();
			
			yield return PauseManager.getPauseManager().WaitForSecondsInterruptable(2.0f);
            get_next_scene(match_winner);
        }
        else if (match_winner == -2) { //empate
			yield return PauseManager.getPauseManager().WaitForSecondsInterruptable(2.0f);
            get_next_scene(match_winner);
        }
    }

    void get_next_scene(int match_winner) {
        if (got_victory) {
            return;
        }
        
        if (match_winner == -2) { //empate
            SceneLoader.getSceneLoader().ResetLevel();
            return;
        }

        got_victory = true;
        vmanager.give_victory(match_winner);
        int game_winner = vmanager.get_game_winner();

        if (game_winner == -1) {
            //jogador ganhou partida mas nao o jogo
            StartCoroutine(wait_transition_reset_level());
        } else {
            //jogador ganhou o jogo
            StartCoroutine(wait_transition_game_over(game_winner));
        }
    }

    IEnumerator wait_transition_reset_level() {
        ScreenTransitionAnimation screenTransition = ScreenTransitionAnimation.getScreenTransitionAnimation();
        
        screenTransition.start_animation(Color.clear);

        yield return new WaitUntil(() => screenTransition.transition_show_ended);

        SceneLoader.getSceneLoader().ResetLevel();
    }

    IEnumerator wait_transition_game_over(int game_winner_ID) {
        Color aux = PlayerDatabase.getPlayerDatabase().players[game_winner_ID].palette.color;
        ScreenTransitionAnimation screenTransition = ScreenTransitionAnimation.getScreenTransitionAnimation();

        screenTransition.start_animation(aux);

        yield return new WaitUntil(() => screenTransition.transition_show_ended);

        SceneLoader.getSceneLoader().GameOver();
    }
}
