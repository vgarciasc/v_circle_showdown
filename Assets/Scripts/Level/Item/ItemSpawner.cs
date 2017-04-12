using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour {
    SpawnLocations itemSpawnLocations;

    [Header("Item Prefabs")]
    [SerializeField]
    GameObject itemPrefab;
    [SerializeField]
    GameObject bombPrefab;
    [SerializeField]
    GameObject healbombPrefab;
    [SerializeField]
    GameObject mushroomCloudPrefab;

    [Header("Level Specifications")]
    public List<LevelItemData> levelItems = new List<LevelItemData>();
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
            yield return PauseManager.getPauseManager().WaitForSecondsInterruptable(itemSpawnInterval);
            if (!itemInGame && PlayerPrefs.GetInt("SpawnItems") == 1)
                spawnItem(itemSpawnLocations.getRandomLocation());
        }
    }

    //spawn a certain item
    public void spawnItem(Vector3 location, ItemData itemData) {
        GameObject aux = (GameObject) Instantiate(itemPrefab, location, Quaternion.identity);
        Item item = aux.GetComponent<Item>();
        item.setItem(this, itemData);
        setItemInGame(true);
    }

    //spawn item of random type
    public void spawnItem(Vector3 location) {
        GameObject aux = (GameObject) Instantiate(itemPrefab, location, Quaternion.identity);
        Item item = aux.GetComponent<Item>();
        item.setItem(this, getRandomType());
        setItemInGame(true);
    }

    ItemData getRandomType() {
        //you ain't gonna get (Item.Type) NONE
        List<int> probabilities = new List<int>();
        int total = 0;

        for (int i = 0; i < levelItems.Count; i++) {
            total += levelItems[i].probability;
            probabilities.Add(total);
            //Debug.Log("probabilities[" + i + "]: " + probabilities[i]);
        }

        int random = Random.Range(1, total);
        //Debug.Log("random: " + random);
        int index;
        for (index = 0; index < probabilities.Count && random > probabilities[index]; index++);
        //Debug.Log("index: " + index);
        return levelItems[index].item;
        //data.type = (ItemType) Random.Range(1, System.Enum.GetNames(typeof(ItemType)).Length);
    }

    #region Bomb
    public void createBomb(Transform player, Transform cannon, float power) {
        Bomb bomb = Instantiate(bombPrefab).GetComponent<Bomb>();
        bomb.transform.position = cannon.position;

        Vector3 bomb_scale = new Vector3(player.localScale.x,
                                        player.localScale.x,
                                        player.localScale.x);
        bomb.transform.GetComponent<Rigidbody2D>().angularVelocity = 600f;
        bomb.setBomb(player.up, bomb_scale, power);
    }
    #endregion

    #region Heal Bomb
    public void createHealbomb(Transform player, Transform cannon, float power) {
        Healbomb bomb = Instantiate(healbombPrefab).GetComponent<Healbomb>();
        bomb.transform.position = cannon.position;

        Vector3 bomb_scale = new Vector3(player.localScale.x,
                                        player.localScale.x,
                                        player.localScale.x);
        bomb.transform.GetComponent<Rigidbody2D>().angularVelocity = 600f;
        bomb.setBomb(player.up, bomb_scale, power);
    }
    #endregion

    #region Mushroom Cloud
    public void createMushroomCloud(Transform player, float duration) {
        MushroomCloud mush = Instantiate(mushroomCloudPrefab).GetComponent<MushroomCloud>();
        mush.transform.position = player.position;

        mush.setMushroomCloud(duration,
            player.localScale,
            player.GetComponentInChildren<Player>().palette.color,
            player.GetComponentInChildren<Player>().ID);
    }
    #endregion
}