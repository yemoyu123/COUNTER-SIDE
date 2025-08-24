namespace NKM;

public class NKMQuadraticBezierCurve : IBezierCurve
{
	private NKMVector3 P0;

	private NKMVector3 P1;

	private NKMVector3 P2;

	private NKMQuadraticBezierCurve()
	{
	}

	public NKMQuadraticBezierCurve(NKMVector3 p0, NKMVector3 p1, NKMVector3 p2)
	{
		P0 = p0;
		P1 = p1;
		P2 = p2;
	}

	public NKMVector3 GetPosition(float t)
	{
		float num = 1f - t;
		return num * num * P0 + 2f * t * num * P1 + t * t * P2;
	}

	public static NKMVector3 GetPosition(float t, NKMVector3 p0, NKMVector3 p1, NKMVector3 p2)
	{
		float num = 1f - t;
		return num * num * p0 + 2f * t * num * p1 + t * t * p2;
	}
}
