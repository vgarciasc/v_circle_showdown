using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenDeluxeArt : MonoBehaviour {

	public void Anim_startIdle() {
		this.GetComponent<Animator>().SetBool("idle", true); }
}
