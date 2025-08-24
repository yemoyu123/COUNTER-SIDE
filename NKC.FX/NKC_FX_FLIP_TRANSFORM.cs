using UnityEngine;

namespace NKC.FX;

public class NKC_FX_FLIP_TRANSFORM : MonoBehaviour
{
	public void Execute()
	{
		base.transform.localRotation = Quaternion.Euler(0f, base.transform.localEulerAngles.y - 180f, 0f);
	}
}
