using UnityEngine;

namespace DropStack.Services
{
    public class MockIapService : IIapService
    {
        private bool noAds;

        public void Purchase(string productId)
        {
            Debug.Log($"Mock purchase: {productId}");
            if (productId == "NoAds")
            {
                noAds = true;
            }
            ServiceLocator.Analytics.IapPurchase(productId);
        }

        public bool IsNoAds()
        {
            return noAds;
        }
    }
}
