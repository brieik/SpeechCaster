using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ShrinkBounceButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Vector3 originalScale;
    public float pressedScale = 0.85f;
    public float animationSpeed = 15f;
    public float bounceAmount = 1.1f;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(BounceScale(originalScale * pressedScale));
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(BounceBack());
    }

    private IEnumerator BounceScale(Vector3 targetScale)
    {
        while (Vector3.Distance(transform.localScale, targetScale) > 0.001f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
            yield return null;
        }
    }

    private IEnumerator BounceBack()
    {
        // Slight overshoot (bounce)
        Vector3 overshootScale = originalScale * bounceAmount;

        // Go past the original size
        while (Vector3.Distance(transform.localScale, overshootScale) > 0.001f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, overshootScale, Time.deltaTime * animationSpeed);
            yield return null;
        }

        // Return to exact normal scale
        while (Vector3.Distance(transform.localScale, originalScale) > 0.001f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * animationSpeed);
            yield return null;
        }
    }
}
