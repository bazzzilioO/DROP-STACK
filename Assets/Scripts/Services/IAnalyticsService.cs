namespace DropStack.Services
{
    public interface IAnalyticsService
    {
        void RunStart();
        void RunEnd(int score, int tierMax, float duration);
        void RewardedShown(RewardedType type);
        void RewardedComplete(RewardedType type);
        void InterstitialShown();
        void ModifierPicked(string id);
        void IapPurchase(string id);
    }
}
