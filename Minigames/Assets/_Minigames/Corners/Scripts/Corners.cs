using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Corners : MonoBehaviour {

	[SerializeField] private Vector3 startCameraPosition;
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
	private bool scoredGoal = false;

	[SerializeField] private SelectableHeight[] selectableHeight;
	[SerializeField] private GameObject[] opponentHeights;

	[SerializeField] private GameObject ball;
	[SerializeField] private Transform scoringPoint;
	[SerializeField] private Transform knockAwayPoint;
	[SerializeField] private Transform[] passingPoint;

	private Vector3 ballEndPoint;

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
			Debug.Log("TOOK TOO LONG!");
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
			firstPairDiff = heightAmount[0] - heightAmount[1];
		}

		if (firstPairDiff > secondPairDiff) {
			winningPair = 0;
		} else {
			winningPair = 1;
		}

	}

	public void SelectPairOne() {
		if (takenCorner) return;

		ballEndPoint = new Vector3(passingPoint[0].position.x,
								   passingPoint[0].position.y + 2.5f,
								   passingPoint[0].position.z);

		selectableHeight[0].PlaySelectAnimation();
		selectableHeight[1].gameObject.SetActive(false);
		HideUI();

		if (winningPair == 0) Success();
		else Failure();
	}

	public void SelectPairTwo() {
		if (takenCorner) return;

		ballEndPoint = new Vector3(passingPoint[1].position.x,
								   passingPoint[1].position.y + 2.5f,
								   passingPoint[1].position.z);

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
		scoredGoal = true;
		takenCorner = true;
		Debug.Log("SCORED A GOAL!");
		MoveCameraToBall();
	}

	private void Failure() {
		scoredGoal = false;
		takenCorner = true;
		Debug.Log("OOPS!");
		MoveCameraToBall();
	}

	private void MoveCameraToBall() {
		StartCoroutine(MoveCameraToBallCoroutine());
	}

	private IEnumerator MoveCameraToBallCoroutine() {
		yield return Yielders.Get(1.5f);

		while (true) {
			float step = 5.0f * Vector3.Distance(Camera.main.transform.position, startCameraPosition) * 0.01f;

	    	Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, startCameraPosition, step);
	    	
	    	if (Vector3.Distance(Camera.main.transform.position, startCameraPosition) <= 0.05f) {
	    		break;
	    	}

	    	yield return Yielders.Get(0.01f);
	    }

	    KickBall();
	}

	private void KickBall() {
		StartCoroutine(KickBallCoroutine());
	}

	private IEnumerator KickBallCoroutine() {
		yield return Yielders.Get(0.5f);

		while (true) {
			float stepCam = 5.0f * Vector3.Distance(Camera.main.transform.position, endCameraPosition) * 0.01f;
			float stepBall = 5.0f * Vector3.Distance(ball.transform.position, ballEndPoint) * 0.02f;

	    	Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, endCameraPosition, stepCam);
	    	ball.transform.position = Vector3.MoveTowards(ball.transform.position, ballEndPoint, stepBall);

	    	if (Vector3.Distance(ball.transform.position, ballEndPoint) <= 0.2f) {
	    		break;
	    	}

	    	yield return Yielders.Get(0.01f);
		}

		if (scoredGoal) KickBallIntoGoal();
		else KickBallAwayFromGoal();
	}

	private void KickBallIntoGoal() {
		StartCoroutine(KickBallIntoGoalCoroutine());
    }

	private IEnumerator KickBallIntoGoalCoroutine() {
		while (true) {
			float step = 5.0f * Vector3.Distance(ball.transform.position, scoringPoint.position) * 0.03f;

	    	ball.transform.position = Vector3.MoveTowards(ball.transform.position, scoringPoint.position, step);

	    	if (Vector3.Distance(ball.transform.position, scoringPoint.position) <= 0.4f) {
	    		break;
	    	}

	    	yield return Yielders.Get(0.01f);
		}
	}

	private void KickBallAwayFromGoal() {
		StartCoroutine(KickBallAwayFromGoalCoroutine());
	}

	private IEnumerator KickBallAwayFromGoalCoroutine() {
		while (true) {
			float step = 5.0f * Vector3.Distance(ball.transform.position, knockAwayPoint.position) * 0.02f;

	    	ball.transform.position = Vector3.MoveTowards(ball.transform.position, knockAwayPoint.position, step);

	    	if (Vector3.Distance(ball.transform.position, knockAwayPoint.position) <= 0.4f) {
	    		break;
	    	}

	    	yield return Yielders.Get(0.01f);
		}
	}
}
