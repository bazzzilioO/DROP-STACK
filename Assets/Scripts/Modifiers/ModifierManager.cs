using System.Collections.Generic;
using System.Linq;
using DropStack.Core;
using DropStack.Services;
using DropStack.Visual;
using UnityEngine;

namespace DropStack.Modifiers
{
    public class ModifierManager
    {
        private readonly GameConfig config;
        private readonly UnlockSystem unlockSystem;
        private readonly GameManager gameManager;
        private readonly Dictionary<string, IModifier> modifiers = new Dictionary<string, IModifier>();
        private readonly Dictionary<string, ModifierConfig> configMap = new Dictionary<string, ModifierConfig>();
        private readonly HashSet<string> activeModifiers = new HashSet<string>();

        public float MergeVelocityThreshold => config.gameplay.mergeVelocityThreshold;
        public int MergeTierBonus { get; private set; }
        public bool HasShield { get; private set; }
        public float ScoreMultiplier { get; private set; } = 1f;
        public bool ExtraPickActive { get; private set; }

        private float magnetTimer;
        private float iceTimer;
        private float bounceTimer;
        private float ghostTimer;
        private float slowTimeTimer;
        private float snapTimer;
        private float scoreBoostTimer;

        private float magnetStrength;

        public ModifierManager(GameConfig gameConfig, UnlockSystem unlocks, GameManager manager)
        {
            config = gameConfig;
            unlockSystem = unlocks;
            gameManager = manager;
            foreach (ModifierConfig modifier in config.modifiers)
            {
                configMap[modifier.id] = modifier;
            }
            Register(new MagnetModifier());
            Register(new IceModifier());
            Register(new BounceModifier());
            Register(new GhostModifier());
            Register(new DoubleMergeModifier());
            Register(new ShieldModifier());
            Register(new SlowTimeModifier());
            Register(new SnapModifier());
            Register(new SplitModifier());
            Register(new CleanseModifier());
            Register(new BoostScoreModifier());
            Register(new ExtraPickModifier());
        }

        private void Register(IModifier modifier)
        {
            modifiers[modifier.Id] = modifier;
        }

        public List<ModifierCardData> GetRandomChoices(int extraCount)
        {
            List<string> available = unlockSystem.GetUnlockedModifiers();
            int count = 3 + extraCount;
            List<string> picks = available.OrderBy(_ => Random.value).Take(count).ToList();
            return picks.Select(ToCardData).ToList();
        }

        public ModifierCardData ToCardData(string id)
        {
            ModifierConfig modifierConfig = configMap[id];
            Color color = PixelArtFactory.Instance.GetPalette()[Mathf.Abs(id.GetHashCode()) % PixelArtFactory.Instance.GetPalette().Count];
            return new ModifierCardData
            {
                id = id,
                displayName = modifierConfig.displayName,
                description = modifierConfig.description,
                icon = PixelArtFactory.Instance.GetModifierIcon(id, color)
            };
        }

        public void ActivateModifier(string id)
        {
            if (!modifiers.TryGetValue(id, out IModifier modifier))
            {
                return;
            }
            modifier.Activate(gameManager, this);
            activeModifiers.Add(id);
            ServiceLocator.Analytics.ModifierPicked(id);
        }

        public void DeactivateModifier(string id)
        {
            if (!modifiers.TryGetValue(id, out IModifier modifier))
            {
                return;
            }
            modifier.Deactivate(gameManager, this);
            activeModifiers.Remove(id);
        }

        public void OnMergeTriggered()
        {
            if (MergeTierBonus > 0)
            {
                MergeTierBonus = 0;
            }
        }

        public void Update(float deltaTime)
        {
            UpdateTimer(ref magnetTimer, deltaTime, () => magnetStrength = 0f);
            UpdateTimer(ref iceTimer, deltaTime, ClearIce);
            UpdateTimer(ref bounceTimer, deltaTime, ClearBounce);
            UpdateTimer(ref ghostTimer, deltaTime, ClearGhost);
            UpdateTimer(ref slowTimeTimer, deltaTime, ClearSlowTime);
            UpdateTimer(ref snapTimer, deltaTime, ClearSnap);
            UpdateTimer(ref scoreBoostTimer, deltaTime, ClearScoreBoost);
        }

        private void UpdateTimer(ref float timer, float deltaTime, System.Action onEnd)
        {
            if (timer <= 0f)
            {
                return;
            }
            timer -= deltaTime;
            if (timer <= 0f)
            {
                onEnd?.Invoke();
            }
        }

        public void ActivateMagnet(float duration, float strength)
        {
            magnetTimer = duration;
            magnetStrength = strength;
        }

