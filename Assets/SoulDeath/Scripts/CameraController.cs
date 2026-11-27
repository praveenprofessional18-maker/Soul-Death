using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public sealed class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float distance = 5.2f;
    [SerializeField] private float sensitivity = 0.12f;
    [SerializeField] private float minimumPitch = -18f;
    [SerializeField] private float maximumPitch = 65f;
    [SerializeField] private float followSharpness = 16f;

    private float yaw;
    private float pitch = 14f;

    public void SetTarget(Transform followTarget)
    {
        target = followTarget;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        if (!GameManager.IsPaused && Mouse.current != null)
        {
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            yaw += mouseDelta.x * sensitivity;
            pitch = Mathf.Clamp(pitch - mouseDelta.y * sensitivity, minimumPitch, maximumPitch);
        }

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 focusPoint = target.position;
        Vector3 desiredPosition = focusPoint - rotation * Vector3.forward * distance;

        if (Physics.Linecast(focusPoint, desiredPosition, out RaycastHit hit, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            desiredPosition = hit.point + hit.normal * 0.2f;

        float blend = 1f - Mathf.Exp(-followSharpness * Time.unscaledDeltaTime);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, blend);
        transform.rotation = Quaternion.LookRotation(focusPoint - transform.position, Vector3.up);
    }
}
