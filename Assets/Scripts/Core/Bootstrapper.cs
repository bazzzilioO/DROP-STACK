using System;
using DropStack.UI;
using DropStack.Visual;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DropStack.Core
{
    public class Bootstrapper : MonoBehaviour
    {
        private static Bootstrapper instance;

        [SerializeField] private bool persistAcrossScenes = true;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            if (persistAcrossScenes)
            {
                DontDestroyOnLoad(gameObject);
            }

            EnsureSceneSetup();
        }

        private void EnsureSceneSetup()
        {
            Camera camera = EnsureCamera();
            Canvas canvas = EnsureCanvas();
            EnsureEventSystem();

            PixelArtFactory artFactory = FindObjectOfType<PixelArtFactory>();
            if (artFactory == null)
            {
                artFactory = new GameObject("PixelArtFactory").AddComponent<PixelArtFactory>();
                DontDestroyIfNeeded(artFactory.gameObject);
            }

            PixelParticles particles = FindObjectOfType<PixelParticles>();
            if (particles == null)
            {
                particles = new GameObject("PixelParticles").AddComponent<PixelParticles>();
                DontDestroyIfNeeded(particles.gameObject);
            }

            PixelBackground background = EnsureBackground();

            Transform pieceParent = EnsureTransform("Pieces");
            Transform spawnPoint = EnsureTransform("SpawnPoint", new Vector3(0f, 4.5f, 0f));
            Transform previewAnchor = EnsureTransform("PreviewAnchor", new Vector3(3.5f, 4.2f, 0f));

            Spawner spawner = FindObjectOfType<Spawner>();
            if (spawner == null)
            {
                spawner = new GameObject("Spawner").AddComponent<Spawner>();
                DontDestroyIfNeeded(spawner.gameObject);
            }
            spawner.Configure(spawnPoint, pieceParent, previewAnchor);

            MergeSystem mergeSystem = FindObjectOfType<MergeSystem>();
            if (mergeSystem == null)
            {
                mergeSystem = new GameObject("MergeSystem").AddComponent<MergeSystem>();
                DontDestroyIfNeeded(mergeSystem.gameObject);
            }

            InputController inputController = FindObjectOfType<InputController>();
            if (inputController == null)
            {
                inputController = new GameObject("InputController").AddComponent<InputController>();
                DontDestroyIfNeeded(inputController.gameObject);
            }
            inputController.Configure(EnsureAimLine());

            GameOverTrigger gameOverTrigger = FindObjectOfType<GameOverTrigger>();
            if (gameOverTrigger == null)
            {
                gameOverTrigger = CreateGameOverZone();
                DontDestroyIfNeeded(gameOverTrigger.gameObject);
            }

            EnsureBoardBounds();

            UIController uiController = EnsureUIController(canvas.transform);

            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager == null)
            {
                gameManager = new GameObject("GameManager").AddComponent<GameManager>();
                DontDestroyIfNeeded(gameManager.gameObject);
            }
            gameManager.Configure(spawner, mergeSystem, inputController, gameOverTrigger, uiController, background);

            if (camera == null)
            {
                Debug.LogError("Bootstrapper failed to create a main camera.");
            }
            if (canvas == null)
            {
                Debug.LogError("Bootstrapper failed to create a UI canvas.");
            }
        }

        private Camera EnsureCamera()
        {
            Camera camera = Camera.main;
            if (camera != null)
            {
                EnsurePixelPerfect(camera.gameObject);
                return camera;
            }

            GameObject cameraObject = new GameObject("Main Camera");
            camera = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
            camera.orthographic = true;
            camera.orthographicSize = 7f;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.05f, 0.05f, 0.08f);
            cameraObject.AddComponent<AudioListener>();
            EnsurePixelPerfect(cameraObject);
            return camera;
        }

        private void EnsurePixelPerfect(GameObject cameraObject)
        {
            if (cameraObject.GetComponent<PixelPerfectSetup>() == null)
            {
                cameraObject.AddComponent<PixelPerfectSetup>();
            }

            Type pixelPerfectType = Type.GetType("UnityEngine.U2D.PixelPerfectCamera, Unity.2D.PixelPerfect");
            if (pixelPerfectType != null && cameraObject.GetComponent(pixelPerfectType) == null)
            {
                cameraObject.AddComponent(pixelPerfectType);
            }
        }

        private Canvas EnsureCanvas()
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                return canvas;
            }

            GameObject canvasObject = new GameObject("Canvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private void EnsureEventSystem()
        {
            if (FindObjectOfType<EventSystem>() != null)
            {
                return;
            }
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        private UIController EnsureUIController(Transform canvasTransform)
        {
            UIController controller = FindObjectOfType<UIController>();
            if (controller != null)
            {
                return controller;
            }

            GameObject uiRoot = new GameObject("UIController");
            uiRoot.transform.SetParent(canvasTransform, false);
            controller = uiRoot.AddComponent<UIController>();

            TMP_Text scoreText = CreateText(uiRoot.transform, "ScoreText", new Vector2(16f, -16f), TextAlignmentOptions.Left, 26);
            TMP_Text bestText = CreateText(uiRoot.transform, "BestText", new Vector2(16f, -46f), TextAlignmentOptions.Left, 20);
            TMP_Text currencyText = CreateText(uiRoot.transform, "CurrencyText", new Vector2(16f, -76f), TextAlignmentOptions.Left, 18);
            TMP_Text toastText = CreateText(uiRoot.transform, "ToastText", new Vector2(0f, 120f), TextAlignmentOptions.Center, 18);
            toastText.rectTransform.anchorMin = new Vector2(0.5f, 0f);
            toastText.rectTransform.anchorMax = new Vector2(0.5f, 0f);

            controller.Configure(scoreText, bestText, currencyText, toastText, null, null, null);
            return controller;
        }

        private TMP_Text CreateText(Transform parent, string name, Vector2 anchoredPos, TextAlignmentOptions alignment, int fontSize)
        {
            GameObject textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);
            TMP_Text text = textObject.AddComponent<TextMeshProUGUI>();
            text.text = string.Empty;
            text.fontSize = fontSize;
            text.alignment = alignment;
            RectTransform rect = text.rectTransform;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = anchoredPos;
            rect.sizeDelta = new Vector2(320f, 40f);
            return text;
        }

        private PixelBackground EnsureBackground()
        {
            PixelBackground background = FindObjectOfType<PixelBackground>();
            if (background != null)
            {
                return background;
            }

            GameObject backgroundObject = new GameObject("Background");
            SpriteRenderer renderer = backgroundObject.AddComponent<SpriteRenderer>();
            renderer.sortingOrder = -10;
            background = backgroundObject.AddComponent<PixelBackground>();
            DontDestroyIfNeeded(backgroundObject);
            return background;
        }

        private Transform EnsureTransform(string name, Vector3? position = null)
        {
            GameObject existing = GameObject.Find(name);
            if (existing != null)
            {
                return existing.transform;
            }
            GameObject created = new GameObject(name);
            if (position.HasValue)
            {
                created.transform.position = position.Value;
            }
            return created.transform;
        }

        private LineRenderer EnsureAimLine()
        {
            LineRenderer line = FindObjectOfType<LineRenderer>();
            if (line != null)
            {
                return line;
            }
            GameObject lineObject = new GameObject("AimLine");
            line = lineObject.AddComponent<LineRenderer>();
            line.positionCount = 2;
            line.startWidth = 0.05f;
            line.endWidth = 0.05f;
            line.material = new Material(Shader.Find("Sprites/Default"));
            line.startColor = new Color(1f, 1f, 1f, 0.6f);
            line.endColor = new Color(1f, 1f, 1f, 0.1f);
            return line;
        }

        private GameOverTrigger CreateGameOverZone()
        {
            GameObject zone = new GameObject("GameOverZone");
            BoxCollider2D collider = zone.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(6.5f, 0.8f);
            zone.transform.position = new Vector3(0f, 4.3f, 0f);
            return zone.AddComponent<GameOverTrigger>();
        }

        private void EnsureBoardBounds()
        {
            if (GameObject.Find("BoardBounds") != null)
            {
                return;
            }

            GameObject bounds = new GameObject("BoardBounds");

            CreateWall(bounds.transform, "Floor", new Vector2(0f, -4.8f), new Vector2(6.6f, 0.8f));
            CreateWall(bounds.transform, "LeftWall", new Vector2(-3.6f, 0f), new Vector2(0.8f, 12f));
            CreateWall(bounds.transform, "RightWall", new Vector2(3.6f, 0f), new Vector2(0.8f, 12f));
        }

        private void CreateWall(Transform parent, string name, Vector2 position, Vector2 size)
        {
            GameObject wall = new GameObject(name);
            wall.transform.SetParent(parent, false);
            wall.transform.position = position;
            BoxCollider2D collider = wall.AddComponent<BoxCollider2D>();
            collider.size = size;
        }

        private void DontDestroyIfNeeded(GameObject target)
        {
            if (persistAcrossScenes)
            {
                DontDestroyOnLoad(target);
            }
        }
    }
}
