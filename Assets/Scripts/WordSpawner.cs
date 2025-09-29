using UnityEngine;
using System.Collections.Generic;

public class WordSpawner : MonoBehaviour
{
    [Header("Word Settings")]
    public GameObject wordPrefab;
    public Transform[] spawnPoints;         
    public string[] wordList;
    public float spawnInterval = 3f;

    [Header("Spawner Movement")]
    public float moveRangeX = 3f;       // max X distance from start
    public float moveRangeY = 1f;       // small Y variation
    public float moveSpeed = 2f;        // speed of movement

    [Header("Pause Settings")]
    [Range(0f, 1f)]
    public float pauseChance = 0.3f;    // 30% chance to pause at each target
    public float minPauseDuration = 0.5f;
    public float maxPauseDuration = 1.5f;

    private Vector3 startPos;
    private Vector3 targetPos;
    private bool isMoving = false;

    private List<string> shuffledWords = new List<string>();
    private int currentIndex = 0;

    void Start()
    {
        startPos = transform.position;
        targetPos = startPos;

        // Set difficulty word list
        switch(GameSettings.selectedDifficulty)
        {
            case 0: spawnInterval = 6f; wordList = WordLists.easyWords; break;
            case 1: spawnInterval = 6f; wordList = WordLists.mediumWords; break;
            case 2: spawnInterval = 6f; wordList = WordLists.hardWords; break;
        }

        ShuffleWords();
        InvokeRepeating("SpawnWord", 1f, spawnInterval);

        PickNewTarget(); // start first movement
    }

    void Update()
    {
        if (!isMoving) return;

        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            isMoving = false;
            StartCoroutine(PauseAndPickNext());
        }
    }

    System.Collections.IEnumerator PauseAndPickNext()
    {
        // Only pause based on chance
        if (Random.value < pauseChance)
        {
            float pauseTime = Random.Range(minPauseDuration, maxPauseDuration);
            yield return new WaitForSeconds(pauseTime);
        }

        PickNewTarget();
    }

    void PickNewTarget()
    {
        float randomX = startPos.x + Random.Range(-moveRangeX, moveRangeX);
        float randomY = startPos.y + Random.Range(-moveRangeY, moveRangeY);
        targetPos = new Vector3(randomX, randomY, startPos.z);
        isMoving = true;
    }

    void ShuffleWords()
    {
        shuffledWords = new List<string>(wordList);
        for (int i = shuffledWords.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            string temp = shuffledWords[i];
            shuffledWords[i] = shuffledWords[j];
            shuffledWords[j] = temp;
        }
        currentIndex = 0;
    }

    void SpawnWord()
    {
        if (currentIndex >= shuffledWords.Count) ShuffleWords();

        string chosenWord = shuffledWords[currentIndex++];
        int pos = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPosition = spawnPoints[pos].position;

        GameObject word = Instantiate(wordPrefab, spawnPosition, Quaternion.identity, transform.parent);
        word.GetComponent<WordObject>().targetWord = chosenWord;
    }
}