        public void ApplyMagnetToPiece(GamePiece piece)
        {
            if (magnetTimer <= 0f || magnetStrength <= 0f)
            {
                return;
            }
            GamePiece[] pieces = Object.FindObjectsOfType<GamePiece>();
            GamePiece closest = null;
            float best = float.MaxValue;
            foreach (GamePiece other in pieces)
            {
                if (other == piece || other.Tier != piece.Tier)
                {
                    continue;
                }
                float dist = Vector2.Distance(piece.transform.position, other.transform.position);
                if (dist < best)
                {
                    best = dist;
                    closest = other;
                }
            }
            if (closest != null)
            {
                Vector2 dir = (closest.transform.position - piece.transform.position).normalized;
                piece.Body.AddForce(dir * magnetStrength);
            }
        }

        public void ActivateIce(float duration)
        {
            iceTimer = duration;
            foreach (Rigidbody2D body in Object.FindObjectsOfType<Rigidbody2D>())
            {
                body.drag = 1.5f;
                body.angularDrag = 1.5f;
            }
        }

        private void ClearIce()
        {
            foreach (Rigidbody2D body in Object.FindObjectsOfType<Rigidbody2D>())
            {
                body.drag = 0.2f;
                body.angularDrag = 0.2f;
            }
        }

        public void ActivateBounce(float duration)
        {
            bounceTimer = duration;
            foreach (PhysicsMaterial2D material in Resources.FindObjectsOfTypeAll<PhysicsMaterial2D>())
            {
                material.bounciness = 0.9f;
            }
        }

        private void ClearBounce()
        {
            foreach (PhysicsMaterial2D material in Resources.FindObjectsOfTypeAll<PhysicsMaterial2D>())
            {
                material.bounciness = 0.2f;
            }
        }

        public void ActivateGhost(float duration)
        {
            ghostTimer = duration;
            GamePiece[] pieces = Object.FindObjectsOfType<GamePiece>();
            foreach (GamePiece piece in pieces)
            {
                if (piece.Body != null)
                {
                    piece.Body.isKinematic = true;
                }
            }
        }

        private void ClearGhost()
        {
            foreach (GamePiece piece in Object.FindObjectsOfType<GamePiece>())
            {
                if (piece.Body != null)
                {
                    piece.Body.isKinematic = false;
                }
            }
        }

        public void ActivateDoubleMerge()
        {
            MergeTierBonus = 1;
        }

        public void ActivateShield()
        {
            HasShield = true;
        }

        public void ConsumeShield()
        {
            HasShield = false;
        }

        public void ActivateSlowTime(float duration)
        {
            slowTimeTimer = duration;
            Time.timeScale = 0.6f;
        }

        private void ClearSlowTime()
        {
            Time.timeScale = 1f;
        }

        public void ActivateSnap(float duration)
        {
            snapTimer = duration;
        }

        private void ClearSnap()
        {
            snapTimer = 0f;
        }

        public bool IsSnapActive => snapTimer > 0f;

        public void ActivateScoreBoost(float duration)
        {
            scoreBoostTimer = duration;
            ScoreMultiplier = 1.25f;
        }

        private void ClearScoreBoost()
        {
            ScoreMultiplier = 1f;
        }

        public void ActivateExtraPick()
        {
            ExtraPickActive = true;
        }

        public void ClearExtraPick()
        {
            ExtraPickActive = false;
        }

        public void TriggerSplit(GamePiece target)
        {
            if (target == null || target.Tier <= 0)
            {
                return;
            }
            Vector3 position = target.transform.position;
            int newTier = target.Tier - 1;
            Object.Destroy(target.gameObject);
            for (int i = 0; i < 2; i++)
            {
                GameObject piece = new GameObject($"SplitPiece_T{newTier}");
                piece.transform.position = position + new Vector3(i == 0 ? -0.2f : 0.2f, 0f, 0f);
                SpriteRenderer renderer = piece.AddComponent<SpriteRenderer>();
                renderer.sprite = PixelArtFactory.Instance.GetTierSprite(newTier);
                Rigidbody2D body = piece.AddComponent<Rigidbody2D>();
                body.mass = newTier + 1;
                CircleCollider2D collider = piece.AddComponent<CircleCollider2D>();
                collider.radius = 0.3f + newTier * 0.05f;
                GamePiece gamePiece = piece.AddComponent<GamePiece>();
                gamePiece.Initialize(newTier, 10, this);
                PixelSnapRenderer snap = piece.AddComponent<PixelSnapRenderer>();
                snap.Initialize(renderer);
            }
        }

        public void TriggerCleanse()
        {
            GamePiece[] pieces = Object.FindObjectsOfType<GamePiece>();
            GamePiece smallest = pieces.OrderBy(p => p.Tier).FirstOrDefault();
            if (smallest != null)
            {
                Object.Destroy(smallest.gameObject);
            }
        }

        public bool IsMagnetActive => magnetTimer > 0f;
        public float MagnetStrength => magnetStrength;
    }

    public struct ModifierCardData
    {
        public string id;
        public string displayName;
        public string description;
        public Sprite icon;
    }
}
