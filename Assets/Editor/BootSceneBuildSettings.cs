#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DropStack.Core;
using DropStack.UI;
using DropStack.Visual;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DropStack.Editor
{
    [InitializeOnLoad]
    public static class BootSceneBuildSettings
    {
        private const string BootScenePath = "Assets/Scenes/Boot.unity";

        static BootSceneBuildSettings()
        {
            EnsureBootScene();
        }

        private static void EnsureBootScene()
        {
            EnsureScenesFolder();
            if (!File.Exists(BootScenePath))
            {
                CreateBootScene();
            }
            EnsureBuildSettings();
        }

        private static void EnsureScenesFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
            {
                AssetDatabase.CreateFolder("Assets", "Scenes");
            }
        }

        private static void CreateBootScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject bootstrap = new GameObject("GameBootstrap");
            bootstrap.AddComponent<Bootstrapper>();

            GameObject cameraObject = new GameObject("Main Camera");
            Camera camera = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
            camera.orthographic = true;
            camera.orthographicSize = 7f;
            cameraObject.AddComponent<AudioListener>();
            cameraObject.AddComponent<PixelPerfectSetup>();

            GameObject canvasObject = new GameObject("Canvas");
            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();

            GameObject uiController = new GameObject("UIController");
            uiController.transform.SetParent(canvasObject.transform, false);
            uiController.AddComponent<UIController>();

            if (Object.FindObjectOfType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }

            EditorSceneManager.SaveScene(scene, BootScenePath);
        }

        private static void EnsureBuildSettings()
        {
            List<EditorBuildSettingsScene> scenes = EditorBuildSettings.scenes.ToList();
            int existingIndex = scenes.FindIndex(scene => scene.path == BootScenePath);
            if (existingIndex >= 0)
            {
                EditorBuildSettingsScene existing = scenes[existingIndex];
                scenes.RemoveAt(existingIndex);
                scenes.Insert(0, existing);
            }
            else
            {
                scenes.Insert(0, new EditorBuildSettingsScene(BootScenePath, true));
            }
            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
#endif
