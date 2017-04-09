using UnityEngine;
using System.Collections;

public class HealbombCloud : MonoBehaviour {
    [SerializeField]
    ParticleSystem partSystem;

    public void set_duration(float duration) {
        partSystem = this.GetComponentInChildren<ParticleSystem>();
        StartCoroutine(kill(duration));
    }

    IEnumerator kill(float duration) {
        yield return PauseManager.getPauseManager().WaitForSecondsInterruptable(duration);

        partSystem.Stop();

        yield return PauseManager.getPauseManager().WaitForSecondsInterruptable(partSystem.startLifetime);

        Destroy(this.gameObject);
    }
}
