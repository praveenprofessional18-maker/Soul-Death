using UnityEngine;

public sealed class EndingController : MonoBehaviour
{
    private GUIStyle title;
    private GUIStyle message;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnGUI()
    {
        if (title == null)
        {
            title = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 52,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.84f, 0.12f, 0.19f) }
            };
            message = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true,
                fontSize = 19,
                normal = { textColor = new Color(0.86f, 0.88f, 0.92f) }
            };
        }

        GUI.color = new Color(0.01f, 0.015f, 0.025f, 0.72f);
        GUI.DrawTexture(new Rect(0f, 0f, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;
        GUI.Label(new Rect(0f, Screen.height * 0.28f, Screen.width, 72f), "SOUL DEATH", title);
        GUI.Label(new Rect(Screen.width * 0.2f, Screen.height * 0.43f, Screen.width * 0.6f, 100f),
            "The guards fall silent. Beneath the arena, a second heartbeat answers yours.\n\nTO BE CONTINUED", message);

        Rect button = new Rect(Screen.width * 0.5f - 120f, Screen.height * 0.68f, 240f, 46f);
        if (GUI.Button(button, "RETURN TO MAIN MENU"))
            SceneTransitionManager.LoadScene(SceneTransitionManager.MainMenu);
    }
}
