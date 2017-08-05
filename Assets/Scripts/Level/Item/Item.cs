using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Item : MonoBehaviour {
    public ItemData data;    

    [Header("Prefabs and References")]
    [SerializeField]
    GameObject blackhole;
    [SerializeField]
    GameObject sprite;
    [SerializeField]
    GameObject sprite_outline;
    [SerializeField]
    ParticleSystem psystem_idle;
    [SerializeField]
    ParticleSystem psystem_destroy;

    [Header("Mystery Box")]
    [SerializeField]
    GameObject mysteryBox;
    [SerializeField]
    bool isMysteryBox = true;

    Transform box1, box2;
    ItemSpawner itemSpawner;
    Animator animator;

    public void setItem(ItemSpawner itemSpawner, ItemData itemData) {
        this.itemSpawner = itemSpawner;
        this.data = itemData;
        if (isMysteryBox) setMysteryBox();
        else setItemSprite();
    }

    void Start() {
        if (itemSpawner == null) {
            itemSpawner = (ItemSpawner) HushPuppy.safeFindComponent("GameController", "ItemSpawner");
        }

        animator = this.GetComponent<Animator>();
    }

    void Update() {
        if (isMysteryBox && box1 != null && box2 != null) {
            float speed = 1f;
            box1.Rotate(new Vector3(0f, 0f, speed));
            box2.Rotate(new Vector3(0f, 0f, -speed));
        } else {
            //sprite.transform.Rotate(new Vector3(0f, 0f, 0.3f));
        }

        if (this.data == null) {
            psystem_idle.Stop();
        }
    }

    void setItemSprite() {
        sprite.SetActive(true);
        mysteryBox.SetActive(false);
        sprite.GetComponent<SpriteRenderer>().sprite = data.sprite;
        sprite_outline.GetComponent<SpriteRenderer>().sprite = data.sprite;
    }

    public void destroy() {
        // if (!idle_state) {
        //     return;
        // }

        idle_state = false;
        this.data = null;

        if (itemSpawner != null) {
            itemSpawner.setItemInGame(false);
        }

        psystem_destroy.Play();
        psystem_idle.Stop();
        animator.SetTrigger("destroy");
        this.GetComponentInChildren<BoxCollider2D>().enabled = false;
        // Destroy(this.gameObject);
    }

    public void AnimDestroy() {
        Destroy(this.gameObject);
    }

    public void activateBlackHole() {
        Instantiate(blackhole, this.transform.position, Quaternion.identity);
        destroy();
    }

    void setMysteryBox() {
        sprite.SetActive(false);
        mysteryBox.SetActive(true);
        box1 = mysteryBox.transform.GetChild(1);
        box2 = mysteryBox.transform.GetChild(2);
    }

    bool idle_state = false;

    void idle() {
        idle_state = true;
    }
}
