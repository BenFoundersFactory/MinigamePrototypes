using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkUpManager : MonoBehaviour {

    private const float FIELD_WIDTH = 5f;
    private const float FIELD_HEIGHT = 11f;
    private const float FIELD_OFFSET = 1f;
    private const float FIELD_START_X = -5f;
    private const float FIELD_START_Z = -5f;
    private const float FIELD_SIZE_X = 10f;
    private const float FIELD_SIZE_Z = 10f;
    private const int NUMBER_OF_PLAYERS = 10;

    [SerializeField] private GameObject pitchPoint;
    [SerializeField] private Transform opponentPitch;
    [SerializeField] private Transform friendlyPitch;

    private List<PitchPoint> opponentPitchPoint = new List<PitchPoint>();
    private List<PitchPoint> friendlyPitchPoint = new List<PitchPoint>();

    [SerializeField] private Transform friendlyHolder;

    private List<GameObject> friendlyObject = new List<GameObject>();
    private List<GameObject> opponentObject = new List<GameObject>();

    [SerializeField] private GameObject friendlyPrefab;

	void Start () {
        PitchSetup();

        InitialSetup();
	}
	
	void Update () {
		
	}

    private void PitchSetup() {
        for (int j = 0; j < FIELD_HEIGHT; j++) {
            for (int i = 0; i < FIELD_WIDTH; i++) {
                Vector3 point = new Vector3((FIELD_START_X + FIELD_OFFSET) + ((FIELD_SIZE_X / FIELD_WIDTH) * i), 
                                            0f, 
                                            (FIELD_START_Z + FIELD_OFFSET/2) + ((FIELD_SIZE_Z / FIELD_HEIGHT) * j));

                GameObject go = (GameObject)Instantiate(pitchPoint, transform.position, Quaternion.identity);
                go.transform.SetParent(opponentPitch, true);

                PitchPoint pp = go.GetComponent<PitchPoint>();
                pp.SetupPoint(i + (j * (int) FIELD_WIDTH));

                go.transform.localPosition = point;
                go.transform.name = pp.id.ToString();

                opponentPitchPoint.Add(pp);

                point = new Vector3((-FIELD_START_X - FIELD_OFFSET) - ((FIELD_SIZE_X / FIELD_WIDTH) * i),
                                     0f,
                                    (FIELD_START_Z + FIELD_OFFSET / 2) + ((FIELD_SIZE_Z / FIELD_HEIGHT) * j));

                go = (GameObject)Instantiate(pitchPoint, transform.position, Quaternion.identity);
                go.transform.SetParent(friendlyPitch, true);

                pp = go.GetComponent<PitchPoint>();
                pp.SetupPoint(i + (j * (int)FIELD_WIDTH));

                go.transform.localPosition = point;
                go.transform.name = pp.id.ToString();

                friendlyPitchPoint.Add(pp);
            }
        }
    }

    private void InitialSetup() {
        for (int i = 0; i < NUMBER_OF_PLAYERS; i++) {
            GameObject go = (GameObject)Instantiate(friendlyPrefab, transform.position, Quaternion.identity);
        
            go.transform.SetParent(friendlyHolder, true);
            go.transform.position = friendlyHolder.position;

            go.transform.localPosition += new Vector3(0f, 0f, 2f * i);
        }
    }
}
