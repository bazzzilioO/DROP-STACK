using UnityEngine;

namespace DropStack.Services
{
    public class MockAdsService : IAdsService
    {
        private float rerollCooldown;

        public bool CanShowRewarded(RewardedType type)
        {
            if (type == RewardedType.Reroll)
            {
                return Time.time >= rerollCooldown;
            }
            return true;
        }

        public void ShowRewarded(RewardedType type)
        {
            if (!CanShowRewarded(type))
            {
                return;
            }
            Debug.Log($"Mock rewarded shown: {type}");
            if (type == RewardedType.Reroll)
            {
                rerollCooldown = Time.time + 30f;
            }
            ServiceLocator.Analytics.RewardedShown(type);
            ServiceLocator.Analytics.RewardedComplete(type);
        }

        public void ShowInterstitial()
        {
            Debug.Log("Mock interstitial shown");
            ServiceLocator.Analytics.InterstitialShown();
        }
    }
}
