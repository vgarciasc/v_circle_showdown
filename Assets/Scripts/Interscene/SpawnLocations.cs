﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnLocations : MonoBehaviour {
    /* Script criado para funcionar como um spawner genérico. Capaz de gerenciar tipos diferentes de spawn
    (como Round Robin). Utilizar em qualquer spawner. */

    List<Transform> locations = new List<Transform>();
    List<Transform> unusedLocations = new List<Transform>();

    void Start () {
        getLocations();
        resetUnusedLocations();
    }
	
    void getLocations() {
        for (int i = 0; i < transform.childCount; i++)
            locations.Add(transform.GetChild(i));
    }

    void resetUnusedLocations() {
        unusedLocations.Clear();
        unusedLocations.AddRange(locations);
    }

    #region Public Interface
    public Vector3 getRandomLocation() {
        return locations[Random.Range(0, locations.Count)].position;
    }

    public Vector3 getRandomUnusedLocation() {
        if (locations.Count == 0) {
            Debug.Log("No new spawn positions in '" + this.gameObject.name + "'. Using an used one instead.");
            resetUnusedLocations();
        }

        int randomIndex = Random.Range(0, locations.Count);
        Vector3 randomLocation = unusedLocations[randomIndex].position;
        unusedLocations.RemoveAt(randomIndex);

        return randomLocation;
    }

    public Vector3 getDefaultLocation() {
        return locations[0].position;
    }
    #endregion
}