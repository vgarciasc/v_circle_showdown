using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerSpawnerCircleWitch : MonoBehaviour {
	[SerializeField]
	Transform target_position;
	static int player_index = 0;

	void Start () {
		// transform.DOMove(target_position.position, 1, false);		
		StartCoroutine(spawn());
	}

	IEnumerator spawn() {
		Vector3 aux = this.transform.localScale;
        this.transform.localScale = Vector3.zero;
        this.transform.DOScale(aux, 1f);

		yield return new WaitForSeconds(2f);

		this.transform.DOScale(Vector3.zero, 1f);
	}
}
