using UnityEngine;

public class HoverButton : MonoBehaviour
{
    public void OnHoverEnterEffect(GameObject go)
    {
        go.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
    }

    public void OnHoverExitEffect(GameObject go)
    {
        go.transform.localScale = Vector3.one;
    }
}
