using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Item : MonoBehaviour {
    public ItemData data;    

    [SerializeField]
    GameObject blackhole;

    ItemSpawner itemSpawner;
    bool isBlackHole = false;

    public void setItem(ItemSpawner itemSpawner) {
        this.itemSpawner = itemSpawner;
    }

    void Start() {
        if (itemSpawner == null) itemSpawner = (ItemSpawner) HushPuppy.safeFindComponent("GameController", "ItemSpawner");
    }

    void Update() {
        this.transform.Rotate(new Vector3(0f, 0f, 0.5f));
    }

    public void destroy() {
        itemSpawner.setItemInGame(false);
        Destroy(this.gameObject);
    }

    public Sprite getSprite() {
        return this.GetComponent<SpriteRenderer>().sprite;
    }

    public void activateBlackHole() {
        Instantiate(blackhole, this.transform.position, Quaternion.identity);
        destroy();
    }

    public void setType(ItemData item) {
        this.data = item;
        this.GetComponent<SpriteRenderer>().sprite = item.sprite;
    }

    public void setRandomType() {
        //you ain't gonna get (Item.Type) NONE
        List<int> probabilities = new List<int>();
        int total = 0;

        for (int i = 0; i < itemSpawner.levelItems.Count; i++) {
            total += itemSpawner.levelItems[i].probability;
            probabilities.Add(total);
            //Debug.Log("probabilities[" + i + "]: " + probabilities[i]);
        }

        int random = Random.Range(1, total);
        //Debug.Log("random: " + random);
        int index;
        for (index = 0; index < probabilities.Count && random > probabilities[index]; index++);
        //Debug.Log("index: " + index);
        setType(itemSpawner.levelItems[index].item);
        //data.type = (ItemType) Random.Range(1, System.Enum.GetNames(typeof(ItemType)).Length);
    }
}
