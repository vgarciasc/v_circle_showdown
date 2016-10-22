using UnityEngine;
using System.Collections;

public class ItemSpawner : MonoBehaviour {
    GameObject itemSpawnLocations;

	void Start () {
        itemSpawnLocations = HushPuppy.findGameObject("Item Spawn Location");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
