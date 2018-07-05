using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TheRightCrowdManager : MonoBehaviour {
    
    private int[] homeRandomRatio;
    private int[] awayRandomRatio;

    private int homeRatio;
    private int awayRatio;

    private int homeCount;
    private int awayCount;
    private int emptyCount;

    [SerializeField] private Text homeRatioText;
    [SerializeField] private Text awayRatioText;

    [SerializeField] private List<Seat> seats = new List<Seat>();
    [SerializeField] private Button submitButton;
    [SerializeField] private Text resultsText;

	void Start () {
        homeRandomRatio = new int[] { 9, 4, 7, 3, 1, 2, 3, 1, 1 };
        awayRandomRatio = new int[] { 1, 1, 3, 2, 1, 3, 7, 4, 9 };

        InitialSetup();
	}

    private void InitialSetup() {
        homeCount = 0;
        awayCount = 0;
        emptyCount = 0;

        resultsText.text = "";
        int randomNumber = Random.Range(0, homeRandomRatio.Length);

        int randomRemoveCount = 10 * Random.Range(0, 4);

        homeRatio = homeRandomRatio[randomNumber];
        awayRatio = awayRandomRatio[randomNumber];

        homeRatioText.text = homeRatio.ToString();
        awayRatioText.text = awayRatio.ToString();

        if (Random.Range(0, 2) == 1) {
            homeRatioText.text += "0%";
            awayRatioText.text += "0%";
        }

        for (int i = 0; i < seats.Count; i++) {
            seats[i].RandomColor();
            seats[i].isOn = true;
            seats[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < randomRemoveCount; i++) {
            seats[seats.Count - 1 - i].isOn = false;
            seats[seats.Count - 1 - i].gameObject.SetActive(false);
        }

        submitButton.interactable = true;
    }

    public void SubmitSeats() {
        submitButton.interactable = false;

        for (int i = 0; i < seats.Count; i++) {
            if (!seats[i].isOn) continue;

            if (seats[i].currentColor == 0) homeCount++;
            else if (seats[i].currentColor == 1) awayCount++;
            else emptyCount++;
        }

        if (homeCount != 0 && awayCount != 0 && emptyCount == 0 && homeCount % homeRatio == 0
            && awayCount % awayRatio == 0 && homeCount / homeRatio == awayCount / awayRatio) {
            GameManager.instance.score++;
            resultsText.text = "CORRECT!";
        } else {
            resultsText.text = "WRONG!";
        }
            

        RestartGame();
    }

    private void RestartGame() {
        StartCoroutine(RestartGameCoroutine());
    }

    private IEnumerator RestartGameCoroutine() {
        yield return Yielders.Get(1f);

        InitialSetup();
    }
}
