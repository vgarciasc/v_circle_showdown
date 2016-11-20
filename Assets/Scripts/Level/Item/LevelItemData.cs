using UnityEngine;

[System.Serializable]
public class LevelItemData {
    public ItemData item;
    [Range(0, 10)]
    public int probability;
}
