using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MakeItRainManager : MonoBehaviour {

    public static MakeItRainManager instance;

    private const int MAX_WAGE = 100000;
    private const float WAGE_MULTIPLES = 0.05f;

    private const float MAX_BONUS = 1;
    private const float BONUS_MULTIPLES = 0.1f;

    [SerializeField] private Text wageText;
    [SerializeField] private Text bonusText;
    [SerializeField] private Text moneyCountText;
    [SerializeField] private Text resultText;

    private int wage;
    private float bonus;

    private int answer;
    private int moneyCount;

    private float timer = 15f;
    private float counter = 0f;
    [SerializeField] private Image visualTimer;

    public bool gameStarted = false;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
	}

	void Start () {
        InitialSetup();

        StartGame();
	}
	
	void Update () {
        if (!gameStarted) return;

        counter -= Time.deltaTime;
        visualTimer.fillAmount = counter / timer;

        if (counter <= 0f) EndGame();
	}

    private void InitialSetup() {
        gameStarted = false;
        counter = timer;
        moneyCountText.text = "£0";
        resultText.text = "";
        moneyCount = 0;

        wage = (int) (MAX_WAGE * WAGE_MULTIPLES * Random.Range(1, (int)(1 / WAGE_MULTIPLES + 1)));
        bonus = MAX_BONUS * BONUS_MULTIPLES * Random.Range(1, (int) (0.5 / BONUS_MULTIPLES + 1));

        answer = (int) (wage * bonus);

        wageText.text = "Wage: ???";
        bonusText.text = "Bonus: ???";
    }

    private void StartGame() {
        StartCoroutine(StartGameCoroutine());
    }

    private IEnumerator StartGameCoroutine() {
        yield return Yielders.Get(1f);

        gameStarted = true;
        wageText.text = "Wage: " + "£" + wage.ToString();
        bonusText.text = "Bonus: " + (bonus * 100).ToString() + "%";
    }

    public void AddAmount(int amount) {
        moneyCount += amount;
        moneyCountText.text = "£" + moneyCount.ToString();

        if (moneyCount >= answer) EndGame();
    }

    private void EndGame() {
        gameStarted = false;

        if (moneyCount == answer) {
            resultText.text = "Nice!";
            GameManager.instance.score++;
        } else if (moneyCount > answer) resultText.text = "Too Much!";
        else resultText.text = "Too Little!";

        RestartGame();
    }

    private void RestartGame() {
        StartCoroutine(RestartGameCoroutine());
    }

    private IEnumerator RestartGameCoroutine() {
        yield return Yielders.Get(1f);

        InitialSetup();
        StartGame();
    }
}
