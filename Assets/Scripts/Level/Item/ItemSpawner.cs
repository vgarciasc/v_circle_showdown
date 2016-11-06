using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour {
    SpawnLocations itemSpawnLocations;
    [SerializeField]
    GameObject itemPrefab;
    [SerializeField]
    [Range(0f, 20f)]
    float itemSpawnInterval = 5.0f;

    bool itemInGame = false;

	void Start () {
        itemSpawnLocations = (SpawnLocations) HushPuppy.safeFindComponent("ItemSpawnLocations", "SpawnLocations");
        StartCoroutine(handleSpawns());
    }

    public void setItemInGame(bool value) {
        itemInGame = value;
    }

    IEnumerator handleSpawns() {
        while (true) {
            yield return new WaitUntil(() => !itemInGame);
            yield return new WaitForSeconds(itemSpawnInterval);
            if (!itemInGame)
                spawnItem(itemSpawnLocations.getRandomLocation());
        }
    }

    //spawn an item of type 'itemType'
    public void spawnItem(Vector3 location, Item.Type itemType) {
        GameObject aux = (GameObject) Instantiate(itemPrefab, location, Quaternion.identity);
        Item item = aux.GetComponent<Item>();
        item.type = itemType;
        setItemInGame(true);
    }

    //spawn item of random type
    public void spawnItem(Vector3 location) {
        GameObject aux = (GameObject) Instantiate(itemPrefab, location, Quaternion.identity);
        Item item = aux.GetComponent<Item>();
        item.setRandomType();
        setItemInGame(true);
    }
}