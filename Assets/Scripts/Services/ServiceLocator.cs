namespace DropStack.Services
{
    public static class ServiceLocator
    {
        public static IAdsService Ads { get; private set; }
        public static IIapService Iap { get; private set; }
        public static IAnalyticsService Analytics { get; private set; }

        public static void Initialize()
        {
            Ads = new MockAdsService();
            Iap = new MockIapService();
            Analytics = new MockAnalyticsService();
        }
    }
}
