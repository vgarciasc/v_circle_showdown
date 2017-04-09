using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffParticleSystem : MonoBehaviour {
	[SerializeField]
	[RangeAttribute(0f, 10f)]
	float delay = 0f;
	
	void Start() {
		StartCoroutine(destroyAfterDelay());
	}
	
	IEnumerator destroyAfterDelay() {
		yield return new WaitForSeconds(delay);

		this.GetComponentInChildren<ParticleSystem>().Stop();
	}
}
