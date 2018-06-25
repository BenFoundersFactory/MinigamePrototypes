using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Corners : MonoBehaviour {

	[SerializeField] private Vector3 endCameraPosition;
	[SerializeField] private GameObject selectionCanvas;

	[SerializeField] private Text[] heightText;
	private float[] heightAmount;

	private float firstPairDiff;
	private float secondPairDiff;

	private int winningPair;

	private float timer = 5f;
	private float counter = 0f;

	private bool takenCorner = false;
	private bool gameStarted = false;

	[SerializeField] private SelectableHeight[] selectableHeight;
	[SerializeField] private GameObject[] opponentHeights;

	void Start () {
		InitialiseValues();
	}
	
	void Update () {
		if (!gameStarted) {
			float step = 5.0f * Vector3.Distance(Camera.main.transform.position, endCameraPosition) * Time.deltaTime;
        	Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, endCameraPosition, step);
        	
        	if (Vector3.Distance(Camera.main.transform.position, endCameraPosition) <= 0.05f) {
        		gameStarted = true;
        		selectionCanvas.SetActive(true);
        	}

        	return;
        }

		counter += Time.deltaTime;

		if (counter >= timer && !takenCorner) {
			Failure();
		}
	}

	private void InitialiseValues() {
		heightAmount = new float[4];

		for (int i = 0; i < heightAmount.Length; i++) {
			heightAmount[i] = Random.Range(1.5f, 2.0f);
			heightText[i].text = heightAmount[i].ToString("0.00");
		}

		firstPairDiff = heightAmount[0] - heightAmount[1];
		secondPairDiff = heightAmount[2] - heightAmount[3];

		if (firstPairDiff == secondPairDiff) {
			heightAmount[2]--;
			heightText[2].text = heightAmount[2].ToString("0.00");
		}

		if (firstPairDiff > secondPairDiff) {
			winningPair = 0;
		} else {
			winningPair = 1;
		}

	}

	public void SelectPairOne() {
		if (takenCorner) return;

		selectableHeight[0].PlaySelectAnimation();
		selectableHeight[1].gameObject.SetActive(false);
		HideUI();

		if (winningPair == 0) Success();
		else Failure();
	}

	public void SelectPairTwo() {
		if (takenCorner) return;

		selectableHeight[1].PlaySelectAnimation();
		selectableHeight[0].gameObject.SetActive(false);
		HideUI();

		if (winningPair == 1) Success();
		else Failure();
	}

	private void HideUI() {
		for (int i = 0; i < opponentHeights.Length; i++) {
			opponentHeights[i].SetActive(false);
		}
	}

	private void Success() {
		takenCorner = true;
		Debug.Log("SCORED A GOAL!");
	}

	private void Failure() {
		takenCorner = true;
		Debug.Log("OOPS!");
	}
}
