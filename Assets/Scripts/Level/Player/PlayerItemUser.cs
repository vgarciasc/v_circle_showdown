using UnityEngine;
using System.Collections;

public class PlayerItemUser : MonoBehaviour {
    public delegate void useItem();
    public event useItem triangle_start,
                        triangle_end,
                        ghost,
                        bomb,
                        mushroom,
                        coffee_start,
                        coffee_end,
                        double_coffee;
    public event useItem healStart, healEnd;

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
            case ItemType.HEAL:
                use_heal(item_data);
                break;
            case ItemType.TRIANGLE:
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
                if (mushroom != null) {
                    mushroom();
                }
                use_mushroom(item_data);
                break;
            case ItemType.COFFEE:
                use_coffee(item_data);
                break;
            case ItemType.AIMBOT:
                //aimbot();
                use_aimbot(item_data);
                break;
            case ItemType.HEALBOMB:
                use_healbomb(item_data);
                break;
            default:
                break;
        }
	}

    //heal
    void use_heal(ItemData data) { StartCoroutine(use_heal_(data)); }
    IEnumerator use_heal_(ItemData data) {
        healStart();
        while (transform.localScale.x > player.originalData.scale.x) {
            player.changeSize(-0.05f);
            yield return HushPuppy.WaitForEndOfFrames(1);
        }
        healEnd();
	}

    //triangulo
	Coroutine triangle_coroutine;
    Coroutine player_triangle_blink_coroutine;
    
    void use_triangle(ItemData data) {
        if (triangle_coroutine != null) {
            StopCoroutine(triangle_coroutine);
        }
        
        triangle_coroutine = StartCoroutine(use_triangle_(data));
    }

    IEnumerator use_triangle_(ItemData data) {
        player.toggleTriangle(true);
        if (triangle_start != null) {
            triangle_start();
        }
        
        if (player_triangle_blink_coroutine != null) {
            StopCoroutine(player_triangle_blink_coroutine);
        }

        player_triangle_blink_coroutine = StartCoroutine(player.start_blink(Time.time + data.cooldown));
        yield return PauseManager.getPauseManager().WaitForSecondsInterruptable(data.cooldown * 0.5f);
		yield return PauseManager.getPauseManager().WaitForSecondsInterruptable(data.cooldown * 0.5f);

        StopCoroutine(player_triangle_blink_coroutine);
        player_triangle_blink_coroutine = null;

        player.end_blink();
        player.toggleTriangle(false);
        if (triangle_end != null) {
            triangle_end();
        }

        triangle_coroutine = null;
    }

    //fantasma
	void use_ghost(ItemData data) { /*StartCoroutine(use_ghost_(data));*/ }
	IEnumerator use_ghost_(ItemData data) {
        // Player ghost_shell = player_spawner.spawnGhostShell(player.instance,
        //                                                     player.transform.position);
        // player.toggle_colliders(false);
        // player.change_player_opacity(0.5f);

        // PlayerGhostLine pgl = (PlayerGhostLine) HushPuppy.safeComponent(Instantiate(ghost_line_prefab),
        //                                                                 "PlayerGhostLine");
        // pgl.init(ghost_shell.transform, player.gameObject.transform);

        // Ray r = ghost_create_ray(ghost_shell.transform, player.gameObject.transform);

		yield return PauseManager.getPauseManager().WaitForSecondsInterruptable(data.cooldown * 3 / 5f);
        // Coroutine blink = StartCoroutine(player.start_blink());
		// yield return PauseManager.getPauseManager().WaitForSecondsInterruptable(data.cooldown * 2 / 5f);
        // StopCoroutine(blink);

        // player.change_player_opacity(1f);
        // player.toggle_colliders(true);
        // Destroy(ghost_shell.gameObject);
        // Destroy(pgl);
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
							player.chargeBuildup);
        player.reset_charge();
    }

    //cogumelo
    void use_mushroom(ItemData data) {
        this.transform.GetComponentInChildren<Rigidbody2D>().velocity += new Vector2(this.transform.up.x,
            this.transform.up.y) * (player.chargeBuildup + 1f);
        item_spawner.createMushroomCloud(this.transform, data.cooldown);
        player.reset_charge();
    }
    
    bool using_coffee = false;

    //cafe
    void use_coffee(ItemData data) { StartCoroutine(use_coffee_(data)); }
    IEnumerator use_coffee_(ItemData data) {
        if (using_coffee && double_coffee != null) {
            double_coffee();
        }

        if (coffee_start != null) {
            coffee_start();
        }

        using_coffee = true;
        player.data.speed *= 3;
        player.data.chargeForce *= 3;
        // player.GetComponent<PlayerParticleSystems>().trail_length_modifier *= 20;

        yield return PauseManager.getPauseManager().WaitForSecondsInterruptable(data.cooldown);

        if (coffee_end != null) {
            coffee_end();
        }

        // player.GetComponent<PlayerParticleSystems>().trail_length_modifier /= 20;
        using_coffee = false;
        player.data.speed /= 3;
        player.data.chargeForce /= 3;        
    }

    //aimbot
    Coroutine aimbot_coroutine = null;

    void use_aimbot(ItemData data) {
        if (aimbot_coroutine != null) {
            StopCoroutine(aimbot_coroutine);
        }

        aimbot_coroutine = StartCoroutine(use_aimbot_(data));
    }

    IEnumerator use_aimbot_(ItemData data) {
        player.start_emitting_aimbot(data.cooldown);

        yield return PauseManager.getPauseManager().WaitForSecondsInterruptable(data.cooldown);

        player.end_emitting_aimbot();
        GameObject[] players = Player.getAllPlayers();
        for (int i = 0; i < players.Length; i++) {
            players[i].GetComponent<Player>().end_receiving_aimbot();
        }

        aimbot_coroutine = null;
    }

    //healbomb
    void use_healbomb(ItemData data) {
        item_spawner.createHealbomb(this.transform,
							player.cannonPosition.transform,
							player.chargeBuildup);
        player.reset_charge();
    }
}
