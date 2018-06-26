using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    private float startTime = 10f;

    public float timer = 10f;
    public int score = 0;

    public string currentGame = "";

    private bool playingGame = false;

	void Awake() {
        if (instance == null) {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        } else {
            Destroy(this.gameObject);
        }
	}

    void Start() {
        currentGame = "";
        timer = startTime;
        score = 0;
        playingGame = false;
	}

	void Update() {
        if (!playingGame) return;

        timer -= Time.deltaTime;

        if (timer <= 0f) {
            playingGame = false;
            SceneManager.LoadScene("ScoreScreen");
        }
	}

	public void LoadScene(string name) {
        currentGame = name;
        timer = startTime;
        score = 0;
        if (name != "ScoreScreen") playingGame = true;
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
        SceneManager.LoadScene("MainGame");
    }


}
