using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour {
    [Header("Prefabs & References")]
    [SerializeField]
    GameObject playerPrefab;

    GameObject playerSpawnLocationManager;

    void Start() {
        playerSpawnLocationManager = HushPuppy.findGameObject("Player Spawn Location Manager");
        PlayerDatabase playerData = HushPuppy.findGameObject("Player Data").GetComponent<PlayerDatabase>();

        if (playerSpawnLocationManager.transform.childCount < playerData.pprefs.Count)
            Debug.Log("Not enough spawn points!");

        for (int i = 0; i < playerData.pprefs.Count; i++)
            spawnPlayer(playerData.pprefs[i]);
    }

    void spawnPlayer(PlayerDatabase.PlayerPrefs data) {
        Player aux = Instantiate(playerPrefab).GetComponent<Player>();
        aux.setPlayer(data.playerID, data.joystick, data.color);
        Transform spawnLocation = playerSpawnLocationManager.transform.GetChild(data.playerID);
        aux.transform.position = spawnLocation.position;
    }
}
