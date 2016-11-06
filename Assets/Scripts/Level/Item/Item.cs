using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Item : MonoBehaviour {
    public enum Type { NONE, TRIANGLE, REVERSE, HERBALIFE, BLACK_HOLE, GHOST, STUN };
    public Type type;

    [SerializeField]
    GameObject blackhole;
    [SerializeField]
    Sprite[] itemSprite;
    [SerializeField]
    public static float triangleDuration = 5.0f;
    [SerializeField]
    public static float reverseDuration = 5.0f;
    [SerializeField]
    public static float ghostDuration = 5.0f;
    [SerializeField]
    public static float stunCarriedDuration = 5.0f;
    [SerializeField]
    public static float stunDuration = 5.0f;
    [SerializeField]
    List<Type> banned = new List<Type>();

    ItemSpawner itemSpawner;
    bool isBlackHole = false;

    void Start() {
        itemSpawner = (ItemSpawner) HushPuppy.safeFindComponent("GameController", "ItemSpawner");
        this.GetComponent<SpriteRenderer>().sprite = itemSprite[(int) type];
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

    public void setRandomType() {
        //you ain't gonna get (Item.Type) NONE
        do { this.type = (Type)Random.Range(1, System.Enum.GetNames(typeof(Type)).Length); }
        while (banned.Contains(this.type));
    }
}
