using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadsUpManager : MonoBehaviour {

    private const float WEAK_HEAD_UP = 7f;

    private const float STRONG_HEAD_UP = 12f;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject ball;

    [SerializeField] private Text targetText;
    [SerializeField] private Text countText;
    [SerializeField] private Text multipleShowText;

    private int target;
    private int count;

    [SerializeField] private Slider powerSlider;
    private float powerFill;

    [SerializeField] private Text successMessage;

    private bool isPlaying = false;
    private bool canKick = false;

	void Start () {
        InitialSetup();

        StartGame();
	}

	void Update () {
        if (!isPlaying) return;

        ball.GetComponent<Rigidbody>().AddForce(-Vector3.up * 2f);

        if (Input.GetMouseButtonDown(0)) {
            if (!canKick) return;
            canKick = false;
            HeadUp();
            if (ball.transform.position.y < 3.5f && ball.transform.position.y > 0.5f) {
                ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                ball.GetComponent<Rigidbody>().AddForce(Vector3.up * WEAK_HEAD_UP, ForceMode.Impulse);

                powerFill += 0.1f;
                powerSlider.value = powerFill;

                count++;
                countText.text = count.ToString();

                multipleShowText.text = "";

                if (powerFill >= 1f) Success();
            }
        } else if (Input.GetMouseButtonDown(1)) {
            if (!canKick) return;
            canKick = false;
            HeadUp();
            if (ball.transform.position.y < 3.5f && ball.transform.position.y > 0.5f) {
                ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                ball.GetComponent<Rigidbody>().AddForce(Vector3.up * STRONG_HEAD_UP, ForceMode.Impulse);

                if ((count + 1) % target == 0) {
                    powerFill += 0.2f;
                    multipleShowText.text = target.ToString() + "x" + (target / (count + 1)).ToString() + "!";
                } else powerFill += 0.1f;

                powerSlider.value = powerFill;

                count++;
                countText.text = count.ToString();

                if (powerFill >= 1f) Success();
            }
        }

        if (ball.transform.position.y <= 0.5f) {
            Failure();
        }
	}

    private void InitialSetup() {
        target = Random.Range(2, 9);
        count = 0;
        powerFill = 0;
        powerSlider.value = powerFill;
        canKick = true;

        targetText.text = target.ToString();
        countText.text = count.ToString();
        multipleShowText.text = "";
        successMessage.text = "";
    }

    private void StartGame() {
        StartCoroutine(StartGameCoroutine());
    }

    private IEnumerator StartGameCoroutine() {
        yield return new WaitUntil(() => ball.transform.position.y <= 0.5f);

        yield return Yielders.Get(0.5f);

        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().AddForce(Vector3.up * STRONG_HEAD_UP, ForceMode.Impulse);

        yield return Yielders.Get(0.25f);

        isPlaying = true;
    }

    private void Success() {
        GameManager.instance.score++;
        successMessage.text = "NICE!";
        EndGame();
    }

    private void Failure() {
        successMessage.text = "OOPS!";
        EndGame();
    }

    private void EndGame() {
        StartCoroutine(EndGameCoroutine());
    }

    private IEnumerator EndGameCoroutine() {
        isPlaying = false;

        yield return Yielders.Get(0.5f);

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

    private void HeadUp() {
        StartCoroutine(HeadUpCoroutine());
    }

    private IEnumerator HeadUpCoroutine() {
        float speed = 3f;

        Transform playerTransform = player.transform;
        Vector3 targetPos = playerTransform.position + new Vector3(0f, 1f, 0f);

        float counter = 0f;
        while (true) {
            counter += Time.deltaTime * speed;

            playerTransform.position = Vector3.MoveTowards(playerTransform.position, targetPos, Time.deltaTime * speed);

            if (counter >= 1f) break;

            yield return Yielders.Get(0.01f);
        }

        counter = 0f;
        targetPos = playerTransform.position - new Vector3(0f, 1f, 0f);
        while (true) {
            counter += Time.deltaTime * speed;

            playerTransform.position = Vector3.MoveTowards(playerTransform.position, targetPos, Time.deltaTime * speed);

            if (counter >= 1f) break;

            yield return Yielders.Get(0.01f);
        }

        canKick = true;
    }   
}
