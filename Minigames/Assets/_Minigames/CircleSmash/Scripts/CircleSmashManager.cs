using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircleSmashManager : MonoBehaviour {

    public static CircleSmashManager instance;

    private const int MIN_NUMBER_OF_CUBES = 3;
    private const int MAX_NUMBER_OF_CUBES = 15;

    [SerializeField] private Text chainText;

    [SerializeField] private GameObject player;
    [SerializeField] private GameObject ball;
    private float rotationAmount;

    [SerializeField] private Transform smashCubeParent;
    [SerializeField] private GameObject smashCube;
    private int selectedStartCube;

    private int numberOfCubes;
    private List<GameObject> cubeObjects = new List<GameObject>();
    private List<CircleSmashCube> cubeList = new List<CircleSmashCube>();

    private int cubeCount;
    private int chainCount;
    private int smashedCount;
    private int roundCount;
    private int startCube;

    private bool ballKicked;

    private int accumulatedScore;

	void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this.gameObject);
        }
	}

	void Start () {
        ballKicked = false;

        InitialSetup();

        StartGame();
	}

    private void InitialSetup() {
        cubeCount = 0;
        chainCount = 0;
        smashedCount = 0;
        roundCount = 0;
        startCube = 0;

        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        numberOfCubes = Random.Range(MIN_NUMBER_OF_CUBES, MAX_NUMBER_OF_CUBES);

        rotationAmount = 360f / numberOfCubes;

        for (int i = 0; i < numberOfCubes; i++) {
            GameObject go = (GameObject) Instantiate(smashCube, smashCubeParent.position, Quaternion.identity);
            go.transform.parent = smashCubeParent;
            go.transform.localPosition = Vector3.zero;
            go.transform.eulerAngles = new Vector3(0f, rotationAmount * i, 0f);

            CircleSmashCube csc = go.transform.GetChild(0).GetComponent<CircleSmashCube>();
            csc.SetupCube(i);
            cubeObjects.Add(go);
            cubeList.Add(csc);
        }
    }

    private void StartGame() {
        for (int i = 0; i < cubeList.Count; i++) {
            cubeList[i].countText.text = " ";
        }

        cubeCount = 0;
        accumulatedScore = 0;
        chainCount = 0;
        chainText.text = "";

        for (int i = 0; i < cubeList.Count; i++) {
            if (!cubeList[i].smashed) {
                cubeList[i].countText.text = "0";

                Quaternion targetRotation = Quaternion.Euler(0f, i * rotationAmount, 0f);
                player.transform.rotation = targetRotation;

                startCube = i;

                break;
            }
        }

        ballKicked = false;
    }

    public void SelectCube(int id) {
        if (id == startCube || ballKicked) return;

        ballKicked = true;

        selectedStartCube = id - startCube;
        cubeCount += startCube;

        RotatePlayer(id);
    }

    public int GetCount() {
        return cubeCount - startCube;
    }

    private void RotatePlayer(int id) {
        StartCoroutine(RotatePlayerCoroutine(id));
    }

    private IEnumerator RotatePlayerCoroutine(int id) {
        Quaternion targetRotation = Quaternion.Euler(0f, (id * rotationAmount), 0f);

        float speed = 3f;
        float time = 1 * speed;

        float counter = 0f;

        float totalRotation =(id * rotationAmount) - player.transform.eulerAngles.y;
        if (totalRotation < 0) totalRotation += 360;

        float rotationTick = (totalRotation * (time / 100f));

        while (true) {
            counter += time / 100f;

            player.transform.Rotate(Vector3.up * rotationTick);

            if (counter >= 1f) {
                player.transform.rotation = targetRotation;
                break;
            }

            yield return Yielders.Get(0.01f);
        }

        KickBall();
    }

    private void KickBall() {
        StartCoroutine(KickBallCoroutine());
    }

    private IEnumerator KickBallCoroutine() {
        cubeCount += selectedStartCube;

        if (cubeList[cubeCount % numberOfCubes].smashed) {
            if (smashedCount == numberOfCubes - 1) EndGame();
            else RestartRound();

            yield break;
        }

        chainCount++;
        smashedCount++;

        ball.GetComponent<Rigidbody>().AddForce(player.transform.forward * 30f, ForceMode.Impulse);

        yield return Yielders.Get(1f);

        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        ball.transform.position = player.transform.position + (player.transform.forward * 1.6f);

        if (smashedCount < numberOfCubes) RotatePlayer((cubeCount + selectedStartCube) % numberOfCubes);
        else EndGame();
    }

    private void RestartRound() {
        StartCoroutine(RestartRoundCoroutine());
    }

    private IEnumerator RestartRoundCoroutine() {
        AddScore(false);

        roundCount++;
        cubeList[cubeCount % numberOfCubes].countText.text = (cubeCount - startCube).ToString();
        chainText.text = chainCount.ToString() + "-CHAIN!";

        yield return Yielders.Get(1f);

        StartGame();
    }

    private void EndGame() {
        StartCoroutine(EndGameCoroutine());
    }

    private IEnumerator EndGameCoroutine() {
        AddScore(true);
        chainText.text = chainCount.ToString() + "-CHAIN!";

        yield return Yielders.Get(1f);

        chainText.text = "CLEAR!";

        yield return Yielders.Get(1f);

        RestartGame();
    }

    private void RestartGame() {
        cubeList.Clear();

        for (int i = 0; i < cubeObjects.Count; i++) {
            Destroy(cubeObjects[i]);
        }

        cubeObjects.Clear();

        InitialSetup();

        StartGame();
    }

    public void AddAccumulatedScore() {
        accumulatedScore++;
    }

    private void AddScore(bool end) {
        if (end) {
            accumulatedScore += chainCount;
            if (roundCount == 0) accumulatedScore *= 2;
            GameManager.instance.score += accumulatedScore;
        } else {
            accumulatedScore += chainCount;
            GameManager.instance.score += accumulatedScore;
        }
    }
}
