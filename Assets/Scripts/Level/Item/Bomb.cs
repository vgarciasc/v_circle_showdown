using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour, ISmashable {
    [SerializeField]
    float lifetime = 2f;

    [SerializeField]
    GameObject sprite;
    [SerializeField]
    GameObject explosion;

    Animator animator;
    Coroutine blinkCoroutine;
    AudioSource audioPlayer;

    [SerializeField]
    AudioClip sfx_explode;

    float intensity;
    bool alreadyExploded = false;

	void Start () {
        animator = this.GetComponent<Animator>();
        audioPlayer = this.GetComponent<AudioSource>();

        blinkCoroutine = StartCoroutine(blink());
        StartCoroutine(deathCount());
	}

    public void setBomb(Vector3 direction, Vector3 scale, float charge) {
        intensity = scale.x;
        this.GetComponent<Rigidbody2D>().velocity = direction * 30f * (charge + 50) / 100f;
        this.transform.localScale = scale;
        if (this.transform.localScale.x > 2f)
            this.transform.localScale = new Vector3(2f, 2f, 2f);
    }

    IEnumerator deathCount() {
        yield return PauseManager.getPauseManager().WaitForSecondsInterruptable(lifetime);
        StopCoroutine(blinkCoroutine);
        explode();
    }

    IEnumerator blink() {
        bool toggle = false;
        Color originalColor = sprite.GetComponent<SpriteRenderer>().color;

        while (true) {
            toggle = !toggle;
            if (toggle) sprite.GetComponent<SpriteRenderer>().color = Color.red;
            else sprite.GetComponent<SpriteRenderer>().color = originalColor;

            for (int i = 0; i < 5; i++)
                yield return new WaitForEndOfFrame();
        }
    }

    void screenShake() {
        Camera.main.GetComponent<SpecialCamera>().screenShake_(intensity);
    }

    void explode() {
        if (alreadyExploded) return;

        foreach (CircleCollider2D cc in this.GetComponentsInChildren<CircleCollider2D>()) {
            cc.enabled = false;
        }

        this.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        
        alreadyExploded = true;
        screenShake();
        explosion.SetActive(true);
        audioPlayer.PlayOneShot(sfx_explode);
        this.GetComponentInChildren<SpriteRenderer>().enabled = false;
        this.GetComponentsInChildren<ParticleSystem>()[0].Play();
        this.GetComponentsInChildren<ParticleSystem>()[1].Play();
        animator.SetTrigger("explode");
    }

    void AnimKill() {
        Destroy(this.gameObject);
    }

    public void smashedDetected() {
        explode();
    }
}
