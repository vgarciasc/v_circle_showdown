using UnityEngine;
using System.Collections;

public class SpecialCamera : MonoBehaviour {

    public void screenShake_(float power) { StartCoroutine(screenShake(power)); }
    IEnumerator screenShake(float power) {
        Vector3 originalPos = this.transform.localPosition;
        for (int i = 0; i < 10; i++) {
            yield return new WaitForEndOfFrame();
            this.transform.localPosition = new Vector3(originalPos.x + Random.Range(-power / 2, power / 2),
                                                       originalPos.y + Random.Range(-power / 2, power / 2),
                                                       originalPos.z);
        }

        this.transform.localPosition = originalPos;
    }
}
