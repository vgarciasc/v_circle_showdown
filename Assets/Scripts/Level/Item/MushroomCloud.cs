using UnityEngine;
using System.Collections;

public class MushroomCloud : MonoBehaviour {
    [SerializeField]
    ParticleSystem partSystem;

    public void setMushroomCloud(float duration, Vector3 scale) {
        StartCoroutine(kill(duration));

        float modifier = scale.x;
        float maxScale = 2f;
        if (scale.x > maxScale) modifier = maxScale;
        this.transform.localScale = new Vector3(1, 1, 1) * modifier;
    }

    IEnumerator kill(float duration) {
        yield return new WaitForSeconds(duration);

        partSystem.Stop();

        yield return new WaitForSeconds(partSystem.startLifetime);

        Destroy(this.gameObject);
    }
}
