using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleBlockManager : MonoBehaviour {

    private float playerRotationSpeed = 70f;

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
    private bool ballShot;

    [SerializeField] private GameObject bonusCoin;

	void Start () {
        player.transform.eulerAngles = new Vector3(0, 180f, 0);

        InitialSetup();

        StartGame();
	}
	
	void Update () {
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

	void FixedUpdate() {
        if (!ballShot) {
            if (Input.GetMouseButton(0)) {
                if (Input.GetAxis("Mouse X") > 0 || Input.GetAxis("Mouse X") < 0) {
                    player.transform.Rotate(Vector3.up * Input.GetAxis("Mouse X") * playerRotationSpeed);
                }
            }
        }
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
        ball.transform.position = opponent.transform.position + (opponent.transform.forward * 0.7f);
        bonusCoin.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
        bonusCoin.transform.GetChild(0).gameObject.SetActive(true);
        player.transform.GetChild(0).GetComponent<CapsuleCollider>().enabled = true;

        calloutBox.SetActive(false);
        visualTimer.gameObject.SetActive(false);

        counter = timer;
        gameStarted = false;
        ballShot = false;
    }

    private void GetRandomCall() {
        clockwise = Random.Range(0, 2);
        calloutDegree = 5 * Random.Range(5, 72);

        if (clockwise == 1) calloutDegree *= -1;

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

        if (player.transform.eulerAngles.y == opponent.transform.eulerAngles.y) Success();
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
        float rounding = player.transform.eulerAngles.y % 5;

        if (rounding < 3) player.transform.eulerAngles = new Vector3(0, player.transform.eulerAngles.y - rounding, 0);
        else player.transform.eulerAngles = new Vector3(0, player.transform.eulerAngles.y + 5 - rounding, 0);
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
