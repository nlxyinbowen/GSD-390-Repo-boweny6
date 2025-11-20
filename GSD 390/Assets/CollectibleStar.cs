using UnityEngine;

public class CollectibleStar : MonoBehaviour
{
    [SerializeField, Range(0f, 360f)]
    private float rotationSpeed = 90f;

    [SerializeField] private float bobHeight = 0.25f;
    [SerializeField] private float bobSpeed = 2f;

    private float baseY;
    private bool collected = false;

    private void Start()
    {
        baseY = transform.position.y;
    }

    private void Update()
    {
        if (collected) return;

        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        Vector3 pos = transform.position;
        pos.y = baseY + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = pos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (collected) return;
        if (!other.CompareTag("Player")) return;

        collected = true;
        gameObject.SetActive(false);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStarCollected();
        }
    }
}
