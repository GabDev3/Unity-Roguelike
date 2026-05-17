using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Editor
{
    /// <summary>
    /// Editor utility to create a complete MainMenu scene with one click.
    /// Access via Unity menu: Tools > Dungeon Game > Create Main Menu Scene
    /// </summary>
    public static class MainMenuSetupEditor
    {
        [MenuItem("Tools/Dungeon Game/Create Main Menu Scene")]
        public static void CreateMainMenuScene()
        {
            // Ask to save current scene
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            // Create new scene
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Add Camera
            GameObject cameraObj = new GameObject("Main Camera");
            Camera camera = cameraObj.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 5;
            camera.backgroundColor = new Color(0.1f, 0.1f, 0.15f, 1f);
            camera.clearFlags = CameraClearFlags.SolidColor;
            cameraObj.AddComponent<AudioListener>();
            cameraObj.tag = "MainCamera";

            // Add MainMenuBuilder
            GameObject builderObj = new GameObject("MainMenuBuilder");
            builderObj.AddComponent<UI.MainMenuBuilder>();

            // Add GameManager if the script exists
            System.Type gameManagerType = System.Type.GetType("Dungeon.GameManager, Assembly-CSharp");
            if (gameManagerType != null)
            {
                GameObject gameManagerObj = new GameObject("GameManager");
                gameManagerObj.AddComponent(gameManagerType);
            }

            // Save the scene
            string scenePath = "Assets/Scenes/MainMenu.unity";
            
            // Create Scenes folder if it doesn't exist
            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            {
                AssetDatabase.CreateFolder("Assets", "Scenes");
            }

            EditorSceneManager.SaveScene(newScene, scenePath);

            // Add to build settings if not already there
            AddSceneToBuildSettings(scenePath);

            Debug.Log($"Main Menu Scene created at: {scenePath}\n" +
                      "The MainMenuBuilder will automatically create all UI elements at runtime.\n" +
                      "Press Play to see the complete menu!");

            EditorUtility.DisplayDialog("Main Menu Created",
                "Main Menu scene has been created at:\n" + scenePath + "\n\n" +
                "The MainMenuBuilder component will automatically create all UI elements when you press Play.\n\n" +
                "You can customize colors, title, and other settings in the MainMenuBuilder inspector.",
                "OK");
        }

        [MenuItem("Tools/Dungeon Game/Add MainMenuBuilder to Current Scene")]
        public static void AddMainMenuBuilderToScene()
        {
            // Check if one already exists
            UI.MainMenuBuilder existingBuilder = Object.FindAnyObjectByType<UI.MainMenuBuilder>();
            if (existingBuilder != null)
            {
                EditorUtility.DisplayDialog("Already Exists",
                    "A MainMenuBuilder already exists in this scene.",
                    "OK");
                Selection.activeGameObject = existingBuilder.gameObject;
                return;
            }

            // Create the builder
            GameObject builderObj = new GameObject("MainMenuBuilder");
            builderObj.AddComponent<UI.MainMenuBuilder>();

            Selection.activeGameObject = builderObj;
            EditorGUIUtility.PingObject(builderObj);

            Debug.Log("MainMenuBuilder added to scene. Press Play to see the generated UI!");
        }

        [MenuItem("Tools/Dungeon Game/Setup Build Settings for Game")]
        public static void SetupBuildSettings()
        {
            string mainMenuPath = "Assets/Scenes/MainMenu.unity";
            string gameScenePath = "Assets/Scenes/SampleScene.unity";

            bool mainMenuAdded = AddSceneToBuildSettings(mainMenuPath, 0);
            bool gameSceneAdded = AddSceneToBuildSettings(gameScenePath, 1);

            string message = "";
            if (mainMenuAdded || gameSceneAdded)
            {
                message = "Build settings updated:\n\n";
                if (mainMenuAdded)
                    message += "✓ MainMenu scene added (index 0)\n";
                if (gameSceneAdded)
                    message += "✓ SampleScene added (index 1)\n";
            }
            else
            {
                message = "Both scenes were already in build settings.";
            }

            EditorUtility.DisplayDialog("Build Settings", message, "OK");
        }

        [MenuItem("Tools/Dungeon Game/Add InGameUIBuilder to Current Scene")]
        public static void AddInGameUIBuilderToScene()
        {
            // Check if one already exists
            UI.InGameUIBuilder existingBuilder = Object.FindAnyObjectByType<UI.InGameUIBuilder>();
            if (existingBuilder != null)
            {
                EditorUtility.DisplayDialog("Already Exists",
                    "An InGameUIBuilder already exists in this scene.",
                    "OK");
                Selection.activeGameObject = existingBuilder.gameObject;
                return;
            }

            // Create the builder
            GameObject builderObj = new GameObject("InGameUIBuilder");
            builderObj.AddComponent<UI.InGameUIBuilder>();

            Selection.activeGameObject = builderObj;
            EditorGUIUtility.PingObject(builderObj);

            Debug.Log("InGameUIBuilder added to scene. Press Play to see the generated UI (Pause Menu, Game Over Screen, Loading Screen)!");
            
            EditorUtility.DisplayDialog("InGameUIBuilder Added",
                "InGameUIBuilder has been added to the scene.\n\n" +
                "When you press Play, it will automatically create:\n" +
                "• Pause Menu (press ESC)\n" +
                "• Game Over Screen\n" +
                "• Loading Screen\n\n" +
                "Customize colors and settings in the Inspector.",
                "OK");
        }

        private static bool AddSceneToBuildSettings(string scenePath, int desiredIndex = -1)
        {
            // Check if asset exists
            if (!System.IO.File.Exists(scenePath))
            {
                Debug.LogWarning($"Scene not found: {scenePath}");
                return false;
            }

            // Get current build scenes
            var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);

            // Check if scene is already in build settings
            foreach (var scene in scenes)
            {
                if (scene.path == scenePath)
                {
                    return false; // Already exists
                }
            }

            // Add the scene
            EditorBuildSettingsScene newScene = new EditorBuildSettingsScene(scenePath, true);
            
            if (desiredIndex >= 0 && desiredIndex < scenes.Count)
            {
                scenes.Insert(desiredIndex, newScene);
            }
            else
            {
                scenes.Add(newScene);
            }

            EditorBuildSettings.scenes = scenes.ToArray();
            return true;
        }
    }
}

