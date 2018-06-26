using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dial : MonoBehaviour {

    private ThrowInsManager throwInsManager;

    [SerializeField] private Image dialFill;
    [SerializeField] private DialSegment[] dialSegment;

    void Start() {
        throwInsManager = ThrowInsManager.instance;    
	}

	public void FillDial(int segment) {
        dialFill.fillAmount = 0.05f + (segment * 0.05f);
    }

    public void SendDialValue() {
        throwInsManager.InputDialAnswer(dialFill.fillAmount);
    }
}
