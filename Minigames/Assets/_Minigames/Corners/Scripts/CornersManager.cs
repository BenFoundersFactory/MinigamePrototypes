using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CornersManager : MonoBehaviour {

	[SerializeField] private Vector3 startCameraPosition;
	[SerializeField] private Vector3 endCameraPosition;

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

    private Vector3 ballStartPos;

	private Vector3 ballEndPoint;

    [SerializeField] private Image visualTimer;

	void Start () {
        ballStartPos = ball.transform.position;

        InitialiseValues();
        InitialSetup();
	}
	
	void Update () {
		if (!gameStarted) {
			float step = 5.0f * Vector3.Distance(Camera.main.transform.position, endCameraPosition) * Time.deltaTime;
        	Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, endCameraPosition, step);
        	
        	if (Vector3.Distance(Camera.main.transform.position, endCameraPosition) <= 0.05f) {
        		gameStarted = true;

                for (int i = 0; i < selectableHeight.Length; i++) {
                    selectableHeight[i].gameObject.SetActive(true);
                    opponentHeights[i].gameObject.SetActive(true);
                }

                visualTimer.gameObject.SetActive(true);
        	}

        	return;
        }

        if (!takenCorner) {
            counter -= Time.deltaTime;
            visualTimer.fillAmount = counter / timer;

            if (counter <= 0f) {
                Debug.Log("TOOK TOO LONG!");
                OutOfTime();
                Failure();
            }
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

    private void InitialSetup() {
        ball.transform.position = ballStartPos;

        takenCorner = false;
        gameStarted = false;
        scoredGoal = false;

        for (int i = 0; i < selectableHeight.Length; i++) {
            selectableHeight[i].gameObject.SetActive(false);
            opponentHeights[i].gameObject.SetActive(false);
        }

        visualTimer.gameObject.SetActive(false);

        counter = timer;
    }

    public void SelectAnswer(int answer) {
        if (takenCorner) return;

        visualTimer.gameObject.SetActive(false);

        ballEndPoint = new Vector3(passingPoint[answer].position.x,
                                   passingPoint[answer].position.y + 2.5f,
                                   passingPoint[answer].position.z);

        selectableHeight[0].PlaySelectAnimation(answer);
        selectableHeight[1].PlaySelectAnimation(answer);
        HideUI();

        if (winningPair == answer) Success();
        else Failure();
    }

    private void OutOfTime() {
        ballEndPoint = new Vector3(passingPoint[0].position.x,
                                   passingPoint[0].position.y + 2.5f,
                                   passingPoint[0].position.z);

        selectableHeight[0].PlaySelectAnimation(2);
        selectableHeight[1].PlaySelectAnimation(2);
        HideUI();
    }

	private void HideUI() {
		for (int i = 0; i < opponentHeights.Length; i++) {
			opponentHeights[i].SetActive(false);
		}
	}

	private void Success() {
		scoredGoal = true;
		takenCorner = true;
        GameManager.instance.score++;
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
            float stepBall = Mathf.Min(5.0f * Vector3.Distance(ball.transform.position, ballEndPoint) * 0.02f, 2f);

	    	Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, endCameraPosition, stepCam);
	    	ball.transform.position = Vector3.MoveTowards(ball.transform.position, ballEndPoint, stepBall);

	    	if (Vector3.Distance(ball.transform.position, ballEndPoint) <= 1f) {
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

        RestartGame();
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

        RestartGame();
	}

    private void RestartGame() {
        StartCoroutine(RestartGameCoroutine());
    }

    private IEnumerator RestartGameCoroutine() {
        yield return Yielders.Get(1f);

        while (true) {
            float step = 5.0f * Vector3.Distance(Camera.main.transform.position, startCameraPosition) * 0.01f;

            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, startCameraPosition, Mathf.Min(step, 1f));

            if (Vector3.Distance(Camera.main.transform.position, startCameraPosition) <= 0.05f) {
                break;
            }

            yield return Yielders.Get(0.01f);
        }

        InitialiseValues();
        InitialSetup();
    }
}
