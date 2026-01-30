using UnityEngine;
#if UNITY_2D_PIXEL_PERFECT
using UnityEngine.U2D;
#endif

namespace DropStack.Visual
{
#if UNITY_2D_PIXEL_PERFECT
    [RequireComponent(typeof(PixelPerfectCamera))]
#endif
    public class PixelPerfectSetup : MonoBehaviour
    {
        [SerializeField] private int assetsPixelsPerUnit = 16;
        [SerializeField] private int referenceResolutionX = 360;
        [SerializeField] private int referenceResolutionY = 640;

        private void Awake()
        {
#if UNITY_2D_PIXEL_PERFECT
            PixelPerfectCamera camera = GetComponent<PixelPerfectCamera>();
            if (camera == null)
            {
                return;
            }
            camera.assetsPPU = assetsPixelsPerUnit;
            camera.refResolutionX = referenceResolutionX;
            camera.refResolutionY = referenceResolutionY;
            camera.upscaleRT = true;
            camera.pixelSnapping = true;
#else
            enabled = false;
#endif
        }
    }
}
