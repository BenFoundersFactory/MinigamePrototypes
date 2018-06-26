using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrowInsManager : MonoBehaviour {

    private const float FRIENDLY_MOVE_MIN = -6f;
    private const float FRIENDLY_MOVE_MAX = 1f;

    [SerializeField] private GameObject calloutObject;
    [SerializeField] private GameObject[] answerBox;

	[SerializeField] private Text randomNumberText;
	[SerializeField] private Text[] answerTexts;

	private float calloutNumber;
	private float randomNumber;

	private List<float> randomDecimal = new List<float>();
	private List<int> randomPercentage = new List<int>();
	private List<Vector2> randomFraction = new List<Vector2>();

	private int correctAnswerBox = 0;
	private int selectedAnswer = 0;

	private int calloutType = 0;

	[SerializeField] private GameObject ball;
	[SerializeField] private GameObject friendly;
	[SerializeField] private GameObject opponent;

	private bool throwingBall = false;
    private bool gameStarted = false;

	void Start () {
        calloutObject.SetActive(false);
        for (int i = 0; i < answerBox.Length; i++) {
            answerBox[i].SetActive(false);
        }

		InitialiseValues();

		selectedAnswer = Random.Range(0, randomDecimal.Count);
		calloutType = Random.Range(0, 3);

		switch (calloutType) {
			case 0:
				randomNumberText.text = randomPercentage[selectedAnswer].ToString() + "%";
				break;
			case 1:
				randomNumberText.text = randomFraction[selectedAnswer].x.ToString() + "/" + randomFraction[selectedAnswer].y.ToString();
				break;
			case 2:
				randomNumberText.text = randomDecimal[selectedAnswer].ToString("0.00");
				break;
			default:
				break;
		}

		SetAnswers();

        StartGame();
	}

	void Update () {
        if (!throwingBall) friendly.transform.position = new Vector3(Mathf.Lerp(FRIENDLY_MOVE_MIN, FRIENDLY_MOVE_MAX, Mathf.Abs(Mathf.Sin(Time.time))), friendly.transform.position.y, friendly.transform.position.z);

        if (!gameStarted) return;
	}

    private void StartGame() {
        StartCoroutine(StartGameCoroutine());
    }

    private IEnumerator StartGameCoroutine() {
        yield return Yielders.Get(1f);

        calloutObject.SetActive(true);

        yield return Yielders.Get(0.5f);

        for (int i = 0; i < answerBox.Length; i++) {
            answerBox[i].SetActive(true);
        }

        gameStarted = true;
    }

	private void InitialiseValues() {
		for (int i = 1; i < 20; i++) {
			randomPercentage.Add(5 * i);
			randomFraction.Add(MathsUtil.GetSmallestFraction(i, 20));
			randomDecimal.Add(0.05f * i);
		}
	}

	private void SetAnswers() {
        HashSet<int> answerSet = new HashSet<int>();

        answerSet.Add(selectedAnswer);

		int randomType = Random.Range(0, 3);
		while (randomType == calloutType) randomType = Random.Range(0, 3);

		correctAnswerBox = Random.Range(0, answerTexts.Length);

		if (randomType == 0) answerTexts[correctAnswerBox].text = randomPercentage[selectedAnswer].ToString() + "%";
		else if (randomType == 1) answerTexts[correctAnswerBox].text = randomFraction[selectedAnswer].x.ToString() + "/" + randomFraction[selectedAnswer].y.ToString();
		else if (randomType == 2) answerTexts[correctAnswerBox].text = randomDecimal[selectedAnswer].ToString("0.00");

		for (int i = 0; i < answerTexts.Length; i++) {
			if (i == correctAnswerBox) continue;

			randomType = Random.Range(0, 2);
			while (randomType == calloutType) randomType = Random.Range(0, 3);

			int randomNumber = Random.Range(0, randomPercentage.Count);
            while (answerSet.Contains(randomNumber)) {
				randomNumber = Random.Range(0, randomPercentage.Count);
			}
            answerSet.Add(randomNumber);

			if (randomType == 0) answerTexts[i].text = randomPercentage[randomNumber].ToString() + "%";
			else if (randomType == 1) answerTexts[i].text = randomFraction[randomNumber].x.ToString() + "/" + randomFraction[randomNumber].y.ToString();
			else if (randomType == 2) answerTexts[i].text = randomDecimal[randomNumber].ToString("0.00");
		}
	}

	public void SelectAnswer(int answer) {
        if (throwingBall) return;

        StartCoroutine(StartThrowingCoroutine(answer));
	}

    private IEnumerator StartThrowingCoroutine(int answer) {
        for (int i = 0; i < answerBox.Length; i++) {
            if (i == answer) continue;
            answerBox[i].SetActive(false);
        }

        if (answer != -1) {
            answerBox[answer].gameObject.GetComponent<CanvasGroup>().interactable = false;

            while (true) {
                answerBox[answer].transform.localScale = new Vector3(answerBox[answer].transform.localScale.x + 0.01f,
                                                                     answerBox[answer].transform.localScale.y + 0.01f,
                                                                     answerBox[answer].transform.localScale.z + 0.01f);

                answerBox[answer].gameObject.GetComponent<CanvasGroup>().alpha -= 0.03f;

                if (answerBox[answer].gameObject.GetComponent<CanvasGroup>().alpha <= 0) break;

                yield return Yielders.Get(0.01f);
            }
        }

        yield return Yielders.Get(0.5f);

        throwingBall = true;

        if (answer != -1) answerBox[answer].SetActive(false);
        calloutObject.SetActive(false);

        if (correctAnswerBox == answer) Success();
        else Fail();
    }

	private void Success() {
		Debug.Log("SUCCESSFUL THROW IN!");

        Vector3 targetPos = new Vector3(Mathf.Lerp(FRIENDLY_MOVE_MIN, FRIENDLY_MOVE_MAX, randomPercentage[selectedAnswer]),
                                friendly.transform.position.y,
                                friendly.transform.position.z);

         MoveFriendlyToPos(true, targetPos);
	}

    private void Fail() {
        Debug.Log("OOPS!");

        Vector3 targetPos = new Vector3(Mathf.Lerp(FRIENDLY_MOVE_MIN, FRIENDLY_MOVE_MAX, randomPercentage[selectedAnswer]),
                                friendly.transform.position.y,
                                friendly.transform.position.z);

        MoveFriendlyToPos(false, targetPos);
    }

    private void MoveFriendlyToPos(bool success, Vector3 targetPos) {
        StartCoroutine(MoveFriendlyToPosCoroutine(success, targetPos));
    }

    private IEnumerator MoveFriendlyToPosCoroutine(bool success, Vector3 targetPos) {
        float speed = 4f;

        float dist = Vector3.Distance(friendly.transform.position, targetPos);
        float counter = 0;

        while (true) {
            counter += 0.01f;

            friendly.transform.position = Vector3.MoveTowards(friendly.transform.position, targetPos, dist * 0.01f * speed);

            if (counter >= 1f/speed) break;

            yield return Yielders.Get(0.01f);
        }

        ThrowBallToFriendly(success, friendly.transform.position);
    }

    private void ThrowBallToFriendly(bool success, Vector3 targetPos) {
        StartCoroutine(ThrowBallToFriendlyCoroutine(success, targetPos));
    }

    private IEnumerator ThrowBallToFriendlyCoroutine(bool success, Vector3 targetPos) {
        if (success) targetPos += new Vector3(0, 0, -2f);
        else targetPos += new Vector3(0, 0, -4f);

        float speed = 2f;

        float dist = Vector3.Distance(opponent.transform.position, friendly.transform.position);

        Vector3 opponentTargetPos = new Vector3(Mathf.Lerp(FRIENDLY_MOVE_MIN, FRIENDLY_MOVE_MAX, randomPercentage[selectedAnswer]),
                                opponent.transform.position.y,
                                opponent.transform.position.z);
        
        float counter = 0;
        while (true) {
            counter += 0.01f;

            if (!success) opponent.transform.position = Vector3.MoveTowards(opponent.transform.position, opponentTargetPos, counter * dist * speed);
            ball.transform.position = MathsUtil.GetBezierPosition(ball.transform.position, targetPos, counter * speed, 1f);

            if (counter >= 1f / speed) break;

            yield return Yielders.Get(0.01f);
        }
    }
}
