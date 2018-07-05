using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seat : MonoBehaviour {

    public int currentColor = -1;

    [SerializeField] private Color white;
    [SerializeField] private Color red;
    [SerializeField] private Color blue;

    private SpriteRenderer spriteRenderer;

    public bool isOn = true;

	void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
	}

	void OnMouseDown() {
        if (currentColor == 2) {
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
        currentColor = -1;
        spriteRenderer.color = white;
	}

    public void RandomColor() {
        int rnd = Random.Range(0, 3);

        if (rnd == 0) {
            spriteRenderer.color = red;
            currentColor = rnd;
        } else if (rnd == 1) {
            spriteRenderer.color = blue;
            currentColor = rnd;
        } else if (rnd == 2) {
            spriteRenderer.color = white;
            currentColor = rnd;
        }
    }
}
