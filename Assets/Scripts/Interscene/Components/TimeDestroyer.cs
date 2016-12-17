using UnityEngine;
using System.Collections;

public class TimeDestroyer : MonoBehaviour {

    [Range(0f, 10f)]
    public float delayUntilFade = 3f;
    [Range(0f, 5f)]
    public float fadeDuration = 2f;

    bool canFade = false;
    SpriteRenderer sprite;
    float startTime;

	void Start () {
        sprite = this.GetComponent<SpriteRenderer>();
        startTime = Time.time;
        StartCoroutine(destroy());
    }

    void FixedUpdate () {
        if (canFade) {
            float t = (Time.time - startTime) / fadeDuration;
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, Mathf.SmoothStep(1f, 0f, t));
            if (sprite.color.a <= 0.1f)
                Destroy(this.gameObject);
        }
    }

    IEnumerator destroy() {
        yield return new WaitForSeconds(delayUntilFade);
        startTime = Time.time;
        canFade = true;
    }	
}