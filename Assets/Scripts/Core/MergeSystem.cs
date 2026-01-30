using System.Collections.Generic;
using System.Linq;
using DropStack.Modifiers;
using DropStack.Visual;
using UnityEngine;

namespace DropStack.Core
{
    public class MergeSystem : MonoBehaviour
    {
        private GameManager gameManager;
        private ModifierManager modifierManager;
        private GameConfig config;
        private readonly HashSet<GamePiece> lockedPieces = new HashSet<GamePiece>();

        public void Initialize(GameManager manager, ModifierManager modifiers)
        {
            gameManager = manager;
            modifierManager = modifiers;
            config = manager.Config;
        }

        public void TryMerge(GamePiece a, GamePiece b)
        {
            if (lockedPieces.Contains(a) || lockedPieces.Contains(b))
            {
                return;
            }
            lockedPieces.Add(a);
            lockedPieces.Add(b);

            int nextTier = Mathf.Min(a.Tier + modifierManager.MergeTierBonus, config.gameplay.maxTier);
            Vector3 spawnPosition = (a.transform.position + b.transform.position) / 2f;
            int scoreValue = a.ScoreValue + b.ScoreValue;

            Destroy(a.gameObject);
            Destroy(b.gameObject);

            SpawnMergedPiece(nextTier, spawnPosition);
            gameManager.AddScore(scoreValue);
            gameManager.RegisterMerge(nextTier);
            PixelParticles.Instance.SpawnMergeParticles(spawnPosition, nextTier);
            modifierManager.OnMergeTriggered();
        }

        private void SpawnMergedPiece(int tier, Vector3 position)
        {
            TierConfig tierConfig = config.tiers.FirstOrDefault(t => t.tier == tier);
            if (tierConfig == null)
            {
                return;
            }

            GameObject piece = new GameObject($"Piece_T{tier}");
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
        }

        public void ClearAllPieces()
        {
            foreach (GamePiece piece in FindObjectsOfType<GamePiece>())
            {
                Destroy(piece.gameObject);
            }
        }
    }
}
