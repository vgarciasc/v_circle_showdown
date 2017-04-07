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
        ShowdownPanelAnimation.EnteringNewLevel();
        
        string level_name = "Level" + ID;
        SceneManager.LoadScene(level_name);
        VictoriesManager.getVictoriesManager().reset_victories();
    }

    public void ResetLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GameOver() {
        SceneManager.LoadScene("GameOver v2");
    }

    public void LevelSelect() {
        SceneManager.LoadScene("LevelSelect 1");
        VictoriesManager.getVictoriesManager().reset_victories();
    }

    public void GameOverLevelSelect() {
        PlayerDatabase.getPlayerDatabase().generate_new_names();
        LevelSelect();
    }
}
