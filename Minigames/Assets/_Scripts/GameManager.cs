using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    private float startTime = 60f;

    public float timer = 0f;
    public int score = 0;

    public string currentGame = "";

    private bool playingGame = false;

    [SerializeField] private GameObject timeAndScoreCanvas;
    [SerializeField] private Text scoreText;
    [SerializeField] private Text timeText;

	void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);;
       } else {
            Destroy(this.gameObject);
        }
	}

    void Start() {
        currentGame = "";
        timer = startTime;
        score = 0;
        playingGame = false;

        timeAndScoreCanvas.SetActive(false);
	}

	void Update() {
        if (!playingGame) return;

        timer -= Time.deltaTime;

        if (timer <= 0f) {
            playingGame = false;
            SceneManager.LoadScene("ScoreScreen");
        }

        timeText.text = "Time: " + timer.ToString("00");
        scoreText.text = "Score: " + score.ToString();
	}

	public void LoadScene(string name) {
        currentGame = name;
        timer = startTime;
        score = 0;
        if (name != "ScoreScreen") {
            playingGame = true;
            timeAndScoreCanvas.SetActive(true);
        }
        SceneManager.LoadScene(name);
    }

    public void Replay() {
        LoadScene(currentGame);
    }

    public void Finish() {
        currentGame = "";
        timer = startTime;
        score = 0;
        playingGame = false;
        timeAndScoreCanvas.SetActive(false);
        SceneManager.LoadScene("MainGame");
    }


}
