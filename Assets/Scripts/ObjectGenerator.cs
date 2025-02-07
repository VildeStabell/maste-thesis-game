using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour {
    public GameObject[] objectPrefabs;

    private RoundController roundCtrl;
    private GameObject board;

    // Spawnable area coordinates
    private float spawnMin;
    private float spawnMax;

    // Tweakable from inspector
    public float spawnHeight = 5;
    public float startTime = 10.0f;
    public float startFrequency = 20.0f;
    public float frequencyIncrease = 0.99f;
    public float spawnAfterSoundDelay = 0.3f;

    // Start is called before the first frame update
    void Start() {
        roundCtrl = gameObject.GetComponent<RoundController>();

        StartCoroutine(SpawnAfterSeconds());
    }

    /**
        Spawns a new object in a random location
    */
    private IEnumerator SpawnNewObject() {
        GameObject prefab = objectPrefabs[Random.Range(0, objectPrefabs.Length)];
        float xCoord = Random.Range(spawnMin, spawnMax);
        float zCoord = Random.Range(spawnMin, spawnMax);
        Vector3 spawnPos = new Vector3(xCoord, spawnHeight, zCoord);

        if (board) {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.objectSpawned, spawnPos);
            yield return new WaitForSeconds(spawnAfterSoundDelay);
            Instantiate(prefab, spawnPos, Quaternion.identity, board.transform);
        }
    }

    /**
        Starts the recursive spawning of objects
    */
    IEnumerator SpawnAfterSeconds() {
        yield return new WaitForSeconds(startTime);

        board = roundCtrl.getBoard();

        // Calculates the spawnable area
        Vector3 boxColliderSize = board.GetComponent<BoxCollider>().size;
        float sideLength = Mathf.Sqrt(Mathf.Pow(boxColliderSize.x, 2) + Mathf.Pow(boxColliderSize.z, 2)) / 2;
        spawnMin = -sideLength / 2;
        spawnMax = sideLength / 2;

        StartCoroutine(SpawnNewObject());

        StartCoroutine(SpawnAfterSeconds(startFrequency));
    }

    /**
        Spawns an object after waiting for the specified amount of seconds
    */
    IEnumerator SpawnAfterSeconds(float seconds) {
        yield return new WaitForSeconds(seconds);

        if (board) {
            StartCoroutine(SpawnNewObject());
            StartCoroutine(SpawnAfterSeconds(seconds * frequencyIncrease));
        }
    }
}
