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
        int index = Random.Range(0, wordList.Length);
        int pos = Random.Range(0, spawnPoints.Length);

        // Calculate world position of the spawn point
        Vector3 spawnPosition = spawnPoints[pos].position;

        // Instantiate word at that world position (not as a child of spawner)
        GameObject word = Instantiate(wordPrefab, spawnPosition, Quaternion.identity, transform.parent);
        word.GetComponent<WordObject>().targetWord = wordList[index];
    }
}
