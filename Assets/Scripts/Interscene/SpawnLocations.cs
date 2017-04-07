using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnLocations : MonoBehaviour {
    /* Script criado para funcionar como um spawner genérico. Capaz de gerenciar tipos diferentes de spawn
    (como Round Robin). Utilizar em qualquer spawner. */

    List<Transform> locations = new List<Transform>();
    List<Transform> unusedLocations = new List<Transform>();

    [SerializeField]
    List<Transform> hardLocations = new List<Transform>();
    [SerializeField]
    bool useHardLocations = false;

    void Start () {
        getLocations();
        resetUnusedLocations();
    }
	
    void getLocations() {
        locations.Clear();
        for (int i = 0; i < transform.childCount; i++)
            locations.Add(transform.GetChild(i));
    }

    void resetUnusedLocations() {
        getLocations();
        unusedLocations.Clear();
        unusedLocations.AddRange(locations);
    }

    #region Public Interface
    public Vector3 getRandomLocation() {
        if (useHardLocations) {
            return hardLocations[Random.Range(0, hardLocations.Count)].position;
        }

        return locations[Random.Range(0, locations.Count)].position;
    }

    public Vector3 getRandomUnusedLocation() {
        if (unusedLocations.Count == 0) {
            Debug.Log("No new spawn positions in '" + this.gameObject.name + "'. Using an used one instead.");
            resetUnusedLocations();
        }

        int randomIndex = Random.Range(0, unusedLocations.Count);
        Vector3 randomLocation = unusedLocations[randomIndex].position;
        unusedLocations.RemoveAt(randomIndex);

        return randomLocation;
    }

    public Vector3 getDefaultLocation() {
        return locations[0].position;
    }

    public Vector3 getLocationByIndex(int index) {
        getLocations();
        if (index < 0) return locations[0].position;
        if (index > locations.Count) return locations[locations.Count - 1].position;

        return locations[index].position;
    }
    #endregion
}
