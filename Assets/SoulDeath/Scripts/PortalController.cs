using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PortalController : MonoBehaviour
{
    [SerializeField] private string destinationScene;
    [SerializeField] private int requiredBossDefeats;

    private bool playerNearby;
    private float pulseOffset;

    public static string NearbyPrompt { get; private set; }

    public void Configure(string scene, int bossDefeats = 0)
    {
        destinationScene = scene;
        requiredBossDefeats = bossDefeats;
    }

    public static void ClearPrompt()
    {
        NearbyPrompt = string.Empty;
    }

    private void Update()
    {
        pulseOffset += Time.deltaTime;
        transform.localScale = Vector3.one * (1f + Mathf.Sin(pulseOffset * 2.3f) * 0.035f);

        if (!playerNearby || GameManager.IsPaused || Keyboard.current == null)
            return;

        bool unlocked = GameManager.Instance == null || GameManager.Instance.BossesDefeated >= requiredBossDefeats;
        NearbyPrompt = unlocked ? "Press E to enter the portal" : "The portal is sealed. Defeat both Card Guards.";

        if (unlocked && Keyboard.current.eKey.wasPressedThisFrame)
            SceneTransitionManager.LoadScene(destinationScene);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerController>() == null)
            return;

        playerNearby = true;
        NearbyPrompt = string.Empty;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<PlayerController>() == null)
            return;

        playerNearby = false;
        NearbyPrompt = string.Empty;
    }
}
