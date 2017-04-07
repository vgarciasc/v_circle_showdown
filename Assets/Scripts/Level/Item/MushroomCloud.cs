using UnityEngine;
using System.Collections;

public class MushroomCloud : MonoBehaviour {
    [SerializeField]
    ParticleSystem partSystem;

    public int playerID;

    public void setMushroomCloud(float duration, Vector3 scale, Color cloud_color, int playerID) {
        StartCoroutine(kill(duration));
    
        this.playerID = playerID;
        ParticleSystem.MainModule aux = partSystem.main;
        aux.startColor = new Color(cloud_color.r - 0.3f, 
            cloud_color.g - 0.3f,
            cloud_color.b - 0.34f,
            1f);
        
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
