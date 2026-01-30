using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DropStack.Visual
{
    public class PixelParticles : MonoBehaviour
    {
        public static PixelParticles Instance { get; private set; }

        [SerializeField] private int particlesPerMerge = 8;
        [SerializeField] private float particleLifetime = 0.6f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void SpawnMergeParticles(Vector3 position, int tier)
        {
            Sprite sprite = PixelArtFactory.Instance.GetTierSprite(tier);
            for (int i = 0; i < particlesPerMerge; i++)
            {
                GameObject particle = new GameObject("MergeParticle");
                particle.transform.position = position;
                SpriteRenderer renderer = particle.AddComponent<SpriteRenderer>();
                renderer.sprite = sprite;
                renderer.sortingOrder = 10;
                renderer.transform.localScale = Vector3.one * 0.2f;
                StartCoroutine(AnimateParticle(particle));
            }
        }

        private IEnumerator AnimateParticle(GameObject particle)
        {
            float time = 0f;
            Vector2 velocity = Random.insideUnitCircle * 1.5f;
            while (time < particleLifetime)
            {
                time += Time.deltaTime;
                particle.transform.position += (Vector3)(velocity * Time.deltaTime);
                particle.transform.localScale = Vector3.Lerp(Vector3.one * 0.2f, Vector3.zero, time / particleLifetime);
                yield return null;
            }
            Destroy(particle);
        }
    }
}
