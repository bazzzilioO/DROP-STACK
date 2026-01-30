namespace DropStack.Services
{
    public interface IIapService
    {
        void Purchase(string productId);
        bool IsNoAds();
    }
}
