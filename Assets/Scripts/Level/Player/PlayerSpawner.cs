using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour {
    [Header("Prefabs & References")]
    [SerializeField]
    GameObject playerPrefab;

    SpawnLocations playerSpawnLocations;
    PlayerDatabase playerDatabase;

    void Start() {
        playerSpawnLocations = (SpawnLocations) HushPuppy.safeFindComponent("PlayerSpawnLocations", "SpawnLocations");
        playerDatabase = (PlayerDatabase) HushPuppy.safeFindComponent("PlayerDatabase", "PlayerDatabase");

        if (playerDatabase == null) {
            startDefaultGame(); return;
        }

        if (playerSpawnLocations.transform.childCount < playerDatabase.pprefs.Count) {
            Debug.Log("Not enough spawn points!"); return;
        }

        for (int i = 0; i < playerDatabase.pprefs.Count; i++)
            spawnPlayer(playerDatabase.pprefs[i],
                        playerSpawnLocations.getRandomUnusedLocation());
    }

    Player spawnPlayer(PlayerData data, Vector3 location) {
        Player aux = Instantiate(playerPrefab).GetComponent<Player>();
        aux.setPlayer(data.playerID, data.joystick, data.color);
        aux.transform.position = location;
        aux.name = "Player #" + (data.playerID + 1);

        return aux;
    }

    void startDefaultGame() {
        PlayerData aux = new PlayerData("_J0", 0, 0, Color.white);
        spawnPlayer(aux, playerSpawnLocations.getDefaultLocation());
    }
}
