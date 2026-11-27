using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PauseMenuController : MonoBehaviour
{
    private GUIStyle heading;
    private GUIStyle detail;

    private void Update()
    {
        if (Keyboard.current == null || GameManager.IsGameOver)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame && GameManager.Instance != null)
            GameManager.Instance.SetPaused(!GameManager.IsPaused);
    }

    private void OnGUI()
    {
        if (!GameManager.IsPaused && !GameManager.IsGameOver)
            return;

        EnsureStyles();
        float width = 400f;
        float height = GameManager.IsGameOver ? 280f : 235f;
        Rect panel = new Rect((Screen.width - width) * 0.5f, (Screen.height - height) * 0.5f, width, height);
        GUI.Box(panel, GUIContent.none);

        GUILayout.BeginArea(new Rect(panel.x + 30f, panel.y + 24f, panel.width - 60f, panel.height - 48f));
        GUILayout.Label(GameManager.IsGameOver ? "SOUL LOST" : "PAUSED", heading);
        GUILayout.Space(10f);
        GUILayout.Label(GameManager.IsGameOver ? "The city has claimed another soul." : "Take a breath. The night is still waiting.", detail);
        GUILayout.Space(18f);

        if (GameManager.IsGameOver)
        {
            if (GUILayout.Button("TRY AGAIN", GUILayout.Height(40f)))
                SceneTransitionManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }
        else if (GUILayout.Button("RESUME", GUILayout.Height(40f)))
        {
            GameManager.Instance.SetPaused(false);
        }

        if (GUILayout.Button("RESTART SCENE", GUILayout.Height(36f)))
            SceneTransitionManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        if (GUILayout.Button("MAIN MENU", GUILayout.Height(36f)))
            SceneTransitionManager.LoadScene(SceneTransitionManager.MainMenu);
        GUILayout.EndArea();
    }

    private void EnsureStyles()
    {
        if (heading != null)
            return;

        heading = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            fontSize = 30,
            fontStyle = FontStyle.Bold,
            normal = { textColor = new Color(0.88f, 0.16f, 0.2f) }
        };
        detail = new GUIStyle(GUI.skin.label)
        {
            alignment = TextAnchor.MiddleCenter,
            wordWrap = true,
            fontSize = 15,
            normal = { textColor = Color.white }
        };
    }
}
