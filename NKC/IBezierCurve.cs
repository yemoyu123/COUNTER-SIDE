using UnityEngine;

namespace NKC;

public interface IBezierCurve
{
	Vector3 GetPosition(float t);
}
