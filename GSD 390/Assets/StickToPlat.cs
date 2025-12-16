using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerStickToMovingPlatformCC : MonoBehaviour
{
    private Transform currentPlatform;

    void LateUpdate()
    {
        if (currentPlatform == null && transform.parent != null)
            transform.SetParent(null);

        currentPlatform = null;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.normal.y < 0.5f) return;

        if (hit.collider.CompareTag("MovingPlatform"))
        {
            currentPlatform = hit.collider.transform;

            if (transform.parent != currentPlatform)
                transform.SetParent(currentPlatform);
        }
        else
        {
            if (transform.parent != null)
                transform.SetParent(null);
        }
    }
}
