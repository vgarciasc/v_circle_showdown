using UnityEngine;
using System.Collections;

public class SpecialCamera : MonoBehaviour {
    enum Movement { None, Right, Left, Top, Bottom };
    GameObject[] players;
    float originalSize;
    Vector3 originalPos;
    Camera cam;

    [SerializeField]
    GameObject camBorders;

    void Start() {
        originalPos = this.transform.localPosition;
        cam = this.GetComponent<Camera>();
        originalSize = cam.orthographicSize;
        //StartCoroutine(restoreOriginalPosition());
    }

    void Update() {
        //updatePlayerList();
        //updateCameraPosition();
    }

    #region Player Framing
    IEnumerator restoreOriginalPosition() {
        while (true) {
            this.transform.localPosition += (originalPos - this.transform.localPosition) / 20f;
            for (int i = 0; i < 2; i++)
                yield return new WaitForEndOfFrame();
        }
    }

    void updatePlayerList() {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    void updateCameraPosition() {
        for (int i = 0; i < players.Length; i++) {
            Vector3 viewportPos = cam.WorldToViewportPoint(players[i].transform.position);
            if ((viewportPos.x > 1 || viewportPos.x < 0 || viewportPos.y > 1 || viewportPos.y < 0)) {
                follow(viewportPos);
            }
        }
    }

    void follow(Vector3 pos) {
        if (pos.x < 0) {
            this.transform.localPosition += new Vector3(-0.2f, 0.0f);
        } else if (pos.x > 1) {
            this.transform.localPosition += new Vector3(0.2f, 0.0f);
        } else if (pos.y < 0) {
            this.transform.localPosition += new Vector3(0.0f, -0.2f);
        } else if (pos.y > 1) {
            this.transform.localPosition += new Vector3(0.0f, 0.2f);
        } 
    }
    #endregion

    #region Screen Shake
    public void screenShake_(float power) { StartCoroutine(screenShake(power)); }
    IEnumerator screenShake(float power) {
        if (power < 0.05f) {
            power = 0.1f;
        }

        for (int i = 0; i < 10; i++) {
            yield return new WaitForEndOfFrame();
            float x = getScreenShakeDistance(power);
            float y = getScreenShakeDistance(power);

            this.transform.localPosition = new Vector3(originalPos.x + x,
                                                       originalPos.y + y,
                                                       originalPos.z);
        }

        this.transform.localPosition = originalPos;
    }

    float getScreenShakeDistance(float power) {
        float power_aux = power;
        int count = 0;
        while (true) {
            count++;
            float aux = Mathf.Pow(-1, Random.Range(0, 2)) * Random.Range(power_aux / 4, power_aux / 2);
            if (Mathf.Abs(aux) > 0.1f) {
                return aux;
            }
            if (count > 5) {
                count = 0;
                power_aux += 0.25f;
            }
        }
    }
    #endregion
}
