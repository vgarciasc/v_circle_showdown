using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour {
    [Header("Prefabs & References")]
    [SerializeField]
    GameObject playerPrefab;

    GameObject playerSpawnLocationManager;

    void Start() {
        playerSpawnLocationManager = HushPuppy.findGameObject("Player Spawn Location Manager");

        GameObject playerDataObject = HushPuppy.findGameObject("Player Data");

        if (playerDataObject == null) {
            startDefaultGame(); return;
        }

        PlayerDatabase playerData = playerDataObject.GetComponent<PlayerDatabase>();

        if (playerSpawnLocationManager.transform.childCount < playerData.pprefs.Count) {
            Debug.Log("Not enough spawn points!"); return;
        }

        for (int i = 0; i < playerData.pprefs.Count; i++)
            spawnPlayer(playerData.pprefs[i],
                        playerSpawnLocationManager.transform.GetChild(playerData.pprefs[i].playerID));
    }

    Player spawnPlayer(PlayerData data, Transform location) {
        Player aux = Instantiate(playerPrefab).GetComponent<Player>();
        aux.setPlayer(data.playerID, data.joystick, data.color);
        aux.transform.position = location.position;
        aux.name = "Player #" + (data.playerID + 1);

        return aux;
    }

    void startDefaultGame() {
        PlayerData aux = new PlayerData("_J0", 0, 0, Color.white);
        spawnPlayer(aux, playerSpawnLocationManager.transform.GetChild(0));
    }
}
