using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class PrototypeSceneBuilder
{
    private const string Root = "Assets/SoulDeath";
    private const string SceneFolder = Root + "/Scenes";
    private const string MaterialFolder = Root + "/Materials";
    private const string RobotPrefabPath = "Assets/Prefabs/PlayerRobot.prefab";
    private const string SuspenseMusicPath = "Assets/SourceFiles/SoundFX/Music_Suspenseful.ogg";
    private const string EtherealMusicPath = "Assets/SourceFiles/SoundFX/Music_Ethereal.ogg";
    private const string SceneGenerationPreference = "SOUL_DEATH_PlayableScenesGenerated";

    private static readonly string[] SceneNames =
    {
        SceneTransitionManager.MainMenu,
        SceneTransitionManager.CityIntro,
        SceneTransitionManager.PoliceChase,
        SceneTransitionManager.ForestPortal,
        SceneTransitionManager.MysteryArena,
        SceneTransitionManager.Ending
    };

    private static Material asphalt;
    private static Material concrete;
    private static Material darkSteel;
    private static Material warmLight;
    private static Material treeBark;
    private static Material foliage;
    private static Material portalGlow;
    private static Material portalCore;
    private static Material bossRed;
    private static Material bossBlue;
    private static Material policeBlack;
    private static Material policeWhite;
    private static Material lanePaint;
    private static Material gold;

    [MenuItem("SOUL DEATH/Build Playable Prototype Scenes")]
    public static void BuildPlayableScenes()
    {
        BuildPlayableScenesCore(true);
    }

    [InitializeOnLoadMethod]
    private static void BuildInitialScenesWhenUnityIsReady()
    {
        EditorApplication.delayCall += () =>
        {
            if (EditorPrefs.GetBool(SceneGenerationPreference, false))
                return;

            string[] existing = SceneNames
                .Select(name => $"{SceneFolder}/{name}.unity")
                .Where(File.Exists)
                .ToArray();

            if (existing.Length == SceneNames.Length)
            {
                EditorPrefs.SetBool(SceneGenerationPreference, true);
                return;
            }

            if (existing.Length > 0)
            {
                Debug.LogError("SOUL DEATH scene generation stopped: only some target scene files exist. Existing files were preserved.");
                return;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;

            try
            {
                BuildPlayableScenesCore(false);
                EditorPrefs.SetBool(SceneGenerationPreference, true);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        };
    }

    private static void BuildPlayableScenesCore(bool showCompletionDialog)
    {
        EnsureFolders();

        string[] existing = SceneNames
            .Select(name => $"{SceneFolder}/{name}.unity")
            .Where(File.Exists)
            .ToArray();

        if (existing.Length > 0)
        {
            string message = "To protect existing work, scene generation stopped because one or more target scenes already exist:\n\n" +
                             string.Join("\n", existing);
            if (showCompletionDialog)
                EditorUtility.DisplayDialog("SOUL DEATH scenes already exist", message, "OK");
            else
                Debug.LogWarning(message);
            return;
        }

        CreateMaterials();
        BuildMainMenu();
        BuildCityIntro();
        BuildPoliceChase();
        BuildForest();
        BuildArena();
        BuildEnding();
        RegisterBuildScenes();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorSceneManager.OpenScene($"{SceneFolder}/{SceneTransitionManager.MainMenu}.unity", OpenSceneMode.Single);
        Debug.Log($"SOUL DEATH: created and registered {SceneNames.Length} playable prototype scenes.");
        if (showCompletionDialog)
        {
            EditorUtility.DisplayDialog(
                "SOUL DEATH prototype scenes created",
                "Six scenes were saved and added to Build Settings. Open 00_MainMenu and press Play to begin.",
                "OK");
        }
    }

    [MenuItem("SOUL DEATH/Build Windows Player")]
    public static void BuildWindowsPlayer()
    {
        string outputDirectory = @"C:\Users\Hi\Downloads\SOUL DEATH\Builds\Windows";
        string executablePath = Path.Combine(outputDirectory, "SOUL DEATH.exe");

        if (File.Exists(executablePath))
        {
            EditorUtility.DisplayDialog(
                "Existing build preserved",
                $"The build was not started because this file already exists:\n{executablePath}",
                "OK");
            return;
        }

        string[] scenes = EditorBuildSettings.scenes
            .Where(scene => scene.enabled)
            .Select(scene => scene.path)
            .ToArray();

        if (scenes.Length == 0)
        {
            EditorUtility.DisplayDialog("No scenes registered", "Build the prototype scenes before building the Windows player.", "OK");
            return;
        }

        Directory.CreateDirectory(outputDirectory);
        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = executablePath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result == BuildResult.Succeeded)
            Debug.Log($"SOUL DEATH Windows build succeeded: {executablePath}");
        else
            Debug.LogError($"SOUL DEATH Windows build failed: {report.summary.result}; errors={report.summary.totalErrors}. See Editor.log.");
    }

    private static void EnsureFolders()
    {
        Directory.CreateDirectory(Path.Combine(Application.dataPath, "SoulDeath", "Scenes"));
        Directory.CreateDirectory(Path.Combine(Application.dataPath, "SoulDeath", "Materials"));
        Directory.CreateDirectory(Path.Combine(Application.dataPath, "SoulDeath", "Scripts"));
        Directory.CreateDirectory(Path.Combine(Application.dataPath, "SoulDeath", "Editor"));
        AssetDatabase.Refresh();
    }

    private static void CreateMaterials()
    {
        asphalt = MakeMaterial("SD_Asphalt", new Color(0.055f, 0.065f, 0.08f));
        concrete = MakeMaterial("SD_Concrete", new Color(0.19f, 0.22f, 0.25f));
        darkSteel = MakeMaterial("SD_DarkSteel", new Color(0.075f, 0.09f, 0.12f));
        warmLight = MakeMaterial("SD_WarmLight", new Color(1f, 0.66f, 0.25f), true);
        treeBark = MakeMaterial("SD_TreeBark", new Color(0.16f, 0.11f, 0.085f));
        foliage = MakeMaterial("SD_Foliage", new Color(0.035f, 0.13f, 0.11f));
        portalGlow = MakeMaterial("SD_PortalGlow", new Color(0.22f, 0.72f, 1f), true);
        portalCore = MakeMaterial("SD_PortalCore", new Color(0.018f, 0.03f, 0.08f), true);
        bossRed = MakeMaterial("SD_CardGuardRed", new Color(0.68f, 0.045f, 0.07f), true);
        bossBlue = MakeMaterial("SD_CardGuardViolet", new Color(0.25f, 0.16f, 0.75f), true);
        policeBlack = MakeMaterial("SD_PoliceBlack", new Color(0.025f, 0.035f, 0.055f));
        policeWhite = MakeMaterial("SD_PoliceWhite", new Color(0.76f, 0.8f, 0.83f));
        lanePaint = MakeMaterial("SD_LanePaint", new Color(0.82f, 0.75f, 0.48f), true);
        gold = MakeMaterial("SD_Gold", new Color(0.85f, 0.52f, 0.13f), true);
        AssetDatabase.SaveAssets();
    }

    private static Material MakeMaterial(string name, Color color, bool emissive = false)
    {
        string path = $"{MaterialFolder}/{name}.mat";
        Material existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null)
            return existing;

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
            shader = Shader.Find("Standard");
        Material material = new Material(shader)
        {
            name = name,
            color = color
        };

        if (material.HasProperty("_BaseColor"))
            material.SetColor("_BaseColor", color);
        if (emissive)
        {
            material.EnableKeyword("_EMISSION");
            if (material.HasProperty("_EmissionColor"))
                material.SetColor("_EmissionColor", color * 1.65f);
        }

        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    private static Scene BeginScene(string sceneName, Color ambient, Color background, bool fog)
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = sceneName;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = ambient;
        RenderSettings.fog = fog;
        RenderSettings.fogColor = background;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = fog ? 0.014f : 0f;
        CreateCamera(background);
        CreateDirectionalLight();
        CreateRuntimeSystems(sceneName);
        return scene;
    }

    private static void SaveScene(Scene scene)
    {
        string path = $"{SceneFolder}/{scene.name}.unity";
        if (!EditorSceneManager.SaveScene(scene, path))
            throw new InvalidOperationException($"Failed to save scene at {path}.");
    }

    private static void BuildMainMenu()
    {
        Scene scene = BeginScene(SceneTransitionManager.MainMenu, new Color(0.12f, 0.13f, 0.18f), new Color(0.008f, 0.012f, 0.025f), false);
        GameObject set = Group("Menu Backdrop");
        Cube("Obsidian dais", set.transform, new Vector3(2f, -0.3f, 1f), new Vector3(10f, 0.6f, 8f), darkSteel);
        GameObject hero = CreateRobotVisual(set.transform, new Vector3(2f, 0f, 1.3f), Quaternion.Euler(0f, 25f, 0f));
        hero.name = "Timmy - menu silhouette";
        hero.transform.localScale = Vector3.one * 1.15f;
        Cube("Back wall", set.transform, new Vector3(2f, 4f, 6f), new Vector3(20f, 9f, 0.6f), asphalt);
        PointLight("Red rim light", set.transform, new Vector3(4f, 4f, 2f), new Color(0.95f, 0.055f, 0.1f), 150f, 9f);
        PointLight("Cold fill light", set.transform, new Vector3(0f, 3f, -1f), new Color(0.14f, 0.32f, 0.65f), 100f, 8f);

        GameObject menu = new GameObject("Main Menu");
        menu.AddComponent<MainMenuController>();
        SaveScene(scene);
    }

    private static void BuildCityIntro()
    {
        Scene scene = BeginScene(SceneTransitionManager.CityIntro, new Color(0.16f, 0.18f, 0.23f), new Color(0.025f, 0.045f, 0.075f), true);
        GameObject city = Group("City Blockout");
        BuildRoad(city.transform, -16f, 46f, 12f);
        for (int i = 0; i < 6; i++)
        {
            float z = -9f + i * 10f;
            Building(city.transform, $"West block {i + 1}", new Vector3(-10f, 0f, z), new Vector3(8f, 8f + (i % 3) * 2f, 8f), i % 2 == 0 ? concrete : darkSteel);
            Building(city.transform, $"East block {i + 1}", new Vector3(10f, 0f, z + 4f), new Vector3(8f, 10f + (i % 2) * 3f, 8f), i % 2 == 0 ? darkSteel : concrete);
            StreetLamp(city.transform, -5.3f, z + 2f);
            StreetLamp(city.transform, 5.3f, z + 7f);
        }

        PoliceCar(city.transform, new Vector3(3.2f, 0f, 23f), false);
        PointLight("Barricade flare", city.transform, new Vector3(0f, 3f, 31f), new Color(0.95f, 0.1f, 0.12f), 55f, 7f);
        AddObjectiveTrigger("City exit - police chase", new Vector3(0f, 1f, 39f), new Vector3(10f, 3f, 2f), SceneTransitionManager.PoliceChase);
        CreatePlayer(new Vector3(0f, 0f, -12f));
        SaveScene(scene);
    }

    private static void BuildPoliceChase()
    {
        Scene scene = BeginScene(SceneTransitionManager.PoliceChase, new Color(0.13f, 0.13f, 0.17f), new Color(0.045f, 0.025f, 0.035f), true);
        GameObject chase = Group("Police Pursuit");
        BuildRoad(chase.transform, -10f, 58f, 10f);
        for (int i = 0; i < 10; i++)
        {
            float z = -5f + i * 7f;
            Cube($"Left barrier {i}", chase.transform, new Vector3(-7f, 0.55f, z), new Vector3(0.45f, 1.1f, 2.6f), i % 2 == 0 ? policeWhite : bossRed);
            Cube($"Right barrier {i}", chase.transform, new Vector3(7f, 0.55f, z + 3f), new Vector3(0.45f, 1.1f, 2.6f), i % 2 == 0 ? policeBlack : policeWhite);
            if (i % 2 == 0)
                StreetLamp(chase.transform, 5.4f, z + 2f);
        }

        GameObject cruiser = PoliceCar(chase.transform, new Vector3(0f, 0f, -11f), true);
        PoliceChaseController chaseController = cruiser.AddComponent<PoliceChaseController>();
        Renderer[] lights = cruiser.GetComponentsInChildren<Renderer>()
            .Where(renderer => renderer.gameObject.name.Contains("Emergency"))
            .ToArray();
        chaseController.Configure(lights);
        PointLight("Pursuit spotlight", chase.transform, new Vector3(0f, 3f, -5f), new Color(0.85f, 0.1f, 0.13f), 45f, 8f);

        CreatePlayer(new Vector3(0f, 0f, -1f));
        AddObjectiveTrigger("Escape route to forest", new Vector3(0f, 1f, 52f), new Vector3(9f, 3f, 2f), SceneTransitionManager.ForestPortal);
        SaveScene(scene);
    }

    private static void BuildForest()
    {
        Scene scene = BeginScene(SceneTransitionManager.ForestPortal, new Color(0.08f, 0.12f, 0.13f), new Color(0.012f, 0.03f, 0.035f), true);
        GameObject forest = Group("Fog Forest");
        Cube("Forest floor", forest.transform, new Vector3(0f, -0.4f, 12f), new Vector3(46f, 0.8f, 90f), treeBark);
        Cube("Muddy path", forest.transform, new Vector3(0f, 0.015f, 12f), new Vector3(5.5f, 0.05f, 90f), asphalt);

        for (int i = 0; i < 24; i++)
        {
            float z = -24f + i * 3.2f;
            float spread = 8f + (i % 4) * 2.2f;
            Tree(forest.transform, new Vector3(-spread, 0f, z), 0.8f + (i % 3) * 0.15f);
            Tree(forest.transform, new Vector3(spread + (i % 2), 0f, z + 1.5f), 0.9f + (i % 4) * 0.12f);
        }

        Vehicle(forest.transform, "Crashed police cruiser", new Vector3(-4.2f, 0f, 12f), policeBlack, policeWhite, true);
        PointLight("Crash sparks", forest.transform, new Vector3(-4f, 1.7f, 12f), new Color(1f, 0.28f, 0.06f), 35f, 5f);
        PointLight("Portal radiance", forest.transform, new Vector3(0f, 2.1f, 34f), new Color(0.12f, 0.58f, 1f), 100f, 9f);

        CreatePlayer(new Vector3(0f, 0f, -10f));
        CreatePortal("Forest Portal", new Vector3(0f, 2.1f, 34f), SceneTransitionManager.MysteryArena, 0);
        SaveScene(scene);
    }

    private static void BuildArena()
    {
        Scene scene = BeginScene(SceneTransitionManager.MysteryArena, new Color(0.11f, 0.08f, 0.15f), new Color(0.02f, 0.012f, 0.045f), true);
        GameObject arena = Group("Mystery Arena");
        Cube("Arena floor", arena.transform, new Vector3(0f, -0.45f, 2f), new Vector3(40f, 0.9f, 46f), darkSteel);
        Cube("North wall", arena.transform, new Vector3(0f, 3.3f, 25f), new Vector3(40f, 7f, 1f), asphalt);
        Cube("West wall", arena.transform, new Vector3(-20f, 3.3f, 2f), new Vector3(1f, 7f, 46f), asphalt);
        Cube("East wall", arena.transform, new Vector3(20f, 3.3f, 2f), new Vector3(1f, 7f, 46f), asphalt);

        for (int i = 0; i < 8; i++)
        {
            float x = -15f + (i % 4) * 10f;
            float z = (i / 4) * 25f - 8f;
            Cube($"Rune pillar {i + 1}", arena.transform, new Vector3(x, 2f, z), new Vector3(1.3f, 4f, 1.3f), i % 2 == 0 ? bossBlue : bossRed);
        }

        PointLight("Arena red key", arena.transform, new Vector3(-7f, 7f, 5f), new Color(0.9f, 0.045f, 0.1f), 100f, 12f);
        PointLight("Arena violet key", arena.transform, new Vector3(7f, 7f, 5f), new Color(0.28f, 0.14f, 0.95f), 110f, 12f);
        PointLight("Arena center", arena.transform, new Vector3(0f, 5f, 14f), new Color(0.12f, 0.5f, 0.9f), 85f, 10f);

        CreatePlayer(new Vector3(0f, 0f, -13f));
        CreateBoss("Card Guard Alpha", new Vector3(-4.3f, 0f, 5f), bossRed, false, 135f, 3.25f, 20f);
        CreateBoss("Card Guard Beta", new Vector3(4.3f, 0f, 8f), bossBlue, true, 165f, 2.25f, 15f);
        CreatePortal("Arena Exit", new Vector3(0f, 2.1f, 19f), SceneTransitionManager.Ending, 2);
        SaveScene(scene);
    }

    private static void BuildEnding()
    {
        Scene scene = BeginScene(SceneTransitionManager.Ending, new Color(0.1f, 0.08f, 0.12f), new Color(0.006f, 0.008f, 0.018f), true);
        GameObject ending = Group("Cliffhanger");
        Cube("Ending platform", ending.transform, new Vector3(0f, -0.35f, 2f), new Vector3(26f, 0.7f, 24f), darkSteel);
        Cube("Black monolith", ending.transform, new Vector3(0f, 4f, 10f), new Vector3(4f, 8f, 1.2f), asphalt);
        Cube("Monolith sigil", ending.transform, new Vector3(0f, 4f, 9.35f), new Vector3(0.25f, 4.5f, 0.1f), bossRed);
        PointLight("Ending light", ending.transform, new Vector3(0f, 5f, 7f), new Color(0.8f, 0.08f, 0.13f), 85f, 10f);
        GameObject hero = CreateRobotVisual(ending.transform, new Vector3(0f, 0f, -2f), Quaternion.identity);
        hero.name = "Timmy - ending witness";
        hero.transform.localScale = Vector3.one * 1.05f;
        new GameObject("Ending Screen").AddComponent<EndingController>();
        SaveScene(scene);
    }

    private static void RegisterBuildScenes()
    {
        List<EditorBuildSettingsScene> scenes = SceneNames
            .Select(name => new EditorBuildSettingsScene($"{SceneFolder}/{name}.unity", true))
            .ToList();

        foreach (EditorBuildSettingsScene existing in EditorBuildSettings.scenes)
        {
            if (existing.enabled && !scenes.Any(scene => scene.path == existing.path))
                scenes.Add(existing);
        }

        EditorBuildSettings.scenes = scenes.ToArray();
    }

    private static void CreateRuntimeSystems(string sceneName)
    {
        GameObject manager = new GameObject("Game Manager");
        manager.AddComponent<GameManager>();

        if (sceneName != SceneTransitionManager.MainMenu && sceneName != SceneTransitionManager.Ending)
        {
            new GameObject("Gameplay HUD").AddComponent<HUDController>();
            new GameObject("Pause Menu").AddComponent<PauseMenuController>();
        }

        GameObject audioObject = new GameObject("Scene Audio");
        AudioSource source = audioObject.AddComponent<AudioSource>();
        source.spatialBlend = 0f;
        source.volume = 0.32f;
        AudioManager audioManager = audioObject.AddComponent<AudioManager>();
        string musicPath = sceneName == SceneTransitionManager.ForestPortal ||
                           sceneName == SceneTransitionManager.MysteryArena ||
                           sceneName == SceneTransitionManager.Ending
            ? EtherealMusicPath
            : SuspenseMusicPath;
        audioManager.Configure(AssetDatabase.LoadAssetAtPath<AudioClip>(musicPath));
    }

    private static void CreateCamera(Color background)
    {
        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = background;
        camera.fieldOfView = 62f;
        camera.nearClipPlane = 0.08f;
        camera.farClipPlane = 220f;
        cameraObject.AddComponent<AudioListener>();
        cameraObject.transform.position = new Vector3(0f, 2.8f, -7f);
    }

    private static void CreateDirectionalLight()
    {
        GameObject lightObject = new GameObject("Moonlight");
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.color = new Color(0.55f, 0.65f, 0.82f);
        light.intensity = 0.72f;
        light.shadows = LightShadows.Soft;
        lightObject.transform.rotation = Quaternion.Euler(48f, -32f, 0f);
    }

    private static void CreatePlayer(Vector3 position)
    {
        GameObject player = CreateRobotVisual(null, position, Quaternion.identity);
        player.name = "Player - Timmy";
        player.tag = "Player";

        foreach (Collider childCollider in player.GetComponentsInChildren<Collider>())
            UnityEngine.Object.DestroyImmediate(childCollider);

        CharacterController character = player.AddComponent<CharacterController>();
        character.height = 1.85f;
        character.radius = 0.34f;
        character.center = new Vector3(0f, 0.92f, 0f);
        character.stepOffset = 0.32f;
        character.slopeLimit = 48f;
        player.AddComponent<PlayerController>();
        player.AddComponent<PlayerHealth>();
        player.AddComponent<PlayerCombat>();

        foreach (Animator animator in player.GetComponentsInChildren<Animator>())
            animator.applyRootMotion = false;

        GameObject cameraTarget = new GameObject("Camera Target");
        cameraTarget.transform.SetParent(player.transform, false);
        cameraTarget.transform.localPosition = new Vector3(0f, 1.45f, 0f);

        GameObject cameraObject = Camera.main != null ? Camera.main.gameObject : null;
        if (cameraObject == null)
            throw new InvalidOperationException("The generated gameplay scene is missing its Main Camera.");
        CameraController cameraController = cameraObject.AddComponent<CameraController>();
        cameraController.SetTarget(cameraTarget.transform);
        cameraObject.transform.position = position + new Vector3(0f, 2.8f, -6.5f);
        cameraObject.transform.LookAt(position + Vector3.up * 1.45f);
    }

    private static GameObject CreateRobotVisual(Transform parent, Vector3 position, Quaternion rotation)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(RobotPrefabPath);
        GameObject instance = prefab != null
            ? PrefabUtility.InstantiatePrefab(prefab) as GameObject
            : GameObject.CreatePrimitive(PrimitiveType.Capsule);

        if (instance == null)
            instance = GameObject.CreatePrimitive(PrimitiveType.Capsule);

        instance.transform.SetParent(parent, true);
        instance.transform.SetPositionAndRotation(position, rotation);
        return instance;
    }

    private static GameObject CreateBoss(string name, Vector3 position, Material bodyMaterial, bool defensive, float health, float speed, float damage)
    {
        GameObject boss = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        boss.name = name;
        boss.transform.position = position;
        boss.transform.localScale = new Vector3(1.2f, 1.15f, 1.2f);
        boss.GetComponent<Renderer>().sharedMaterial = bodyMaterial;
        UnityEngine.Object.DestroyImmediate(boss.GetComponent<CapsuleCollider>());

        GameObject card = Cube("Card sigil", boss.transform, new Vector3(0f, 1.1f, 0.52f), new Vector3(0.42f, 0.55f, 0.14f), gold);
        card.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
        foreach (Collider collider in boss.GetComponentsInChildren<Collider>())
            UnityEngine.Object.DestroyImmediate(collider);

        CharacterController character = boss.AddComponent<CharacterController>();
        character.height = 2.5f;
        character.radius = 0.56f;
        character.center = new Vector3(0f, 1.1f, 0f);
        BossAI ai = boss.AddComponent<BossAI>();
        ai.displayName = name;
        ai.Configure(defensive);
        ai.ConfigureStats(health, speed, 24f, damage, defensive ? 1.9f : 1.2f);
        return boss;
    }

    private static void BuildRoad(Transform parent, float startZ, float endZ, float roadWidth)
    {
        float center = (startZ + endZ) * 0.5f;
        float length = endZ - startZ;
        Cube("Road foundation", parent, new Vector3(0f, -0.34f, center), new Vector3(roadWidth + 18f, 0.68f, length + 8f), concrete);
        Cube("Asphalt", parent, new Vector3(0f, 0.02f, center), new Vector3(roadWidth, 0.08f, length + 6f), asphalt);
        for (float z = startZ + 3f; z < endZ; z += 8f)
            Cube("Center lane dash", parent, new Vector3(0f, 0.075f, z), new Vector3(0.12f, 0.025f, 3.2f), lanePaint);
        Cube("West curb", parent, new Vector3(-roadWidth * 0.5f - 0.18f, 0.1f, center), new Vector3(0.36f, 0.25f, length), policeWhite);
        Cube("East curb", parent, new Vector3(roadWidth * 0.5f + 0.18f, 0.1f, center), new Vector3(0.36f, 0.25f, length), policeWhite);
    }

    private static GameObject Building(Transform parent, string name, Vector3 position, Vector3 size, Material material)
    {
        GameObject building = Cube(name, parent, position + Vector3.up * (size.y * 0.5f), size, material);
        for (int floor = 0; floor < Mathf.FloorToInt(size.y / 2.3f); floor++)
        {
            float y = position.y + 1.2f + floor * 2.3f;
            for (int column = 0; column < 2; column++)
            {
                float x = position.x + (column == 0 ? -size.x * 0.24f : size.x * 0.24f);
                Cube($"{name} window", parent, new Vector3(x, y, position.z - size.z * 0.5f - 0.06f), new Vector3(0.8f, 1.05f, 0.06f), floor % 3 == 0 ? warmLight : darkSteel);
            }
        }
        return building;
    }

    private static void StreetLamp(Transform parent, float x, float z)
    {
        Cylinder("Streetlight post", parent, new Vector3(x, 2.1f, z), new Vector3(0.1f, 2.1f, 0.1f), darkSteel);
        Cube("Streetlight arm", parent, new Vector3(x * 0.8f, 4.12f, z), new Vector3(0.85f, 0.1f, 0.1f), darkSteel);
        Cube("Streetlight fixture", parent, new Vector3(x * 0.72f, 4.02f, z), new Vector3(0.55f, 0.12f, 0.36f), warmLight);
        PointLight("Street pool", parent, new Vector3(x * 0.72f, 3.7f, z), new Color(1f, 0.58f, 0.23f), 8f, 4f);
    }

    private static void Tree(Transform parent, Vector3 position, float scale)
    {
        Cylinder("Tree trunk", parent, position + Vector3.up * (1.45f * scale), new Vector3(0.32f, 1.45f, 0.32f) * scale, treeBark);
        GameObject crown = Sphere("Canopy", parent, position + Vector3.up * (3.45f * scale), new Vector3(2.5f, 3.4f, 2.5f) * scale, foliage);
        crown.transform.localRotation = Quaternion.Euler(0f, position.z * 7f, 0f);
    }

    private static GameObject PoliceCar(Transform parent, Vector3 position, bool moving)
    {
        return Vehicle(parent, moving ? "Pursuit cruiser" : "Police barricade cruiser", position, policeBlack, policeWhite, true);
    }

    private static GameObject Vehicle(Transform parent, string name, Vector3 position, Material body, Material accent, bool addLights)
    {
        GameObject car = Group(name, parent);
        car.transform.position = position;
        Cube("Vehicle chassis", car.transform, new Vector3(0f, 0.72f, 0f), new Vector3(2.15f, 0.72f, 4.2f), body);
        Cube("Vehicle cabin", car.transform, new Vector3(0f, 1.28f, -0.35f), new Vector3(1.55f, 0.72f, 1.9f), accent);
        Cube("Windshield", car.transform, new Vector3(0f, 1.32f, 0.62f), new Vector3(1.38f, 0.5f, 0.06f), darkSteel);
        for (int side = -1; side <= 1; side += 2)
        {
            for (int axle = -1; axle <= 1; axle += 2)
                Cylinder("Wheel", car.transform, new Vector3(side * 1.05f, 0.4f, axle * 1.35f), new Vector3(0.36f, 0.16f, 0.36f), darkSteel, Quaternion.Euler(0f, 0f, 90f));
        }

        if (addLights)
        {
            GameObject red = Cube("Emergency red", car.transform, new Vector3(-0.42f, 1.73f, -0.35f), new Vector3(0.48f, 0.18f, 0.22f), bossRed);
            GameObject blue = Cube("Emergency blue", car.transform, new Vector3(0.42f, 1.73f, -0.35f), new Vector3(0.48f, 0.18f, 0.22f), portalGlow);
            red.name = "Emergency Red Light";
            blue.name = "Emergency Blue Light";
        }
        return car;
    }

    private static void CreatePortal(string name, Vector3 position, string destination, int requiredBosses)
    {
        GameObject portal = new GameObject(name);
        portal.transform.position = position;
        SphereCollider trigger = portal.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 2.8f;
        portal.AddComponent<PortalController>().Configure(destination, requiredBosses);

        GameObject outer = Cylinder("Portal rim", portal.transform, new Vector3(0f, 0f, 0f), new Vector3(1.8f, 0.13f, 2.65f), portalGlow, Quaternion.Euler(90f, 0f, 0f));
        outer.name = "Portal Glow";
        GameObject core = Cylinder("Portal depth", portal.transform, new Vector3(0f, 0f, 0.12f), new Vector3(1.53f, 0.16f, 2.38f), portalCore, Quaternion.Euler(90f, 0f, 0f));
        core.name = "Portal Core";
        PointLight("Portal aura", portal.transform, new Vector3(0f, 0.5f, -1f), new Color(0.14f, 0.52f, 1f), 26f, 4f);
    }

    private static void AddObjectiveTrigger(string name, Vector3 position, Vector3 size, string destination)
    {
        GameObject triggerObject = new GameObject(name);
        triggerObject.transform.position = position;
        BoxCollider trigger = triggerObject.AddComponent<BoxCollider>();
        trigger.isTrigger = true;
        trigger.size = size;
        triggerObject.AddComponent<ObjectiveTrigger>().Configure(destination);
    }

    private static GameObject PointLight(string name, Transform parent, Vector3 position, Color color, float intensity, float range)
    {
        GameObject lightObject = new GameObject(name);
        lightObject.transform.SetParent(parent, false);
        lightObject.transform.localPosition = position;
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Point;
        light.color = color;
        light.intensity = intensity;
        light.range = range;
        light.shadows = LightShadows.None;
        return lightObject;
    }

    private static GameObject Cube(string name, Transform parent, Vector3 position, Vector3 scale, Material material)
    {
        GameObject item = GameObject.CreatePrimitive(PrimitiveType.Cube);
        item.name = name;
        item.transform.SetParent(parent, false);
        item.transform.localPosition = position;
        item.transform.localScale = scale;
        item.GetComponent<Renderer>().sharedMaterial = material;
        return item;
    }

    private static GameObject Cylinder(string name, Transform parent, Vector3 position, Vector3 scale, Material material, Quaternion? rotation = null)
    {
        GameObject item = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        item.name = name;
        item.transform.SetParent(parent, false);
        item.transform.localPosition = position;
        item.transform.localScale = scale;
        item.transform.rotation = rotation ?? Quaternion.identity;
        item.GetComponent<Renderer>().sharedMaterial = material;
        return item;
    }

    private static GameObject Sphere(string name, Transform parent, Vector3 position, Vector3 scale, Material material)
    {
        GameObject item = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        item.name = name;
        item.transform.SetParent(parent, false);
        item.transform.localPosition = position;
        item.transform.localScale = scale;
        item.GetComponent<Renderer>().sharedMaterial = material;
        return item;
    }

    private static GameObject Group(string name, Transform parent = null)
    {
        GameObject group = new GameObject(name);
        if (parent != null)
            group.transform.SetParent(parent, true);
        return group;
    }
}
