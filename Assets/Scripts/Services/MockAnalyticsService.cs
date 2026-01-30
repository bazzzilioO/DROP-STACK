using UnityEngine;

namespace DropStack.Services
{
    public class MockAnalyticsService : IAnalyticsService
    {
        public void RunStart()
        {
            Debug.Log("Analytics: run_start");
        }

        public void RunEnd(int score, int tierMax, float duration)
        {
            Debug.Log($"Analytics: run_end score={score} tier={tierMax} duration={duration}");
        }

        public void RewardedShown(RewardedType type)
        {
            Debug.Log($"Analytics: rewarded_shown {type}");
        }

        public void RewardedComplete(RewardedType type)
        {
            Debug.Log($"Analytics: rewarded_complete {type}");
        }

        public void InterstitialShown()
        {
            Debug.Log("Analytics: interstitial_shown");
        }

        public void ModifierPicked(string id)
        {
            Debug.Log($"Analytics: modifier_picked {id}");
        }

        public void IapPurchase(string id)
        {
            Debug.Log($"Analytics: iap_purchase {id}");
        }
    }
}
