using UnityEngine;

namespace DropStack.Core
{
    public class InputController : MonoBehaviour
    {
        [SerializeField] private float minX = -2.5f;
        [SerializeField] private float maxX = 2.5f;
        [SerializeField] private float spawnY = 4.5f;
        [SerializeField] private LineRenderer aimLine;

        private GameManager gameManager;
        private Spawner spawner;
        private bool isDragging;
        private Vector3 currentPosition;

        public void Configure(LineRenderer aimLineRef)
        {
            aimLine = aimLineRef;
        }

        public void Initialize(GameManager manager, Spawner spawn)
        {
            gameManager = manager;
            spawner = spawn;
            currentPosition = new Vector3(0f, spawnY, 0f);
            UpdateAimLine();
        }

        private void Update()
        {
            if (gameManager == null || gameManager.IsGameOver)
            {
                return;
            }
            HandleInput();
        }

        private void HandleInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
            }
            if (Input.GetMouseButton(0) && isDragging)
            {
                Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                currentPosition.x = Mathf.Clamp(world.x, minX, maxX);
                if (gameManager.Modifiers.IsSnapActive)
                {
                    currentPosition.x = SnapToBestX(currentPosition.x);
                }
                currentPosition.y = spawnY;
                UpdateAimLine();
            }
            if (Input.GetMouseButtonUp(0) && isDragging)
            {
                isDragging = false;
                DropPiece();
            }
        }

        private void DropPiece()
        {
            spawner.SpawnPiece(currentPosition);
        }

        private void UpdateAimLine()
        {
            if (aimLine == null)
            {
                return;
            }
            aimLine.positionCount = 2;
            aimLine.SetPosition(0, currentPosition + Vector3.up * 1.2f);
            aimLine.SetPosition(1, currentPosition + Vector3.down * 6f);
        }

        private float SnapToBestX(float currentX)
        {
            GamePiece[] pieces = FindObjectsOfType<GamePiece>();
            float bestX = currentX;
            float bestDist = float.MaxValue;
            foreach (GamePiece piece in pieces)
            {
                float dist = Mathf.Abs(piece.transform.position.x - currentX);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestX = piece.transform.position.x;
                }
            }
            return Mathf.Clamp(bestX, minX, maxX);
        }
    }
}
