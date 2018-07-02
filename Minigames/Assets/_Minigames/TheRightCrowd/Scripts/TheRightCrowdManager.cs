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
        homeRandomRatio = new int[] { 1, 2, 3, 1, 2, 3, 4, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        awayRandomRatio = new int[] { 3, 2, 1, 4, 3, 2, 1, 9, 8, 7, 6, 5, 4, 3, 2, 1 };

        InitialSetup();
	}

    private void InitialSetup() {
        homeCount = 0;
        awayCount = 0;
        emptyCount = 0;

        resultsText.text = "";
        int randomNumber = Random.Range(0, homeRandomRatio.Length);

        homeRatio = homeRandomRatio[randomNumber];
        awayRatio = awayRandomRatio[randomNumber];

        homeRatioText.text = homeRatio.ToString();
        awayRatioText.text = awayRatio.ToString();

        for (int i = 0; i < seats.Count; i++) {
            seats[i].ResetColor();
            seats[i].currentColor = -1;
        }

        submitButton.interactable = true;
    }

    public void SubmitSeats() {
        submitButton.interactable = false;

        for (int i = 0; i < seats.Count; i++) {
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
