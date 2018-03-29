using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float smoothSpeed;

    private Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        if (!target)
            return;

        Vector3 targetPos = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, smoothSpeed);
        transform.LookAt(target);
    }
}