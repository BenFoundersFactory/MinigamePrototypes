using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialSegment : MonoBehaviour  {

    [SerializeField] private int segmentID;

    private Image img;

	void Awake() {
        img = GetComponent<Image>();
	}

	void OnMouseDown () {
        Debug.Log("successful check!" + segmentID);
    }
}
