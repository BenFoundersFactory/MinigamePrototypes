using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBlockPoint : MonoBehaviour {

    public int id;
    public int xCoord;
    public int yCoord;

    public void SetupPoint(int id, int x, int y) {
        this.id = id;
        xCoord = x;
        yCoord = y;
    }

    void OnMouseDown() {
        GridBlockManager.instance.CheckSuccess(this);
    }
}
