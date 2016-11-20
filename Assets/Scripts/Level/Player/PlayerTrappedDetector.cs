using UnityEngine;
using System.Collections;

public class PlayerTrappedDetector : MonoBehaviour {
 //   Player player;
 //   bool colliding;

	//void Start () {
 //       player = this.transform.root.GetComponent<Player>();
 //       if (player == null)
 //           Debug.Log("Player not found by PlayerTrappedDetector.");
 //   }

 //   //void OnCollisionEnter2D(Collision2D target) {
 //   //       if (target.gameObject.layer == LayerMask.NameToLayer("CommonTerrain")) {
 //   //           colliding = true;
 //   //           StartCoroutine(waitAndSee());
 //   //       }
 //   //   }

 //   //   void OnCollisionStay2D(Collision2D target) {
 //   //       if (target.gameObject.layer == LayerMask.NameToLayer("CommonTerrain")) {
 //   //           colliding = true;
 //   //           StartCoroutine(waitAndSee());
 //   //       }
 //   //   }

 //   //   void OnCollisionExit2D(Collision2D target) {
 //   //       if (target.gameObject.layer == LayerMask.NameToLayer("CommonTerrain")) {
 //   //           colliding = false;
 //   //       }
 //   //   }

 //   //   IEnumerator waitAndSee() {
 //   //       yield return new WaitForSeconds(1.0f);
 //   //       player.corneredDetection((int) myDirection, colliding);
 //   //}

 //   void OnCollisionEnter2D(Collision2D target) {
 //       if (target.gameObject.layer == LayerMask.NameToLayer("CommonTerrain")) {
 //           colliding = true;
 //           StartCoroutine(waitAndSee());
 //       }
 //   }

 //   void OnCollisionExit2D(Collision2D target) {
 //       if (target.gameObject.layer == LayerMask.NameToLayer("CommonTerrain")) {
 //           colliding = false;
 //       }
 //   }

 //   IEnumerator waitAndSee() {
 //       for (int i = 0; i < 5; i++)
 //           yield return new WaitForEndOfFrame();
 //       if (colliding) player.corneredDetection();
 //   }
}
