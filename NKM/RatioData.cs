namespace NKM;

public struct RatioData
{
	public float ratio_SSR;

	public float ratio_SR;

	public float ratio_R;

	public float ratio_N;

	public RatioData(float ssr, float sr, float r, float n)
	{
		ratio_SSR = ssr;
		ratio_SR = sr;
		ratio_R = r;
		ratio_N = n;
	}
}
