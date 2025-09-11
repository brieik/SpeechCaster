using UnityEngine;

public class WordSpawner : MonoBehaviour
{
    public GameObject wordPrefab;
    public Transform[] spawnPoints;         // Relative to the spawner
    public string[] wordList;
    public float spawnInterval = 3f;

    [Header("Spawner Movement")]
    public float moveDistance = 3f;
    public float moveSpeed = 2f;

    private Vector3 startPos;
    private int lastWordIndex = -1; // store last word index

    void Start()
    {
        startPos = transform.position;
        InvokeRepeating("SpawnWord", 1f, spawnInterval);
    }

    void Update()
    {
        // Move spawner left and right
        float offset = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        transform.position = new Vector3(startPos.x + offset, startPos.y, startPos.z);
    }

    void SpawnWord()
    {
        // Pick random word but not same as last one
        int index;
        do
        {
            index = Random.Range(0, wordList.Length);
        } while (index == lastWordIndex && wordList.Length > 1);

        lastWordIndex = index;

        // Pick random spawn position
        int pos = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPosition = spawnPoints[pos].position;

        // Instantiate word at that world position (not as a child of spawner)
        GameObject word = Instantiate(wordPrefab, spawnPosition, Quaternion.identity, transform.parent);
        word.GetComponent<WordObject>().targetWord = wordList[index];
    }
}
