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
        yield return PauseManager.getPauseManager().WaitForSecondsInterruptable(duration);

        partSystem.Stop();

        yield return PauseManager.getPauseManager().WaitForSecondsInterruptable(partSystem.startLifetime);

        Destroy(this.gameObject);
    }
}
