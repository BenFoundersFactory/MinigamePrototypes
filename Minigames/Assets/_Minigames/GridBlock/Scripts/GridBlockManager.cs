using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridBlockManager : MonoBehaviour {

    public static GridBlockManager instance;

    private const int GRID_WIDTH = 6;
    private const int GRID_HEIGHT = 10;

    private const float GRID_SIZE = 1.65f;

    [SerializeField] private GameObject ball;
    private Vector3 ballStartPos;

    [SerializeField] private GameObject gridBlockPoint;
    [SerializeField] private Transform gridStartPosition;
    [SerializeField] private Transform gridBlockParent;

    private GridBlockPoint chosenPoint;

    private List<GridBlockPoint> gbpList = new List<GridBlockPoint>();

    [SerializeField] private Text calloutText;

    private bool readyToBlock;

	void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
	}

	void Start () {
        ballStartPos = ball.transform.position;

        InitialSetup();

        StartGame();
	}

    private void InitialSetup() {
        calloutText.gameObject.SetActive(false);
        readyToBlock = false;

        for (int i = 0; i <= GRID_HEIGHT; i++) {
            for (int j = 0; j <= GRID_WIDTH; j++) {
                GameObject go = (GameObject)Instantiate(gridBlockPoint, gridStartPosition.position + new Vector3(j + (j * GRID_SIZE), i + (i * GRID_SIZE), 0), Quaternion.identity);
                go.transform.parent = gridBlockParent;

                GridBlockPoint gbp = go.GetComponent<GridBlockPoint>();
                gbp.SetupPoint((i * GRID_WIDTH) + j + i, j, i);
                go.transform.name = gbp.id.ToString();

                gbpList.Add(gbp);
            }
        }
    }

    private void StartGame() {
        StartCoroutine(StartGameCoroutine());
    }

    private IEnumerator StartGameCoroutine() {
        chosenPoint = gbpList[Random.Range(0, gbpList.Count)];

        ball.transform.position = ballStartPos;
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        yield return Yielders.Get(0.5f);

        calloutText.gameObject.SetActive(true);
        calloutText.text = "(" + chosenPoint.xCoord + ", " + chosenPoint.yCoord + ")";
        readyToBlock = true;
    }

    public void CheckSuccess(GridBlockPoint p) {
        if (!readyToBlock) return;

        readyToBlock = false;

        if (p.id == chosenPoint.id) Success(p.transform.position, true);
        else Failure(p.transform.position, false);
    }

    private void Success(Vector3 pos, bool success) {
        Debug.Log("Success!");
        GameManager.instance.score++;
        KickBallToPoint(pos, true);
    }

    private void Failure(Vector3 pos, bool success) {
        Debug.Log("OOPS!");
        KickBallToPoint(pos, false);
    }

    private void KickBallToPoint(Vector3 pos, bool success) {
        StartCoroutine(KickBallToPointCoroutine(pos, success));
    }

    private IEnumerator KickBallToPointCoroutine(Vector3 pos, bool success) {
        yield return Yielders.Get(0.5f);

        float speed = 2f;
        float time = 1 * speed;
        Transform ballTransform = ball.transform;
        float step = (Vector3.Distance(ballTransform.position, pos) / 100f) * speed;

        Vector3 dir = (pos - ballTransform.position).normalized;

        float counter = 0f;
        while (true) {
            counter += time / 100f;

            ballTransform.position = Vector3.MoveTowards(ballTransform.position, pos, step);

            if (counter >= 1f) break;

            yield return Yielders.Get(0.01f);
        }

        counter = 0f;
        if (!success) {
            calloutText.text = "OOPS!";

            step = (Vector3.Distance(ballTransform.position, pos + dir * 50f) / 100f) * speed;

            while (true) {
                counter += time / 100f;

                ballTransform.position = Vector3.MoveTowards(ballTransform.position, pos + dir * 50f, step);

                if (counter >= 1f) break;

                yield return Yielders.Get(0.01f);
            }
        } else {
            calloutText.text = "NICE BLOCK!";
            Vector3 forceToAdd = Camera.main.transform.forward + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);
            ball.GetComponent<Rigidbody>().AddForce(forceToAdd * 50f, ForceMode.Impulse);
        }
                

        yield return Yielders.Get(1f);

        RestartGame();
    }

    private void RestartGame() {
        StartCoroutine(RestartGameCoroutine());
    }

    private IEnumerator RestartGameCoroutine() {
        yield return Yielders.Get(1f);

        calloutText.gameObject.SetActive(false);

        StartGame();
    }
}
