using UnityEngine;
using System.Collections;

public class PlayerItemUser : MonoBehaviour {
    public delegate void useItem();
    public event useItem healroot,
                        triangle,
                        ghost,
                        bomb,
                        mushroom,
                        coffee;
    
	ItemSpawner item_spawner;
    PlayerSpawner player_spawner;
	Player player;

    public GameObject ghost_line_prefab;

	void Start() {
		player = (Player) HushPuppy.safeComponent(gameObject, "Player");
		player.use_item_event += use_item;
		item_spawner = (ItemSpawner) HushPuppy.safeFindComponent("GameController", "ItemSpawner");
		player_spawner = (PlayerSpawner) HushPuppy.safeFindComponent("GameController", "PlayerSpawner");
	}

	void use_item(ItemData item_data) {
         switch (item_data.type) {
            case ItemType.HERBALIFE:
                //healroot();
                use_herbalife();
                break;
            case ItemType.TRIANGLE:
                //triangle();
                use_triangle(item_data);
                break;
            case ItemType.GHOST:
                //ghost();
                use_ghost(item_data);
                break;
            case ItemType.BOMB:
                //bomb();
                use_bomb();
                break;
            case ItemType.MUSHROOM:
                //mushroom();
                use_mushroom(item_data);
                break;
            case ItemType.COFFEE:
                //coffee();
                use_coffee(item_data);
                break;
            default:
                break;
        }
	}

    //herbalife
	void use_herbalife() {
		transform.localScale = player.originalData.scale;
	}

    //triangulo
	void use_triangle(ItemData data) { StartCoroutine(use_triangle_(data)); }
    IEnumerator use_triangle_(ItemData data) {
        player.toggleTriangle(true);
        yield return new WaitForSeconds(data.cooldown);
        player.toggleTriangle(false);
    }

    //fantasma
	void use_ghost(ItemData data) { /*StartCoroutine(use_ghost_(data));*/ }
	IEnumerator use_ghost_(ItemData data) {
        Player ghost_shell = player_spawner.spawnGhostShell(player.instance,
                                                            player.transform.position);
        player.toggle_colliders(false);
        player.change_player_opacity(0.5f);

        PlayerGhostLine pgl = (PlayerGhostLine) HushPuppy.safeComponent(Instantiate(ghost_line_prefab),
                                                                        "PlayerGhostLine");
        pgl.init(ghost_shell.transform, player.gameObject.transform);

        Ray r = ghost_create_ray(ghost_shell.transform, player.gameObject.transform);

		yield return new WaitForSeconds(data.cooldown * 3 / 5f);
        Coroutine blink = StartCoroutine(player.start_blink());
		yield return new WaitForSeconds(data.cooldown * 2 / 5f);
        StopCoroutine(blink);

        player.change_player_opacity(1f);
        player.toggle_colliders(true);
        Destroy(ghost_shell.gameObject);
        Destroy(pgl);
	}

    Ray ghost_create_ray(Transform origin, Transform direction) {
        Ray aux = new Ray();
        aux.origin = origin.position;
        aux.direction = direction.position;
        return aux;
    }

    //bomba
	void use_bomb() {
        item_spawner.createBomb(this.transform,
							player.cannonPosition.transform,
							player.tackleBuildup);
        player.resetTackle();
    }

    //cogumelo
    void use_mushroom(ItemData data) {
        item_spawner.createMushroomCloud(this.transform, data.cooldown);
    }
    
    //cafe
    void use_coffee(ItemData data) { StartCoroutine(use_coffee_(data)); }
    IEnumerator use_coffee_(ItemData data) {
        player.data.speed *= 3;
        player.data.tackleForce *= 3;

        yield return new WaitForSeconds(data.cooldown);

        player.data.speed /= 3;
        player.data.tackleForce /= 3;        
    }
}
