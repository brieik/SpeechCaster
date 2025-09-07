using UnityEngine;
using TMPro;

public class WordObject : MonoBehaviour
{
    public TextMeshProUGUI wordText;  // Assign the TMP text component here
    public string targetWord;         // The word to display
    public float speed = 100f;        // Speed of falling — visible in Inspector

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        // Auto-assign TMP if not set in Inspector
        if (wordText == null)
            wordText = GetComponentInChildren<TextMeshProUGUI>();

        wordText.text = targetWord;
    }

    void Update()
    {
        // Move downward
        rectTransform.anchoredPosition += Vector2.down * speed * Time.deltaTime;

        // Get canvas height
        float canvasBottom = -((rectTransform.parent as RectTransform).rect.height / 2f);

        // Get bottom of this word
        float wordBottom = rectTransform.anchoredPosition.y - (rectTransform.rect.height / 2f);

        if (wordBottom <= canvasBottom)
        {
            GameManager.Instance.MissWord();
            Destroy(gameObject);
        }
    }



  public void Explode()
{
    GameObject explosion = Instantiate(GameManager.Instance.explosionPrefab, transform.position, Quaternion.identity, transform.parent);
    Destroy(explosion, 1f); // Destroy explosion after 1 second
    Destroy(gameObject);    // Destroy the word itself
}

    
   
}
