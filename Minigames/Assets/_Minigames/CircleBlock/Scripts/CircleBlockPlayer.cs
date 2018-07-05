using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleBlockPlayer : MonoBehaviour {

    private float playerRotationSpeed = 70f;

    [SerializeField] private Text playerDegreeText;
    [SerializeField] private Transform playerParent;

	void Start() {
        playerDegreeText.gameObject.SetActive(false);
	}

	void OnMouseDown() {
        playerDegreeText.gameObject.SetActive(true);
	}

	void OnMouseDrag() {
        if (!CircleBlockManager.instance.ballShot) {
            if (Input.GetAxis("Mouse X") > 0 || Input.GetAxis("Mouse X") < 0) {
                playerParent.Rotate(Vector3.up * -Input.GetAxis("Mouse X") * playerRotationSpeed);

                if ((int)(CircleBlockManager.instance.PROTRACTOR_FILL) != 1) {
                    if (playerParent.rotation.y < 0) {
                        playerParent.eulerAngles = new Vector3(0f, 0f, 0f);
                    }

                    if (playerParent.eulerAngles.y > 360 * CircleBlockManager.instance.PROTRACTOR_FILL) {
                        playerParent.eulerAngles = new Vector3(0f, 360 * CircleBlockManager.instance.PROTRACTOR_FILL, 0f);
                    }
                }
            }
        }

        playerDegreeText.text = playerParent.eulerAngles.y.ToString("F0");
    }

	void OnMouseUp() {
        playerDegreeText.gameObject.SetActive(false);
	}
}
