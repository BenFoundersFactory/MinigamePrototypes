using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialSegment : MonoBehaviour {

    [SerializeField] private Dial dial;

    [SerializeField] private int segmentID;

    private Image img;

    void Awake() {
        img = GetComponent<Image>();
    }

    void OnMouseOver() {
        if (Input.GetMouseButton(0)) {
            dial.FillDial(segmentID);
        }
    }

    void OnMouseUp() {
        dial.SendDialValue();
    }

}
