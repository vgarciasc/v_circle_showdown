using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {
    public enum Type { TRIANGLE, REVERSE, HERBALIFE, BLACK_HOLE };
    public Type type;

    public void destroy() {
        Destroy(this.gameObject);
    }

    public Sprite getSprite() {
        return this.GetComponent<SpriteRenderer>().sprite;
    }
}
