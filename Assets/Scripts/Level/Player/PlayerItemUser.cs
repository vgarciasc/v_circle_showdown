﻿using UnityEngine;
using System.Collections;

public class PlayerItemUser : MonoBehaviour {
    public delegate void useItem();
    public event useItem healroot,
                        triangle,
                        ghost,
                        bomb,
                        mushroom;
    
	ItemSpawner item_spawner;
	Player player;

	void Start() {
		player = (Player) HushPuppy.safeComponent(gameObject, "Player");
		player.use_item_event += use_item;
		item_spawner = (ItemSpawner) HushPuppy.safeFindComponent("GameController", "ItemSpawner");
	}

	void use_item(ItemData item_data) {
         switch (item_data.type) {
            case ItemType.HERBALIFE:
                healroot();
                use_herbalife();
                break;
            case ItemType.TRIANGLE:
                //triangle();
                use_triangle_(item_data.cooldown);
                break;
            case ItemType.GHOST:
                //ghost();
                use_ghost_(item_data.cooldown);
                break;
            case ItemType.BOMB:
                //bomb();
                use_bomb();
                break;
            case ItemType.MUSHROOM:
                //mushroom();
                use_mushroom(item_data.cooldown);
                break;
            default:
                break;
        }
	}

    //herbalife
	void use_herbalife() {
		transform.localScale = player.data.scale;
	}

    //triangulo
	void use_triangle_(float duration) { StartCoroutine(use_triangle(duration)); }
    IEnumerator use_triangle(float duration) {
        player.toggleTriangle(true);
        yield return new WaitForSeconds(duration);
        player.toggleTriangle(false);
    }

    //fantasma
	void use_ghost_(float duration) { StartCoroutine(use_ghost(duration)); }
	IEnumerator use_ghost(float duration) {
		yield return new WaitForSeconds(duration);
	}

    //bomba
	void use_bomb() {
        item_spawner.createBomb(this.transform,
							player.cannonPosition.transform,
							player.tackleBuildup);
        player.resetTackle();
    }

    //cogumelo
    void use_mushroom(float duration) {
        item_spawner.createMushroomCloud(this.transform, duration);
    }
}
