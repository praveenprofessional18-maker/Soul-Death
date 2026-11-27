using UnityEngine;

public sealed class MainMenuController : MonoBehaviour
{
    private bool showSettings;
    private bool showCredits;
    private float musicVolume = 0.32f;
    private GUIStyle title;
    private GUIStyle subtitle;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnGUI()
    {
        EnsureStyles();
        GUI.color = new Color(0.015f, 0.02f, 0.04f, 0.64f);
        GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;

        float panelWidth = Mathf.Min(460f, Screen.width - 50f);
        Rect panel = new Rect(52f, Screen.height * 0.16f, panelWidth, Screen.height * 0.68f);
        GUILayout.BeginArea(panel);
        GUILayout.Label("SOUL", title);
        GUILayout.Label("DEATH", title);
        GUILayout.Space(8f);
        GUILayout.Label("A MYSTERY IN THE DARK", subtitle);
        GUILayout.Space(34f);

        if (!showSettings && !showCredits)
        {
            if (GUILayout.Button("PLAY", GUILayout.Height(48f)))
                SceneTransitionManager.LoadScene(SceneTransitionManager.CityIntro);
            GUILayout.Space(8f);
            if (GUILayout.Button("SETTINGS", GUILayout.Height(42f)))
                showSettings = true;
            GUILayout.Space(8f);
            if (GUILayout.Button("CREDITS", GUILayout.Height(42f)))
                showCredits = true;
            GUILayout.Space(8f);
            if (GUILayout.Button("EXIT", GUILayout.Height(42f)))
                Application.Quit();
        }
        else if (showSettings)
        {
            GUILayout.Label("SETTINGS", subtitle);
            GUILayout.Space(12f);
            GUILayout.Label("MUSIC VOLUME", subtitle);
            musicVolume = GUILayout.HorizontalSlider(musicVolume, 0f, 1f, GUILayout.Height(24f));
            AudioManager[] audioManagers = FindObjectsByType<AudioManager>(FindObjectsSortMode.None);
            foreach (AudioManager audioManager in audioManagers)
            {
                AudioSource source = audioManager.GetComponent<AudioSource>();
                if (source != null)
                    source.volume = musicVolume;
            }
            GUILayout.Space(18f);
            GUILayout.Label("WASD Move   |   Mouse Look   |   Space Jump", subtitle);
            GUILayout.Label("Shift Sprint   |   Left Mouse Attack   |   E Interact", subtitle);
            GUILayout.Label("Escape Pause", subtitle);
            GUILayout.Space(20f);
            if (GUILayout.Button("BACK", GUILayout.Height(42f)))
                showSettings = false;
        }
        else
        {
            GUILayout.Label("A SMALL TEAM WITH A BIG MYSTERY", subtitle);
            GUILayout.Space(20f);
            GUILayout.Label("Built with Unity primitives and the SOUL DEATH project assets.", subtitle);
            GUILayout.Space(20f);
            if (GUILayout.Button("BACK", GUILayout.Height(42f)))
                showCredits = false;
        }

        GUILayout.EndArea();
        GUI.color = new Color(0.72f, 0.78f, 0.88f, 0.85f);
        GUI.Label(new Rect(52f, Screen.height - 38f, 520f, 24f), "A single-player mystery action adventure");
        GUI.color = Color.white;
    }

    private void EnsureStyles()
    {
        if (title != null)
            return;

        title = new GUIStyle(GUI.skin.label)
        {
            fontSize = Mathf.Clamp(Screen.width / 26, 42, 72),
            fontStyle = FontStyle.Bold,
            normal = { textColor = new Color(0.9f, 0.12f, 0.18f) }
        };
        subtitle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 15,
            wordWrap = true,
            normal = { textColor = new Color(0.78f, 0.84f, 0.92f) }
        };
    }
}
