using UnityEngine;

public class WitchMover : MonoBehaviour
{
    public float moveDistance = 3f;     // How far left and right to move
    public float moveSpeed = 2f;        // How fast to move

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Move left and right using sine wave
        float offset = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        transform.position = new Vector3(startPos.x + offset, startPos.y, startPos.z);
    }
}
