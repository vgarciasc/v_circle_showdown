using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLobbyMapPreview : MonoBehaviour {
	
	public LevelLobbyManager levelLobby;

	void EndAnimLeft() {
		levelLobby.end_animation();
	}

	void EndAnimRight() {
		levelLobby.end_animation();
	}
}
