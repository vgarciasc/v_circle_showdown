using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {
    public enum Type { NONE, TRIANGLE, REVERSE, HERBALIFE, BLACK_HOLE };
    public Type type;

    [SerializeField]
    GameObject blackhole;
    [SerializeField]
    Sprite[] itemSprite;
    [SerializeField]
    public static float triangleDuration = 5.0f;
    [SerializeField]
    public static float reverseDuration = 5.0f;

    ItemSpawner itemSpawner;
    bool isBlackHole = false;

    void Start() {
        itemSpawner = (ItemSpawner) HushPuppy.safeFindComponent("GameController", "ItemSpawner");
        this.GetComponent<SpriteRenderer>().sprite = itemSprite[(int) type];
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

    public void setRandomType() {
        //you ain't gonna get (Item.Type) NONE
        this.type = (Type) Random.Range(1, System.Enum.GetNames(typeof(Type)).Length);
    }
}
