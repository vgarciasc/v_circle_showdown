using UnityEngine;
using System.Collections;

public enum ItemType { NONE, TRIANGLE, HERBALIFE, BLACK_HOLE, GHOST, STUN, BOMB };
[CreateAssetMenu(fileName = "Data", menuName = "Item Data", order = 1)]
public class ItemData : ScriptableObject {
    public float cooldown = 5f;
    public Sprite sprite;
    public ItemType type;
}
