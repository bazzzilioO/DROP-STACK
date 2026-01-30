using System.Collections.Generic;
using System.Linq;
using DropStack.Core;
using UnityEngine;

namespace DropStack.Visual
{
    public class PixelArtFactory : MonoBehaviour
    {
        public static PixelArtFactory Instance { get; private set; }

        [SerializeField] private int spriteSize = 32;
        [SerializeField] private int pixelsPerUnit = 16;

        private readonly Dictionary<int, Sprite> tierSprites = new Dictionary<int, Sprite>();
        private readonly Dictionary<string, Sprite> iconSprites = new Dictionary<string, Sprite>();
        private List<Color> palette;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            palette = DefaultPalette();
        }

        public Sprite GetTierSprite(int tier)
        {
            if (tierSprites.TryGetValue(tier, out Sprite sprite))
            {
                return sprite;
            }
            GameConfig config = ConfigLoader.Load();
            TierConfig tierConfig = config.tiers.FirstOrDefault(t => t.tier == tier) ?? config.tiers.First();
            Color baseColor = palette[tierConfig.colorId % palette.Count];
            Texture2D texture = GenerateOrbTexture(baseColor, spriteSize);
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, pixelsPerUnit);
            tierSprites[tier] = sprite;
            return sprite;
        }

        public Sprite GetModifierIcon(string id, Color baseColor)
        {
            if (iconSprites.TryGetValue(id, out Sprite sprite))
            {
                return sprite;
            }
            Texture2D texture = GenerateIconTexture(baseColor, spriteSize);
            sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f, pixelsPerUnit);
            iconSprites[id] = sprite;
            return sprite;
        }

        public List<Color> GetPalette()
        {
            return palette;
        }

        private Texture2D GenerateOrbTexture(Color baseColor, int size)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;

            Color outline = Darken(baseColor, 0.4f);
            Color highlight = Lighten(baseColor, 0.35f);
            Color shadow = Darken(baseColor, 0.25f);

            int radius = size / 2 - 2;
            Vector2 center = new Vector2(size / 2f, size / 2f);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 pos = new Vector2(x, y);
                    float dist = Vector2.Distance(center, pos);
                    if (dist > radius + 1)
                    {
                        texture.SetPixel(x, y, Color.clear);
                        continue;
                    }
                    if (dist > radius)
                    {
                        texture.SetPixel(x, y, outline);
                        continue;
                    }
                    Color color = baseColor;
                    if (x < size * 0.4f && y > size * 0.6f)
                    {
                        color = highlight;
                    }
                    if (x > size * 0.6f && y < size * 0.4f)
                    {
                        color = shadow;
                    }
                    texture.SetPixel(x, y, color);
                }
            }
            texture.Apply();
            return texture;
        }

        private Texture2D GenerateIconTexture(Color baseColor, int size)
        {
            Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;

            Color outline = Darken(baseColor, 0.4f);
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    bool border = x == 0 || y == 0 || x == size - 1 || y == size - 1;
                    texture.SetPixel(x, y, border ? outline : baseColor);
                }
            }
            for (int i = 4; i < size - 4; i++)
            {
                texture.SetPixel(i, size - 5, Color.white);
            }
            texture.Apply();
            return texture;
        }

        private static Color Darken(Color color, float amount)
        {
            return new Color(color.r * (1 - amount), color.g * (1 - amount), color.b * (1 - amount), color.a);
        }

        private static Color Lighten(Color color, float amount)
        {
            return new Color(Mathf.Lerp(color.r, 1f, amount), Mathf.Lerp(color.g, 1f, amount), Mathf.Lerp(color.b, 1f, amount), color.a);
        }

        private static List<Color> DefaultPalette()
        {
            return new List<Color>
            {
                new Color(0.84f, 0.35f, 0.38f),
                new Color(0.94f, 0.62f, 0.25f),
                new Color(0.96f, 0.82f, 0.35f),
                new Color(0.48f, 0.78f, 0.46f),
                new Color(0.34f, 0.67f, 0.86f),
                new Color(0.34f, 0.42f, 0.9f),
                new Color(0.72f, 0.36f, 0.88f),
                new Color(0.85f, 0.5f, 0.72f)
            };
        }
    }
}
