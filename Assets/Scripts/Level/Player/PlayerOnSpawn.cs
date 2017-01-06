using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerOnSpawn : MonoBehaviour {
	void Start () {
        StartCoroutine(spawn_animation());
	}

    #region On Spawn
    IEnumerator spawn_animation() {
        Vector3 aux = this.transform.localScale;
        this.transform.localScale = Vector3.zero;

        yield return new WaitForSeconds(1f);

        this.transform.DOScale(aux, 2f);
    }
    #endregion
}
