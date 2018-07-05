using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitchPoint : MonoBehaviour {

    public int id;

    public void SetupPoint(int id) {
        this.id = id;
    }

	void OnMouseDown() {
        Debug.Log(id);
	}
}
