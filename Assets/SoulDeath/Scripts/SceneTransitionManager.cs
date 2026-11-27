using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneTransitionManager
{
    public const string MainMenu = "00_MainMenu";
    public const string CityIntro = "01_CityIntro";
    public const string PoliceChase = "02_PoliceChase";
    public const string ForestPortal = "03_ForestPortal";
    public const string MysteryArena = "04_MysteryArena";
    public const string Ending = "05_Ending";

    public static void LoadScene(string sceneName)
    {
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"Scene '{sceneName}' is not included in the active Build Settings.");
            return;
        }

        if (GameManager.Instance != null)
            GameManager.Instance.SetPaused(false);

        SceneManager.LoadScene(sceneName);
    }

    public static string GetObjective(string sceneName)
    {
        switch (sceneName)
        {
            case CityIntro:
                return "Follow the road and reach the police barricade.";
            case PoliceChase:
                return "Outrun the cruiser. Keep moving down the road.";
            case ForestPortal:
                return "Search the crash site and enter the forest portal.";
            case MysteryArena:
                return "Defeat Card Guard Alpha and Card Guard Beta.";
            case Ending:
                return "The truth is only beginning to surface.";
            default:
                return string.Empty;
        }
    }
}
