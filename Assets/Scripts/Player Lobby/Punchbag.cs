using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punchbag : MonoBehaviour, ISmashable {
	public int playerIndex = -1;
	
	[HeaderAttribute("References")]
	[SerializeField]
	ParticleSystem explosion;
	[SerializeField]
	GameObject headsetPrefab;
	[SerializeField]
	SpriteRenderer background;
	[SerializeField]
	List<Color> colors = new List<Color>();
	[SerializeField]
	AudioClip explosionSFX;
	[SerializeField]
	AudioClip hitSFX;

	Animator anim;

	bool invincible = false;
	bool is_dead = false;

	void Start() {
		anim = this.GetComponent<Animator>();

		float auxScale = Random.Range(1.5f, 2.5f);
		this.transform.localScale = new Vector3(auxScale, auxScale, auxScale);
		this.transform.Rotate(new Vector3(0f, 0f, Random.Range(0f, 360f)));

		background.color = colors[Random.Range(0, colors.Count)];
		var aux = explosion.main;
		aux.startColor = background.color;
	}

	public void takeHit(float transferSize) {
		if (invincible) {
			return;
		}

		this.GetComponent<AudioSource>().PlayOneShot(hitSFX, transferSize / 2.5f);

		shakeScreen(transferSize / 4);
        changeSize(transferSize * 1.5f);
        StartCoroutine(temporaryInvincibility(5));
	}

    IEnumerator temporaryInvincibility(int frames) {
        invincible = true;
        for (int i = 0; i < frames; i++)
            yield return new WaitForEndOfFrame();
        invincible = false;
    }

	void changeSize(float sizeIncrement) {
		this.transform.localScale += new Vector3(sizeIncrement, sizeIncrement, sizeIncrement);
	}

	void shakeScreen(float amount) {
		Camera.main.GetComponent<SpecialCamera>().screenShake_(amount);
	}

	public void smashedDetected() {
		if (is_dead) {
			return;
		}

		is_dead = true;

		explosion.Play();
		this.GetComponent<AudioSource>().PlayOneShot(explosionSFX);

		anim.enabled = true;
		anim.SetTrigger("explode");

		StartCoroutine(slowmo(1f));

		this.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
		
		foreach (CircleCollider2D c in this.GetComponentsInChildren<CircleCollider2D>()) {
			c.enabled = false;
		}

		Instantiate(headsetPrefab, this.transform.position, Quaternion.identity);
	}

	public void AnimationKillPlayer() {
		StartCoroutine(SetReady());
	}

	IEnumerator SetReady() {
		yield return new WaitForSeconds(0.5f);

		// StartCoroutine(PlayerDatabase.getPlayerDatabase().setReady(playerIndex));

		this.gameObject.SetActive(false);
	}

    IEnumerator slowmo(float duration) {
        float timescale = 1f;
        float slow = 0.1f;

        Time.timeScale = slow;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
        yield return new WaitForSeconds(duration * slow);

        int aux = 20;
        for (int i = 0; i < aux; i++) {
            if (Time.timeScale < timescale) {
                Time.timeScale += (timescale - slow) / aux;
                Time.fixedDeltaTime = 0.02F * Time.timeScale;
            }
            yield return new WaitForEndOfFrame();
        }
        
        Time.timeScale = timescale;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
    }

	Coroutine rodando = null;

	void OnTriggerEnter2D(Collider2D target) {
		if (rodando == null) {
			rodando = StartCoroutine(foo());
		}
	}

	void OnTriggerStay2D(Collider2D target) {
		if (rodando == null) {
			rodando = StartCoroutine(foo());
		}
	}

	void OnTriggerExit2D(Collider2D target) {
		entalando = false;
	}

	bool entalando = false;
	IEnumerator foo() {
		entalando = true;

		yield return new WaitForSeconds(0.15f);

		if (entalando) {
			smashedDetected();
		}

		rodando = null;
	}
}
