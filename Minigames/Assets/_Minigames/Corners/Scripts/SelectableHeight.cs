using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableHeight : MonoBehaviour {

	private Transform associatedTransform;
	private CanvasGroup canvasGroup;

    [SerializeField] private int answer;

	void Awake () {
		associatedTransform = GetComponent<Transform>();
		canvasGroup = GetComponent<CanvasGroup>();
	}
	
	public void PlaySelectAnimation(int answer) {
        if (this.answer != answer) this.gameObject.SetActive(false);
		else StartCoroutine(PlaySelectAnimationCoroutine());
	}

	private IEnumerator PlaySelectAnimationCoroutine() {
		while (true) {
			associatedTransform.localScale = new Vector3(associatedTransform.localScale.x + 0.01f,
													associatedTransform.localScale.y + 0.01f,
													associatedTransform.localScale.z + 0.01f);

			canvasGroup.alpha -= 0.03f;

			if (canvasGroup.alpha <= 0) {
				this.gameObject.SetActive(false);
				break;
			}

			yield return Yielders.Get(0.01f);
		}
	}
}
