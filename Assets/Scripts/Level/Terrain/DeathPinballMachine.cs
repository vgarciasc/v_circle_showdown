using UnityEngine;
using System.Collections;

public class DeathPinballMachine : MonoBehaviour {
    [SerializeField]
    GameObject deathPinball;
    [SerializeField]
    [Range(0f, 10f)]
    float interval = 1f;
    [SerializeField]
    Transform spawnPoint;

	void Start () {
        StartCoroutine(spawnPinball());
	}
	
    IEnumerator spawnPinball() {
        while (true) {
            yield return new WaitForSeconds(interval);
            Instantiate(deathPinball, spawnPoint.position, Quaternion.identity);
        }
    }
}
