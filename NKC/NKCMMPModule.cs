namespace NKC;

public abstract class NKCMMPModule
{
	public abstract void Init(bool enableLogging);

	public abstract void SendEvent(string eventId);

	public abstract void SendRevenueEvent(string eventId, int productId, double localPrice, string priceCurrency);
}
