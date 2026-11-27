using UnityEngine;

public sealed class PoliceChaseController : MonoBehaviour
{
    [SerializeField] private float followingDistance = 8f;
    [SerializeField] private float chaseSpeed = 6.5f;
    [SerializeField] private Renderer[] emergencyLights;

    private Transform player;
    private float flashClock;

    public void Configure(Renderer[] lights)
    {
        emergencyLights = lights;
    }

    private void Update()
    {
        if (GameManager.IsPaused || GameManager.IsGameOver)
            return;

        if (player == null)
        {
            PlayerController controller = FindAnyObjectByType<PlayerController>();
            if (controller != null)
                player = controller.transform;
        }

        if (player != null)
        {
            Vector3 desiredPosition = player.position - player.forward * followingDistance;
            desiredPosition.y = transform.position.y;
            transform.position = Vector3.MoveTowards(transform.position, desiredPosition, chaseSpeed * Time.deltaTime);
            Vector3 direction = player.position - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 4f * Time.deltaTime);
        }

        flashClock += Time.deltaTime;
        if (emergencyLights == null)
            return;

        bool redPhase = Mathf.Sin(flashClock * 11f) > 0f;
        for (int i = 0; i < emergencyLights.Length; i++)
        {
            Renderer lightRenderer = emergencyLights[i];
            if (lightRenderer != null)
                lightRenderer.enabled = (i % 2 == 0) == redPhase;
        }
    }
}
