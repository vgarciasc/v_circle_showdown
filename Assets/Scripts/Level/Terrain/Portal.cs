using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Portal : MonoBehaviour {
    [SerializeField]
    ParticleSystem partSystem;
    [SerializeField]
    Transform sprite;

    [SerializeField]
    Portal[] possibleExits;
    [SerializeField]
    [Range(0f, 2f)]
    float portalCooldown = 1.0f;
    
    static List<GameObject> inCooldown = new List<GameObject>();

    void Start() {
        setParticleSystem();
    }

    Portal nextPortal() {
        return possibleExits[Random.Range(0, possibleExits.Length)];
    }

    public void teleport(GameObject target) {
        if (!inCooldown.Contains(target)) inCooldown.Add(target);
        else return;

        Portal next = nextPortal();
        Vector3 position = next.transform.position;
        target.transform.position = position;
        StartCoroutine(endCooldown(target));

        //this.playTeleportParticles();
        next.playTeleportParticles();
    }

    IEnumerator endCooldown(GameObject target) {
        yield return new WaitForSeconds(portalCooldown);
        if (inCooldown.Contains(target)) inCooldown.Remove(target);
    }

    #region Particle System
    void setParticleSystem() {
        partSystem.startColor = sprite.GetComponent<SpriteRenderer>().color;
    }

    public void playTeleportParticles() {
        this.partSystem.Play();
    }
    #endregion
}
