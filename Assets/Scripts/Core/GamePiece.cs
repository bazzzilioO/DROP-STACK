using DropStack.Modifiers;
using UnityEngine;

namespace DropStack.Core
{
    public class GamePiece : MonoBehaviour
    {
        [SerializeField] private int tier;
        [SerializeField] private int scoreValue;
        private ModifierManager modifierManager;
        private Rigidbody2D body;
        private bool canMerge = true;

        public int Tier => tier;
        public int ScoreValue => scoreValue;
        public Rigidbody2D Body => body;
        public bool CanMerge => canMerge;

        public void Initialize(int tierValue, int score, ModifierManager modifiers)
        {
            tier = tierValue;
            scoreValue = score;
            modifierManager = modifiers;
            body = GetComponent<Rigidbody2D>();
        }

        public void DisableMerge()
        {
            canMerge = false;
        }

        public void EnableMerge()
        {
            canMerge = true;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!canMerge)
            {
                return;
            }
            GamePiece other = collision.gameObject.GetComponent<GamePiece>();
            if (other == null || !other.canMerge)
            {
                return;
            }
            if (other.tier != tier)
            {
                return;
            }
            float relativeSpeed = collision.relativeVelocity.magnitude;
            if (relativeSpeed > modifierManager.MergeVelocityThreshold)
            {
                return;
            }
            MergeSystem mergeSystem = FindObjectOfType<MergeSystem>();
            if (mergeSystem != null)
            {
                mergeSystem.TryMerge(this, other);
            }
        }

        private void FixedUpdate()
        {
            if (modifierManager != null && modifierManager.IsMagnetActive)
            {
                modifierManager.ApplyMagnetToPiece(this);
            }
        }
    }
}
