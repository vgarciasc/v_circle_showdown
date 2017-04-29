using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerHeadsetUser : MonoBehaviour {
	public SpriteRenderer stomachSprite;
	public Transform white_circle;

	Player player;
	Rigidbody2D rb;

	bool spirited_away = false;

	void Start () {
		player = this.GetComponent<Player>();
		rb = this.GetComponent<Rigidbody2D>();
		
		player.get_item_event += get_item;
		player.use_item_event += use_item;		
	}

	void Update() {
		if (Input.GetButtonDown("Submit2" + player.joystick)) {
			if (!spirited_away) {
				stomachSprite.enabled = false;
				StartCoroutine(PlayerDatabase.getPlayerDatabase().setReady(player.ID));
				StartCoroutine(despawn_circle());
			}
		}
	}

	void get_item(ItemData data) {
		if (data.type == ItemType.VR_HEADSET) {
			stomachSprite.enabled = true;
		}
	}

	void use_item(ItemData data) {
		if (data.type == ItemType.VR_HEADSET) {
			spirited_away = true;
			stomachSprite.enabled = false;
			StartCoroutine(despawn_circle());
		}
	}

	public IEnumerator despawn_circle() {
		white_circle.GetComponent<SpriteRenderer>().color = new Color(
			player.palette.color.r + 0.3f,
			player.palette.color.g + 0.3f,
			player.palette.color.b + 0.3f);

        player.toggle_block_all_input(true);
        RigidbodyConstraints2D rb_original_constraints = rb.constraints;
        
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

        Vector3 max_scale = white_circle.localScale;
        white_circle.localScale = Vector3.zero;
        white_circle.GetComponent<SpriteRenderer>().enabled = true;

        white_circle.DOScale(max_scale, 0.5f);
        yield return new WaitForSeconds(0.5f);
        
        player.toggle_visibility(false);

        white_circle.DOScale(Vector3.zero, 0.5f);
        yield return new WaitForSeconds(0.5f);
        white_circle.GetComponent<SpriteRenderer>().enabled = false;

        player.toggle_block_all_input(true);
        // rb.constraints = rb_original_constraints;
	}
}
