using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MakeItRainManager : MonoBehaviour {

    private int DIFFICULTY_SCALE = 0;

    private const int MAX_WAGE = 100000;
    private float WAGE_MULTIPLES = 0.05f;

    private const float MAX_BONUS = 1;
    private float BONUS_MULTIPLES = 0.1f;

    public static MakeItRainManager instance;

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

    [SerializeField] private Text totaliserAnswer;
    [SerializeField] private Slider totaliser;

    public bool gameStarted = false;

    void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
	}

	void Start () {
        SetDifficultyScale();

        InitialSetup();

        StartGame();
	}
	
	void Update () {
        if (!gameStarted) return;

        counter -= Time.deltaTime;
        visualTimer.fillAmount = counter / timer;

        if (counter <= 0f) EndGame();
	}

    private void SetDifficultyScale() {
        switch (DIFFICULTY_SCALE) {
            case 0:
                WAGE_MULTIPLES = 0.10f;
                BONUS_MULTIPLES = 0.10f;
                totaliserAnswer.gameObject.SetActive(true);
                totaliser.gameObject.SetActive(true);
                break;
            case 1:
                WAGE_MULTIPLES = 0.05f;
                BONUS_MULTIPLES = 0.05f;
                totaliserAnswer.gameObject.SetActive(true);
                totaliser.gameObject.SetActive(true);
                break;
            case 2:
                WAGE_MULTIPLES = 0.05f;
                BONUS_MULTIPLES = 0.05f;
                totaliserAnswer.gameObject.SetActive(false);
                totaliser.gameObject.SetActive(true);
                break;
            case 3:
                WAGE_MULTIPLES = 0.05f;
                BONUS_MULTIPLES = 0.05f;
                totaliserAnswer.gameObject.SetActive(false);
                totaliser.gameObject.SetActive(false);
                break;
            case 4:
                WAGE_MULTIPLES = 0.01f;
                BONUS_MULTIPLES = 0.01f;
                totaliserAnswer.gameObject.SetActive(false);
                totaliser.gameObject.SetActive(false);
                break;
        }
    }

    private void InitialSetup() {
        gameStarted = false;
        counter = timer;
        moneyCountText.text = "£0";
        resultText.text = "";
        totaliserAnswer.text = "???";
        moneyCount = 0;

        wage = (int) Mathf.Ceil((MAX_WAGE * (WAGE_MULTIPLES * Random.Range(1, (int)(1 / WAGE_MULTIPLES + 1)))));
        bonus = MAX_BONUS * (BONUS_MULTIPLES * Random.Range(1, (int) (1 / BONUS_MULTIPLES + 1)));

        answer = (int) Mathf.Ceil(wage * bonus);
        totaliser.value = (float)(moneyCount) / answer;

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
        totaliserAnswer.text = "£" + answer.ToString();
    }

    public void AddAmount(int amount) {
        moneyCount += amount;
        moneyCountText.text = "£" + moneyCount.ToString();
        totaliser.value = (float) (moneyCount) / answer;

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

        SetDifficultyScale();
        InitialSetup();
        StartGame();
    }

    public void SetDifficulty(Dropdown dd) {
        DIFFICULTY_SCALE = dd.value;
    }
}
