using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour {
    SpawnLocations itemSpawnLocations;
    [SerializeField]
    GameObject itemPrefab;

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
            yield return new WaitForSeconds(5.0f);
            if (!itemInGame)
                activateRotatingPlatformItem(getRandomRotatingPlatform());
        }
    }

    RotatingPlatform getRandomRotatingPlatform() {
        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");
        List<RotatingPlatform> rotatingPlatforms = new List<RotatingPlatform>();

        for (int i = 0; i < floors.Length; i++) {
            RotatingPlatform aux = floors[i].GetComponent<RotatingPlatform>();
            if (aux != null)
                rotatingPlatforms.Add(aux);
        }

        if (rotatingPlatforms.Count == 0) return null;
        return rotatingPlatforms[Random.Range(0, rotatingPlatforms.Count)];
    }

    void activateRotatingPlatformItem(RotatingPlatform rp) {
        if (rp == null) return;

        rp.switchItem(true);
        itemInGame = true;
    }

    //spawn an item of type 'itemType'
    public void spawnItem(Vector3 location, Item.Type itemType) {
        GameObject aux = (GameObject) Instantiate(itemPrefab, location, Quaternion.identity);
        Item item = aux.GetComponent<Item>();
        item.type = itemType;
    }

    //spawn item of random type
    public void spawnItem(Vector3 location) {
        GameObject aux = (GameObject)Instantiate(itemPrefab, location, Quaternion.identity);
        Item item = aux.GetComponent<Item>();
        item.setRandomType();
    }
}