using UnityEngine;

public sealed class HUDController : MonoBehaviour
{
    private PlayerHealth playerHealth;
    private BossAI[] bosses = new BossAI[0];
    private float nextBossRefresh;
    private GUIStyle titleStyle;
    private GUIStyle bodyStyle;
    private GUIStyle smallStyle;

    private void OnGUI()
    {
        EnsureStyles();

        if (playerHealth == null)
            playerHealth = FindAnyObjectByType<PlayerHealth>();
        if (Time.unscaledTime >= nextBossRefresh)
        {
            bosses = FindObjectsByType<BossAI>(FindObjectsSortMode.None);
            nextBossRefresh = Time.unscaledTime + 0.5f;
        }

        if (playerHealth != null)
        {
            GUI.Box(new Rect(22f, 20f, 300f, 90f), GUIContent.none);
            GUI.Label(new Rect(38f, 30f, 260f, 25f), "SOUL / VITALITY", titleStyle);
            Rect bar = new Rect(38f, 62f, 260f, 16f);
            GUI.color = new Color(0.12f, 0.12f, 0.15f, 0.95f);
            GUI.DrawTexture(bar, Texture2D.whiteTexture);
            float ratio = playerHealth.MaximumHealth <= 0f ? 0f : playerHealth.CurrentHealth / playerHealth.MaximumHealth;
            GUI.color = Color.Lerp(new Color(0.75f, 0.08f, 0.1f), new Color(0.3f, 0.82f, 0.57f), ratio);
            GUI.DrawTexture(new Rect(bar.x, bar.y, bar.width * ratio, bar.height), Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.Label(new Rect(38f, 80f, 260f, 20f), $"{Mathf.CeilToInt(playerHealth.CurrentHealth)} / {Mathf.CeilToInt(playerHealth.MaximumHealth)}", smallStyle);
        }

        string objective = GameManager.Instance != null ? GameManager.Instance.CurrentObjective : string.Empty;
        if (!string.IsNullOrEmpty(objective))
        {
            GUI.Box(new Rect(22f, 125f, 420f, 66f), GUIContent.none);
            GUI.Label(new Rect(38f, 135f, 380f, 24f), "CURRENT OBJECTIVE", titleStyle);
            GUI.Label(new Rect(38f, 159f, 385f, 26f), objective, bodyStyle);
        }

        if (!string.IsNullOrEmpty(PortalController.NearbyPrompt))
        {
            GUI.Box(new Rect(Screen.width * 0.5f - 230f, Screen.height - 110f, 460f, 48f), GUIContent.none);
            GUI.Label(new Rect(Screen.width * 0.5f - 215f, Screen.height - 102f, 430f, 32f), PortalController.NearbyPrompt, bodyStyle);
        }

        float bossY = 22f;
        foreach (BossAI boss in bosses)
        {
            if (boss == null || boss.IsDefeated)
                continue;

            GUI.Box(new Rect(Screen.width - 320f, bossY, 295f, 56f), GUIContent.none);
            GUI.Label(new Rect(Screen.width - 305f, bossY + 5f, 265f, 20f), boss.displayName, titleStyle);
            Rect bar = new Rect(Screen.width - 305f, bossY + 31f, 265f, 12f);
            GUI.color = new Color(0.12f, 0.12f, 0.15f, 0.95f);
            GUI.DrawTexture(bar, Texture2D.whiteTexture);
            float ratio = boss.MaximumHealth <= 0f ? 0f : boss.CurrentHealth / boss.MaximumHealth;
            GUI.color = boss.IsDefensive ? new Color(0.45f, 0.32f, 0.95f) : new Color(0.9f, 0.12f, 0.16f);
            GUI.DrawTexture(new Rect(bar.x, bar.y, bar.width * ratio, bar.height), Texture2D.whiteTexture);
            GUI.color = Color.white;
            bossY += 64f;
        }

        if (playerHealth != null && !GameManager.IsPaused && !GameManager.IsGameOver)
        {
            GUI.color = new Color(1f, 1f, 1f, 0.9f);
            GUI.DrawTexture(new Rect(Screen.width * 0.5f - 2f, Screen.height * 0.5f - 2f, 4f, 4f), Texture2D.whiteTexture);
            GUI.color = Color.white;
            GUI.Label(new Rect(Screen.width - 230f, Screen.height - 36f, 210f, 24f), "WASD move  |  E interact", smallStyle);
        }
    }

    private void EnsureStyles()
    {
        if (titleStyle != null)
            return;

        titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 13,
            fontStyle = FontStyle.Bold,
            normal = { textColor = new Color(0.82f, 0.88f, 0.96f) }
        };
        bodyStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 15,
            wordWrap = true,
            normal = { textColor = Color.white }
        };
        smallStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 11,
            normal = { textColor = new Color(0.68f, 0.73f, 0.8f) }
        };
    }
}
