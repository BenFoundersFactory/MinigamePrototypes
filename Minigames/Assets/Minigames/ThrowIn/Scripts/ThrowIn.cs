using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrowIn : MonoBehaviour {

	[SerializeField] private Text randomNumberText;
	[SerializeField] private Text[] answerTexts;

	private float calloutNumber;
	private float randomNumber;

	private List<float> randomDecimal = new List<float>();
	private List<int> randomPercentage = new List<int>();
	private List<Vector2> randomFraction = new List<Vector2>();

	private float timer = 5f;
	private float counter = 0f;

	private int correctAnswerBox = 0;
	private int selectedAnswer = 0;

	private int calloutType = 0;

	[SerializeField] private GameObject ball;
	[SerializeField] private GameObject friendly;
	[SerializeField] private GameObject opponent;

	private bool throwingBall = false;

	void Start () {
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
	}

	void Update () {
		counter += Time.deltaTime;

		if (counter >= timer && !throwingBall) {
			Debug.Log("OOPS! TOOK TOO LONG!");
			throwingBall = true;
		}

		if (!throwingBall) friendly.transform.position = new Vector3(Mathf.Lerp(-6f, 6f, Mathf.Abs(Mathf.Sin(Time.time))), friendly.transform.position.y, friendly.transform.position.z);
	}

	private void InitialiseValues() {
		for (int i = 1; i < 20; i++) {
			randomPercentage.Add(5 * i);
			randomFraction.Add(GetSmallestFraction(i, 20));
			randomDecimal.Add(0.05f * i);
		}
	}

	private void SetAnswers() {
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
			while (randomPercentage[randomNumber] == randomPercentage[selectedAnswer]) {
				randomNumber = Random.Range(0, randomPercentage.Count);
			}

			if (randomType == 0) answerTexts[i].text = randomPercentage[randomNumber].ToString() + "%";
			else if (randomType == 1) answerTexts[i].text = randomFraction[randomNumber].x.ToString() + "/" + randomFraction[randomNumber].y.ToString();
			else if (randomType == 2) answerTexts[i].text = randomDecimal[randomNumber].ToString("0.00");
		}
	}

	public void SelectAnswerOne() {
		throwingBall = true;

		if (correctAnswerBox == 0) Success();
		else Fail();
	}

	public void SelectAnswerTwo() {
		throwingBall = true;

		if (correctAnswerBox == 1) Success();
		else Fail();
	}

	public void SelectAnswerThree() {
		throwingBall = true;

		if (correctAnswerBox == 2) Success();
		else Fail();
	}

	private void Success() {
		Debug.Log("SUCCESSFUL THROW IN!");
	}

	private void Fail() {
		Debug.Log("OOPS!");
	}


	private Vector2 GetSmallestFraction(int x, int y) {
		int divider = x;

		while (divider > 0) {
			if (y % divider == 0 && x % divider == 0) {
				return new Vector2(x/divider, y/divider);
			}

			divider--;
		}

		return new Vector2(x, y);
	}
}
