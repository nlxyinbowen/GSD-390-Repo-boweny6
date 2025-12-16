using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Vector3 localMoveDirection = Vector3.right;
    [SerializeField] private float distance = 3f; 
    [SerializeField] private float speed = 2f; 
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        float t = Mathf.Sin(Time.time * speed);
        Vector3 offset = localMoveDirection.normalized * (t * distance);
        transform.position = startPos + offset;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 dir = localMoveDirection.normalized;
        Gizmos.DrawLine(transform.position - dir * distance, transform.position + dir * distance);
    }
}
