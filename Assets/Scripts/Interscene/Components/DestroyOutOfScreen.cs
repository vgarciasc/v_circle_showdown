using UnityEngine;
using System.Collections;

public class DestroyOutOfScreen : MonoBehaviour {
    SpriteRenderer srenderer;

    void Start() {
        srenderer = GetComponent<SpriteRenderer>();
        if (srenderer == null)
            Debug.Log("Could not find 'SpriteRenderer' in game object '" + name + "'.");
        StartCoroutine(checkOutOfScreen());
    }

	IEnumerator checkOutOfScreen() {
        while (true) {
            yield return new WaitForSeconds(2.0f);
            if (!srenderer.isVisible) {
                yield return new WaitForSeconds(2.0f);
                if (!srenderer.isVisible)
                    Destroy(gameObject);
            }
        }
	}
}
