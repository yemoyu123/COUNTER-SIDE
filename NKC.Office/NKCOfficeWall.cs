using UnityEngine;
using UnityEngine.UI;

namespace NKC.Office;

[RequireComponent(typeof(RectTransform), typeof(Image))]
public class NKCOfficeWall : NKCOfficeFloorBase
{
	public bool bInvertRequired;

	public override bool GetFunitureInvert(NKCOfficeFunitureData funitureData)
	{
		return bInvertRequired;
	}

	protected override bool GetFunitureInvert()
	{
		return bInvertRequired;
	}
}
