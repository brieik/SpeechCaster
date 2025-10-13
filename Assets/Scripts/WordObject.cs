using UnityEngine;
using TMPro;

public class WordObject : MonoBehaviour
{
    public TextMeshProUGUI wordText;
    public string targetWord;
    public float speed = 100f;

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (wordText == null)
            wordText = GetComponentInChildren<TextMeshProUGUI>();
        wordText.text = targetWord;
    }

    void Update()
    {
        // Move word downward
        rectTransform.anchoredPosition += Vector2.down * speed * Time.deltaTime;

        // Check if word reached bottom of canvas
        float canvasBottom = -((rectTransform.parent as RectTransform).rect.height / 2f);
        float wordBottom = rectTransform.anchoredPosition.y - (rectTransform.rect.height / 2f);

        if (wordBottom <= canvasBottom)
        {
            // Word has fallen → deduct heart and notify GameManager
            GameManager.Instance.WordFallen();
            Destroy(gameObject);
        }
    }

    public void Explode(float confidence = 1f)
    {
        // Spawn explosion effect
        if (GameManager.Instance.explosionPrefab != null)
        {
            GameObject explosion = Instantiate(GameManager.Instance.explosionPrefab, transform.position, Quaternion.identity, transform.parent);
            Destroy(explosion, 1f);
        }

        // Notify GameManager
        GameManager.Instance.OnWordExploded(confidence);

        // Destroy the word
        Destroy(gameObject);
    }
}