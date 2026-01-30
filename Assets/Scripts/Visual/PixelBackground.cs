using DropStack.Core;
using UnityEngine;

namespace DropStack.Visual
{
    public class PixelBackground : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer targetRenderer;
        [SerializeField] private int width = 64;
        [SerializeField] private int height = 128;
        [SerializeField] private float noiseAmount = 0.05f;

        public void Initialize(GameConfig config, SaveData saveData)
        {
            if (targetRenderer == null)
            {
                targetRenderer = GetComponent<SpriteRenderer>();
            }
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;

            Color top = new Color(0.1f, 0.1f, 0.15f);
            Color bottom = new Color(0.05f, 0.05f, 0.08f);
            for (int y = 0; y < height; y++)
            {
                float t = y / (float)height;
                Color row = Color.Lerp(bottom, top, t);
                for (int x = 0; x < width; x++)
                {
                    float noise = Random.Range(-noiseAmount, noiseAmount);
                    Color final = new Color(
                        Mathf.Clamp01(row.r + noise),
                        Mathf.Clamp01(row.g + noise),
                        Mathf.Clamp01(row.b + noise),
                        1f);
                    texture.SetPixel(x, y, final);
                }
            }
            texture.Apply();
            targetRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, width, height), Vector2.one * 0.5f, 16f);
        }
    }
}
