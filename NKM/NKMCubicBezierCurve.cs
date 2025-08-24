namespace NKM;

public class NKMCubicBezierCurve : IBezierCurve
{
	private NKMVector3 P0;

	private NKMVector3 P1;

	private NKMVector3 P2;

	private NKMVector3 P3;

	private NKMCubicBezierCurve()
	{
	}

	public NKMCubicBezierCurve(NKMVector3 p0, NKMVector3 p1, NKMVector3 p2, NKMVector3 p3)
	{
		P0 = p0;
		P1 = p1;
		P2 = p2;
		P3 = p3;
	}

	public NKMVector3 GetPosition(float t)
	{
		float num = 1f - t;
		return num * num * num * P0 + 3f * num * num * t * P1 + 3f * num * t * t * P2 + t * t * t * P3;
	}

	public static NKMVector3 GetPosition(float t, NKMVector3 p0, NKMVector3 p1, NKMVector3 p2, NKMVector3 p3)
	{
		float num = 1f - t;
		return num * num * num * p0 + 3f * num * num * t * p1 + 3f * num * t * t * p2 + t * t * t * p3;
	}
}
