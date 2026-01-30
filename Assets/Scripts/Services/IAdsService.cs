namespace DropStack.Services
{
    public enum RewardedType
    {
        Continue,
        Reroll,
        DoubleReward
    }

    public interface IAdsService
    {
        bool CanShowRewarded(RewardedType type);
        void ShowRewarded(RewardedType type);
        void ShowInterstitial();
    }
}
