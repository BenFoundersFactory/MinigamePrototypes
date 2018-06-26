using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	void Awake() {
        DontDestroyOnLoad(this.gameObject);
	}

    public void LoadScene(string name) {
        SceneManager.LoadScene(name);
    }
}
