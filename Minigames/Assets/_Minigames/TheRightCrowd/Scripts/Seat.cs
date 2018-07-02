using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : MonoBehaviour {

    public int currentColor = -1;

    [SerializeField] private Color white;
    [SerializeField] private Color red;
    [SerializeField] private Color blue;

    private SpriteRenderer spriteRenderer;

	void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void OnMouseDown() {
        if (currentColor == -1) {
            spriteRenderer.color = red;
            currentColor = 0;
        } else if (currentColor == 0) {
            spriteRenderer.color = blue;
            currentColor = 1;
        } else if (currentColor == 1) {
            spriteRenderer.color = red;
            currentColor = 0;
        }
	}

	public void ResetColor() {
        spriteRenderer.color = white;
	}
}
