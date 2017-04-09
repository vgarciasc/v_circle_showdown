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

        // StartCoroutine(circle_animation_color());

        player.toggle_visibility(false);
        player.toggle_block_all_input(true);
        RigidbodyConstraints2D rb_original_constraints = rb.constraints;
        
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => !ShowdownPanelAnimation.shouldPlayAnimation);

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
    
    IEnumerator circle_animation_color() {
        int iterations = 10;

        for (int i = 0; i < iterations; i++) {
            white_circle.GetComponent<SpriteRenderer>().color = Color.Lerp(Color.white,
                Color.black,
                i / (float) iterations);

            if (player.ID == 0) {
                Debug.Log("Cor: " + white_circle.GetComponent<SpriteRenderer>().color);
            }
            //1.5f is duration of animation (see other function above)
            yield return HushPuppy.WaitForEndOfFrames((int) (0.5f * 30f / iterations));
        }    
    }

    void set_circle() {
        white_circle.GetComponent<SpriteRenderer>().color = player.palette.color + new Color(0.3f, 0.3f, 0.3f);
    }
    #endregion
}
