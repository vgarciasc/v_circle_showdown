using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Portal : MonoBehaviour {
    [SerializeField]
    Portal[] possibleExits;
    [SerializeField]
    [Range(0f, 2f)]
    float portalCooldown = 1.0f;
    
    static List<GameObject> inCooldown = new List<GameObject>();

    void Update() {
        this.transform.Rotate(new Vector3(0f, 0f, 0.5f));
    }

    Portal nextPortal() {
        return possibleExits[Random.Range(0, possibleExits.Length)];
    }

    public void teleport(GameObject target) {
        if (!inCooldown.Contains(target)) inCooldown.Add(target);
        else return;

        Vector3 position = nextPortal().transform.position;
        target.transform.position = position;
        StartCoroutine(endCooldown(target));
    }

    IEnumerator endCooldown(GameObject target) {
        yield return new WaitForSeconds(portalCooldown);
        if (inCooldown.Contains(target)) inCooldown.Remove(target);
    }
}
