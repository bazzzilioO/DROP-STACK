using UnityEngine;
using UnityEngine.U2D;

namespace DropStack.Visual
{
    [RequireComponent(typeof(PixelPerfectCamera))]
    public class PixelPerfectSetup : MonoBehaviour
    {
        [SerializeField] private int assetsPixelsPerUnit = 16;
        [SerializeField] private int referenceResolutionX = 360;
        [SerializeField] private int referenceResolutionY = 640;

        private void Awake()
        {
            PixelPerfectCamera camera = GetComponent<PixelPerfectCamera>();
            camera.assetsPPU = assetsPixelsPerUnit;
            camera.refResolutionX = referenceResolutionX;
            camera.refResolutionY = referenceResolutionY;
            camera.upscaleRT = true;
            camera.pixelSnapping = true;
        }
    }
}
