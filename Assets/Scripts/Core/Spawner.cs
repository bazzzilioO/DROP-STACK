using System.Collections.Generic;
using System.Linq;
using DropStack.Modifiers;
using DropStack.Visual;
using UnityEngine;

namespace DropStack.Core
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private Transform pieceParent;
        [SerializeField] private Transform previewAnchor;

        private GameManager gameManager;
        private ModifierManager modifierManager;
        private GameConfig config;
        private int currentTier;
        private int lastSpawnedTier;
        private GameObject previewObject;

        public int CurrentTier => currentTier;
        public int LastSpawnedTier => lastSpawnedTier;

        public void Initialize(GameManager manager, ModifierManager modifiers)
        {
            gameManager = manager;
            modifierManager = modifiers;
            config = manager.Config;
            GenerateNextTier();
        }

        public void ResetSpawner()
        {
            ClearPreview();
            GenerateNextTier();
        }

        public GameObject SpawnPiece(Vector3 position)
        {
            int tier = currentTier;
            GenerateNextTier();

            TierConfig tierConfig = config.tiers.FirstOrDefault(t => t.tier == tier);
            if (tierConfig == null)
            {
                tierConfig = config.tiers[0];
                tier = tierConfig.tier;
            }

            GameObject piece = new GameObject($"Piece_T{tier}");
            piece.transform.SetParent(pieceParent, false);
            piece.transform.position = position;

            SpriteRenderer renderer = piece.AddComponent<SpriteRenderer>();
            renderer.sprite = PixelArtFactory.Instance.GetTierSprite(tier);
            renderer.sortingOrder = 5;

            Rigidbody2D body = piece.AddComponent<Rigidbody2D>();
            body.mass = tierConfig.mass;
            body.gravityScale = 1f;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;

            CircleCollider2D collider = piece.AddComponent<CircleCollider2D>();
            collider.radius = tierConfig.radius;

            GamePiece gamePiece = piece.AddComponent<GamePiece>();
            gamePiece.Initialize(tier, tierConfig.score, modifierManager);

            PixelSnapRenderer snap = piece.AddComponent<PixelSnapRenderer>();
            snap.Initialize(renderer);

            lastSpawnedTier = tier;
            gameManager.HandlePieceSpawned(tier);
            return piece;
        }

        public void SpawnPreview()
        {
            ClearPreview();
            previewObject = new GameObject("NextPreview");
            previewObject.transform.SetParent(previewAnchor, false);
            SpriteRenderer renderer = previewObject.AddComponent<SpriteRenderer>();
            renderer.sprite = PixelArtFactory.Instance.GetTierSprite(currentTier);
            renderer.sortingOrder = 20;
            PixelSnapRenderer snap = previewObject.AddComponent<PixelSnapRenderer>();
            snap.Initialize(renderer);
        }

        private void ClearPreview()
        {
            if (previewObject != null)
            {
                Destroy(previewObject);
            }
        }

        private void GenerateNextTier()
        {
            currentTier = ChooseTier();
            SpawnPreview();
        }

        private int ChooseTier()
        {
            List<SpawnWeight> weights = config.spawnWeights;
            float total = weights.Sum(w => w.weight);
            float roll = Random.Range(0f, total);
            float cumulative = 0f;
            foreach (SpawnWeight weight in weights)
            {
                cumulative += weight.weight;
                if (roll <= cumulative)
                {
                    return weight.tier;
                }
            }
            return weights.Count > 0 ? weights[0].tier : 0;
        }
    }
}
