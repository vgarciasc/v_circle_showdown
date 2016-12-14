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

        if (playerSpawnLocations.transform.childCount < playerDatabase.players.Count) {
            Debug.Log("Not enough spawn points!"); return;
        }

        for (int i = 0; i < playerDatabase.players.Count; i++)
            spawnPlayer(playerDatabase.players[i],
                        playerSpawnLocations.getLocationByIndex(i));
    }

    Player spawnPlayer(PlayerInstance data, Vector3 location) {
        Player aux = Instantiate(playerPrefab).GetComponent<Player>();
        aux.setPlayer(data);
        aux.transform.position = location;
        aux.name = "Player #" + (data.playerID + 1);

        return aux;
    }

    void startDefaultGame() {
        PlayerInstance aux = new PlayerInstance("_J0", 0, 0, Color.grey);
        spawnPlayer(aux, playerSpawnLocations.getDefaultLocation());
    }
}
