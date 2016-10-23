using UnityEngine;
using System.Collections;

static class Hub {
    public static ItemSpawner itemSpawner;
    public static GameController gameController;

    static Hub() {
        itemSpawner = (ItemSpawner) HushPuppy.safeFindComponent("GameController", "ItemSpawner");
        gameController = (GameController) HushPuppy.safeFindComponent("GameController", "GameController");

    }
}
