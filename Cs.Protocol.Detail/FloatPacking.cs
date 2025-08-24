namespace Cs.Protocol.Detail;

internal static class FloatPacking
{
	public static uint FloatToLow(this float data)
	{
		return (uint)(data * 100f);
	}

	public static float LowToFloat(this uint data)
	{
		return (float)(int)data * 0.01f;
	}
}
