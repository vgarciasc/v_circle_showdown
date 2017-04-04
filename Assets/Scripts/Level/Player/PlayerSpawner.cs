using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour {
    [Header("Prefabs & References")]
    [SerializeField]
    GameObject playerPrefab;
    [SerializeField]
    PlayerData playerData;

    SpawnLocations playerSpawnLocations;
    PlayerDatabase playerDatabase;

    public static PlayerSpawner getPlayerSpawner() {
        return (PlayerSpawner) HushPuppy.safeFindComponent("GameController", "PlayerSpawner");
    }

    void Start() {
        playerSpawnLocations = (SpawnLocations) HushPuppy.safeFindComponent("PlayerSpawnLocations", "SpawnLocations");
        playerDatabase = PlayerDatabase.getPlayerDatabase();

        if (playerDatabase == null) {
            startDefaultGame(); return;
        }

        // if (playerSpawnLocations.transform.childCount < playerDatabase.players.Count) {
        //     Debug.Log("Not enough spawn points!");
        // }
    }

    public void spawnAllPlayers() {
        for (int i = 0; i < playerDatabase.players.Count; i++) {
            spawnPlayer(playerDatabase.players[i],
                        playerSpawnLocations.getRandomUnusedLocation());
        }
        // PlayerInstance aux = new PlayerInstance("_J0", 0, 0, "vinizinho", Color.grey);
        // spawnPlayer(aux, playerSpawnLocations.getRandomUnusedLocation());
    }

    public Player spawnPlayer(PlayerInstance data, Vector3 location) {
        Player aux = Instantiate(playerPrefab).GetComponent<Player>();
        aux.setPlayer(data);
        aux.transform.position = location;
        aux.name = "Player #" + (data.playerID + 1);
        aux.originalData = playerData;

        return aux;
    }

    public Player spawnGhostShell(PlayerInstance data, Vector3 location) {
        Player aux = Instantiate(playerPrefab).GetComponent<Player>();
        aux.setPlayer(data);
        aux.transform.position = location;
        aux.name = "Player Ghost #" + (data.playerID + 1);
        aux.toggle_block_all_input(true);

        return aux;
    }

    void startDefaultGame() {
        PlayerColor p_color = new PlayerColor();
        p_color.gradient = null;
        
        p_color.color = Color.grey;
        PlayerInstance aux = new PlayerInstance("_J0", 0, 0, "vinizinho", p_color);

        p_color.color = Color.white;
        PlayerInstance aux2 = new PlayerInstance("_J1", 1, 1, "rasputin", p_color);

        p_color.color = Color.magenta;
        PlayerInstance aux3 = new PlayerInstance("_J2", 2, 2, "destructor", p_color);

        spawnPlayer(aux, playerSpawnLocations.getRandomUnusedLocation());
        spawnPlayer(aux2, playerSpawnLocations.getRandomUnusedLocation());
        spawnPlayer(aux3, playerSpawnLocations.getRandomUnusedLocation());
    }
}
