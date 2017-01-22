using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerOnSpawn : MonoBehaviour {
    [SerializeField]
    Transform white_circle;

    Rigidbody2D rb;
    Player player;
    Transform player_transform;
    bool end_spawn = false;

    void Start () {
        player = GetComponent<Player>();
        player_transform = this.transform;
        rb = this.GetComponent<Rigidbody2D>();

        set_circle();
        StartCoroutine(spawn_animation());
	}

    #region On Spawn
    IEnumerator spawn_animation() {
        player.toggle_visibility(false);
        player.toggle_block_all_input(true);
        RigidbodyConstraints2D rb_original_constraints = rb.constraints;
        
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(0.5f);

        Vector3 max_scale = white_circle.localScale;
        white_circle.localScale = Vector3.zero;
        white_circle.GetComponent<SpriteRenderer>().enabled = true;

        white_circle.DOScale(max_scale, 0.5f);
        yield return new WaitForSeconds(0.5f);
        
        player.toggle_visibility(true);

        white_circle.DOScale(Vector3.zero, 0.5f);
        yield return new WaitForSeconds(0.5f);
        white_circle.GetComponent<SpriteRenderer>().enabled = false;

        player.toggle_block_all_input(false);
        rb.constraints = rb_original_constraints;
    }

    public void AnimEndSpawn() {
        end_spawn = true;
    }

    void set_circle() {
        white_circle.GetComponent<SpriteRenderer>().color = player.color + new Color(0.3f, 0.3f, 0.3f);
    }
    #endregion
}
