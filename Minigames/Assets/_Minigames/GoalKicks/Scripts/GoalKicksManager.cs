using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalKicksManager : MonoBehaviour {

    public static GoalKicksManager instance;

    private const int GRID_WIDTH = 10;
    private const int GRID_HEIGHT = 20;

    private const float GRID_SIZE = 1.65f;

    private Vector3 ballStartPosition;
    [SerializeField] private Transform cameraStartPosition;
    [SerializeField] private Transform cameraTopPosition;


    [SerializeField] private Transform gridStartPosition;
    [SerializeField] private GameObject goalKickPoint;
    [SerializeField] private Transform goalKickPointParent;

    private List<GoalKickPoint> gkpList = new List<GoalKickPoint>();
    private GoalKickPoint chosenPoint;

    [SerializeField] private Text calloutText;

    [SerializeField] private GameObject ball;
    [SerializeField] private GameObject friendly;
    [SerializeField] private GameObject opponent;

    private bool ballKicked;

	void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this.gameObject);
        }
	}

	void Start () {
        ballStartPosition = ball.transform.position;

        InitialSetup();

        StartGame();
	}

    private void InitialSetup() {
        calloutText.gameObject.SetActive(false);

        for (int i = 0; i <= GRID_HEIGHT; i++) {
            for (int j = 0; j <= GRID_WIDTH; j++) {
                GameObject go = (GameObject) Instantiate(goalKickPoint, gridStartPosition.position + new Vector3(j + (j*GRID_SIZE), 0, i + (i*GRID_SIZE)), Quaternion.identity);
                go.transform.parent = goalKickPointParent;

                GoalKickPoint gkp = go.GetComponent<GoalKickPoint>();
                gkp.SetupPoint((i * GRID_WIDTH) + j + i, j, i);
                go.transform.name = gkp.id.ToString();

                go.SetActive(false);

                gkpList.Add(gkp);
            }
        }
    }

    private void StartGame() {
        StartCoroutine(StartGameCoroutine());
    }

    private IEnumerator StartGameCoroutine() {
        yield return Yielders.Get(1f);

        chosenPoint = gkpList[Random.Range(0, gkpList.Count)];
        calloutText.text = "(" + chosenPoint.xCoord + ", " + chosenPoint.yCoord + ")";

        MoveCamera(cameraTopPosition, true);
    }

    private void MoveCamera(Transform targetPos, bool playing) {
        StartCoroutine(MoveCameraCoroutine(targetPos, playing));
    }

    private IEnumerator MoveCameraCoroutine(Transform targetPos, bool playing) {
        float speed = 1f;
        float time = 1 / speed;
        Transform camTransform = Camera.main.transform;

        float counter = 0f;
        while (true) {
            counter += time / 100f;

            camTransform.position = Vector3.Lerp(camTransform.position, targetPos.position, counter);
            camTransform.rotation = Quaternion.Lerp(camTransform.rotation, targetPos.rotation, counter);

            if (counter >= 1f) break;

            yield return Yielders.Get(0.01f);
        }

        SetAllPointsActive();

        if (playing) calloutText.gameObject.SetActive(true);
        else {
            RestartGame();
        }
    }

    private void SetAllPointsActive() {
        for (int i = 0; i < gkpList.Count; i++) {
            gkpList[i].gameObject.SetActive(true);
        }
    }

    public void CheckSuccess(GoalKickPoint p) {
        if (ballKicked) return;

        ballKicked = true;

        Vector3 pos = new Vector3(p.gameObject.transform.position.x, ball.transform.position.y, p.gameObject.transform.position.z);

        if (p.id == chosenPoint.id) Success(pos);
        else Failure(pos);
    }

    private void Success(Vector3 pos) {
        KickBallToPoint(pos, true);
        GameManager.instance.score++;
        Debug.Log("Success!");
    }

    private void Failure(Vector3 pos) {
        KickBallToPoint(pos, false);
        Debug.Log("OOPS!");
    }

    private void RestartGame() {
        ballKicked = false;
        StartGame();
    }

    private void KickBallToPoint(Vector3 pos, bool success) {
        StartCoroutine(KickBallToPointCoroutine(pos, success));
    }

    private IEnumerator KickBallToPointCoroutine(Vector3 pos, bool success) {
        yield return Yielders.Get(0.5f);

        float speed = 2f;
        float time = 1 / speed;
        Transform ballTransform = ball.transform;

        float counter = 0f;
        while (true) {
            counter += time / 100f;

            ballTransform.position = MathsUtil.GetBezierPosition(ballTransform.position, pos, counter, 2.5f);
            if (success) friendly.transform.position = Vector3.Lerp(friendly.transform.position, pos, counter);
            else opponent.transform.position = Vector3.Lerp(opponent.transform.position, pos, counter);

            if (counter >= 1f) break;

            yield return Yielders.Get(0.01f);
        }

        yield return Yielders.Get(0.5f);

        calloutText.gameObject.SetActive(false);
        ball.transform.position = ballStartPosition;

        MoveCamera(cameraStartPosition, false);
    }

}
