using UnityEngine;

namespace DropStack.Visual
{
    public class PixelSnapRenderer : MonoBehaviour
    {
        [SerializeField] private int pixelsPerUnit = 16;
        private SpriteRenderer targetRenderer;
        private Vector3 originalLocalPosition;

        public void Initialize(SpriteRenderer renderer)
        {
            targetRenderer = renderer;
            originalLocalPosition = renderer.transform.localPosition;
        }

        private void LateUpdate()
        {
            if (targetRenderer == null)
            {
                return;
            }
            Vector3 worldPos = targetRenderer.transform.position;
            float pixelSize = 1f / pixelsPerUnit;
            float snappedX = Mathf.Round(worldPos.x / pixelSize) * pixelSize;
            float snappedY = Mathf.Round(worldPos.y / pixelSize) * pixelSize;
            Vector3 snapped = new Vector3(snappedX, snappedY, worldPos.z);
            targetRenderer.transform.position = snapped;
        }

        private void OnDisable()
        {
            if (targetRenderer != null)
            {
                targetRenderer.transform.localPosition = originalLocalPosition;
            }
        }
    }
}
