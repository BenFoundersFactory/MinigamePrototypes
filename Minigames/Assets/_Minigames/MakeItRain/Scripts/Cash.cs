using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cash : MonoBehaviour {

    [SerializeField] private int amount;

    [SerializeField] private ParticleSystem moneyEmitter;

	void OnMouseDown() {
        if (!MakeItRainManager.instance.gameStarted) return;

        MakeItRainManager.instance.AddAmount(amount);
        moneyEmitter.Emit(1);
	}
}
