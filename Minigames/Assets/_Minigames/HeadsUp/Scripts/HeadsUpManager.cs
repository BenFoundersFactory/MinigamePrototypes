using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadsUpManager : MonoBehaviour {

    private const float WEAK_HEAD_UP = 8f;

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
    [SerializeField] private Text multipleMessage;

    private bool isPlaying = false;
    private bool canKick = false;
    private bool ballHitGround = false;

	void Start () {
        InitialSetup();

        StartGame();
	}

	void Update () {
        if (!isPlaying) return;

        if (Input.GetMouseButtonDown(0)) {
            if (ballHitGround) {
                if (!canKick) return;
                canKick = false;
                HeadUp();
                ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                ball.GetComponent<Rigidbody>().AddForce(Vector3.up * STRONG_HEAD_UP, ForceMode.Impulse);

                return;
            }
        }

        ball.GetComponent<Rigidbody>().AddForce(-Vector3.up * 2f);

        if (Input.GetMouseButtonDown(0)) {
            if (!canKick) return;
            canKick = false;
            HeadUp();
            if (ball.transform.position.y < 3.5f && ball.transform.position.y > 0.6f) {
                ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                ball.GetComponent<Rigidbody>().AddForce(Vector3.up * WEAK_HEAD_UP, ForceMode.Impulse);

                powerFill += 0.1f;
                powerSlider.value = powerFill;

                count++;
                countText.text = count.ToString();

                if (count % target == 0) PlayMultipleMessage("MISSED MULTIPLE!");

                multipleShowText.text = "";

                if (powerFill >= 1f) Success();
            }
        } else if (Input.GetMouseButtonDown(1)) {
            if (!canKick) return;
            canKick = false;
            HeadUp();
            if (ball.transform.position.y < 3.5f && ball.transform.position.y > 0.6f) {
                ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                ball.GetComponent<Rigidbody>().AddForce(Vector3.up * STRONG_HEAD_UP, ForceMode.Impulse);

                if ((count + 1) % target == 0) {
                    PlayMultipleMessage((count + 1).ToString() + "!");
                    powerFill += 0.2f;
                    multipleShowText.text = target.ToString() + "x" + (target / (count + 1)).ToString() + "!";
                } else powerFill += 0.1f;

                powerSlider.value = powerFill;

                count++;
                countText.text = count.ToString();

                if (count % target != 0) PlayMultipleMessage("TOO EARLY!");

                if (powerFill >= 1f) Success();
            }
        }

        if (!ballHitGround && ball.transform.position.y <= 0.6f) {
            powerFill -= 0.1f;
            powerSlider.value = powerFill;
            ballHitGround = true;
        }
	}

    private void InitialSetup() {
        target = Random.Range(2, 9);
        count = 0;
        powerFill = 0;
        powerSlider.value = powerFill;
        canKick = true;
        ballHitGround = false;
        multipleMessage.gameObject.SetActive(false);

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

    private void PlayMultipleMessage(string message) {
        StartCoroutine(PlayMultipleMessageCoroutine(message));
    }

    private IEnumerator PlayMultipleMessageCoroutine(string message) {
        Transform mmTransform = multipleMessage.transform;
        CanvasGroup mmCanvasGroup = multipleMessage.gameObject.GetComponent<CanvasGroup>();

        multipleMessage.gameObject.SetActive(true);

        multipleMessage.text = message;
        mmTransform.localScale = new Vector3(1f, 1f, 1f);
        mmCanvasGroup.alpha = 1f;

        float counter = 0f;
        while (true) {
            counter += Time.deltaTime;

            Vector3 scale = mmTransform.localScale;
            scale += new Vector3(Time.deltaTime, Time.deltaTime, Time.deltaTime);
            mmTransform.localScale = scale;

            mmCanvasGroup.alpha -= Time.deltaTime;

            if (counter >= 1f) break;

            yield return Yielders.Get(0.01f);
        }

        multipleMessage.gameObject.SetActive(false);
    }

    private void HeadUp() {
        StartCoroutine(HeadUpCoroutine());
    }

    private IEnumerator HeadUpCoroutine() {
        ballHitGround = false;

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
