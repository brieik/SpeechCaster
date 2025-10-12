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
    public float moveRangeX = 3f;
    public float moveRangeY = 1f;
    public float moveSpeed = 2f;

    [Header("Pause Settings")]
    [Range(0f, 1f)]
    public float pauseChance = 0.3f;
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

        // Choose word list & spawn interval based on difficulty
        switch (GameSettings.selectedDifficulty)
        {
            case 0: spawnInterval = 6f; wordList = WordLists.easyWords; break;
            case 1: spawnInterval = 6f; wordList = WordLists.mediumWords; break;
            case 2: spawnInterval = 6f; wordList = WordLists.hardWords; break;
        }

        ShuffleWords();
        InvokeRepeating(nameof(SpawnWord), 1f, spawnInterval);
        PickNewTarget();
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
            (shuffledWords[i], shuffledWords[j]) = (shuffledWords[j], shuffledWords[i]);
        }
        currentIndex = 0;
    }

    void SpawnWord()
    {
        // ✅ Stop spawning if game over or max words reached
       if (GameManager.isGameOver || GameManager.Instance.WordsSpawned >= GameManager.Instance.maxWords)

        {
            CancelInvoke(nameof(SpawnWord));
            return;
        }

        if (currentIndex >= shuffledWords.Count)
            ShuffleWords();

        string chosenWord = shuffledWords[currentIndex++];
        int pos = Random.Range(0, spawnPoints.Length);
        Vector3 spawnPosition = spawnPoints[pos].position;

        GameObject word = Instantiate(wordPrefab, spawnPosition, Quaternion.identity, transform.parent);
        WordObject wordObj = word.GetComponent<WordObject>();
        wordObj.targetWord = chosenWord;

        // ✅ Notify GameManager that a word has spawned
        GameManager.Instance.OnWordSpawned();
    }
}
