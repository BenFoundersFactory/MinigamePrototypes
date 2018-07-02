using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleSmashCube : MonoBehaviour {

    private int id;
    [SerializeField] private GameObject cube;
    [SerializeField] private GameObject smashedCube;
    public Text countText;

    public bool smashed = false;

	void OnMouseDown() {
        CircleSmashManager.instance.SelectCube(id);
	}

    public void SetupCube(int id) {
        this.id = id;
    }

	void OnTriggerEnter(Collider other) {
        cube.SetActive(false);
        smashedCube.SetActive(true);
        countText.text = CircleSmashManager.instance.GetCount().ToString();
        CircleSmashManager.instance.AddAccumulatedScore();
        smashed = true;
	}

}
