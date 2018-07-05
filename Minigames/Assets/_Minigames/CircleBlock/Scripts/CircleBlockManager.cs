using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleBlockManager : MonoBehaviour {

    public static CircleBlockManager instance;

    private int DIFFICULTY_SCALE = 0;

    private int CALLOUT_INCREMENT = 5;
    public float PROTRACTOR_FILL = 1f;

    [SerializeField] private GameObject opponent;
    [SerializeField] private GameObject ball;
    [SerializeField] private GameObject player;
    [SerializeField] private Text degreeText;

    [SerializeField] private GameObject calloutBox;
    [SerializeField] private Text calloutText;

    private float timer = 10f;
    private float counter = 0f;
    [SerializeField] private Image visualTimer;

    private int calloutDegree;
    private int clockwise;

    private bool gameStarted;
    public bool ballShot;

    [SerializeField] private GameObject bonusCoin;
    [SerializeField] private Image protractor;

    [SerializeField] private Transform ballReceiver;

    // Swipe controls
    //private bool swipeStarted;
    //private float speedX = 0f;
    //private float playerSwipeRotationSpeed = 0.3f;

	void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
	}

	void Start() {
        SetDifficultyScale();

        player.transform.eulerAngles = new Vector3(0, Random.Range(0, 360 * CircleBlockManager.instance.PROTRACTOR_FILL), 0);

        InitialSetup();

        StartGame();
    }

    void Update() {
        if (gameStarted) {
            counter -= Time.deltaTime;
            visualTimer.fillAmount = counter / timer;

            if (counter <= 0f) {
                gameStarted = false;
                ballShot = true;
                RotateOpponent();
            }
        }
    }

    /*
    private void SwipeControls() {
        for (int i = 0; i < Input.touchCount; i++) {

            Touch touch = Input.GetTouch(i);

            if (!swipeStarted && touch.phase == TouchPhase.Began) {
                playerDegreeText.gameObject.SetActive(true);
                swipeStarted = true;
            }

            if (touch.phase == TouchPhase.Moved && swipeStarted) {
                if (touch.deltaPosition.x * speedX < 0) speedX = 0;

                speedX = touch.deltaPosition.x;
            }

            if (swipeStarted) {
                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) {
                    speedX = 0f;
                    swipeStarted = false;
                    playerDegreeText.gameObject.SetActive(false);
                }
            }
        }

        player.transform.Rotate(Vector3.up * speedX * playerSwipeRotationSpeed);
    }*/

    private void SetDifficultyScale() {
        switch (DIFFICULTY_SCALE) {
            case 0:
                CALLOUT_INCREMENT = 10;
                PROTRACTOR_FILL = 0.25f;
                break;
            case 1:
                CALLOUT_INCREMENT = 10;
                PROTRACTOR_FILL = 0.5f;
                break;
            case 2:
                CALLOUT_INCREMENT = 10;
                PROTRACTOR_FILL = 1f;
                break;
            case 3:
                CALLOUT_INCREMENT = 5;
                PROTRACTOR_FILL = 0.25f;
                break;
            case 4:
                CALLOUT_INCREMENT = 5;
                PROTRACTOR_FILL = 0.5f;
                break;
            case 5:
                CALLOUT_INCREMENT = 5;
                PROTRACTOR_FILL = 1f;
                break;
        }

        protractor.fillAmount = PROTRACTOR_FILL;
    }

	private void StartGame() {
        StartCoroutine(StartGameCoroutine());
    }

    private IEnumerator StartGameCoroutine() {
        yield return Yielders.Get(1f);

        calloutBox.SetActive(true);
        visualTimer.gameObject.SetActive(true);

        GetRandomCall();

        yield return Yielders.Get(0.5f);

        gameStarted = true;
    }

    private void InitialSetup() {
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        ball.transform.position = opponent.transform.position + (opponent.transform.forward * 0.8f);

        bonusCoin.transform.eulerAngles = new Vector3(0, Random.Range(0, 360 * PROTRACTOR_FILL), 0);
        bonusCoin.transform.GetChild(0).gameObject.SetActive(true);

        player.transform.GetChild(0).GetComponent<CapsuleCollider>().enabled = true;

        calloutBox.SetActive(false);
        visualTimer.gameObject.SetActive(false);

        counter = timer;
        gameStarted = false;
        ballShot = false;

        //swipeStarted = false;
        //speedX = 0f;
    }

    private void GetRandomCall() {
        calloutDegree = CALLOUT_INCREMENT * Random.Range(1, (int) ((360/CALLOUT_INCREMENT) * PROTRACTOR_FILL));

        calloutDegree = calloutDegree - (int) opponent.transform.eulerAngles.y;
        calloutDegree = calloutDegree - (calloutDegree % CALLOUT_INCREMENT);

        calloutText.text = calloutDegree.ToString() + "!";
    }

    private void RotateOpponent() {
        LockPlayerPosition();

        StartCoroutine(RotateOpponentCoroutine());
    }

    private IEnumerator RotateOpponentCoroutine() {
        float finalRotation = opponent.transform.eulerAngles.y + calloutDegree;

        float rotationSpeed = 3f;
        float count = 0f;

        while (true) {
            count += 0.01f * rotationSpeed;

            ballReceiver.rotation = Quaternion.Lerp(ballReceiver.rotation, Quaternion.Euler(0f, finalRotation, 0f), count);

            opponent.transform.Rotate(Vector3.up * calloutDegree * 0.01f * rotationSpeed);
            degreeText.text = opponent.transform.eulerAngles.y.ToString("F0");

            if (count >= 1f) {
                opponent.transform.eulerAngles = new Vector3(0, finalRotation, 0);
                degreeText.text = opponent.transform.eulerAngles.y.ToString("F0");
                break;
            }

            yield return Yielders.Get(0.01f);
        }

        CheckSuccess();
    }

    private void CheckSuccess() {
        ball.GetComponent<Rigidbody>().AddForce(opponent.transform.forward * 30f, ForceMode.Impulse);

        if ((int) player.transform.eulerAngles.y == (int)opponent.transform.eulerAngles.y) Success();
        else Failure();

        RestartGame();
    }

    private void Success() {
        Debug.Log("success");
        player.transform.GetChild(0).GetComponent<CapsuleCollider>().enabled = true;
        GameManager.instance.score++;
    }

    private void Failure() {
        Debug.Log("oops!");
        player.transform.GetChild(0).GetComponent<CapsuleCollider>().enabled = false;
    }

    private void LockPlayerPosition() {
        float rounding = player.transform.eulerAngles.y % CALLOUT_INCREMENT;

        if (rounding < 3) player.transform.eulerAngles = new Vector3(0, player.transform.eulerAngles.y - rounding, 0);
        else player.transform.eulerAngles = new Vector3(0, player.transform.eulerAngles.y + CALLOUT_INCREMENT - rounding, 0);
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
