using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreScreenManager : MonoBehaviour {
    
    [SerializeField] private Text score;

	void Start () {
        score.text = "SCORE: " + GameManager.instance.score.ToString();
	}

    public void Replay() {
        GameManager.instance.LoadScene(GameManager.instance.currentGame);
    }

    public void Finish() {
        GameManager.instance.Finish();
    }
	
}
