using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour {
    public static SceneLoader getSceneLoader() {
        return (SceneLoader) HushPuppy.safeFindComponent("PlayerDatabase", "SceneLoader");
    }
    
    public void LoadScene(string name) {
        SceneManager.LoadScene(name);
    }

    public void ButtonLoadLevel(string ID) {
        getSceneLoader().LoadLevel(ID);
    }

    public void LoadLevel(string ID) {
        if (ID == "Random") {
            LoadRandomLevel();
            return;
        }
        
        ShowdownPanelAnimation.EnteringNewLevel();

        string level_name = "Level" + ID;
        VictoriesManager.getVictoriesManager().reset_victories();

        StartCoroutine(delayThenLoad(level_name));
    }

    public void LoadRandomLevel() {
        ShowdownPanelAnimation.EnteringNewLevel();

        string level_name = "Level" + Random.Range(0, 7);
        VictoriesManager.getVictoriesManager().reset_victories();

        StartCoroutine(delayThenLoad(level_name));
    }

    IEnumerator delayThenLoad(string name) {
        LevelSoundManager.getLevelSoundManager().fadeOut(1f);

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(name);
    }

    public void ResetLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameOver() {
        StartCoroutine(delayThenLoad("GameOver v2"));
    }

    public void LevelSelect() {
        SceneManager.LoadScene("LevelSelect 1");
        VictoriesManager.getVictoriesManager().reset_victories();
    }

    public void RestartGame() {
        PlayerSelect();
    }

    public void PlayerSelect() {
        SceneManager.LoadScene("PlayerSelect");
    }

    public void GameOverLevelSelect() {
        PlayerDatabase.getPlayerDatabase().generate_new_names();
        LevelSelect();
    }
}
