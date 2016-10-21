using UnityEngine;
using System.Collections;

public class Portal : MonoBehaviour {
    [SerializeField]
    Portal[] possibleExits;

    public Portal nextPortal() {
        return possibleExits[Random.Range(0, possibleExits.Length)];
    }
}
