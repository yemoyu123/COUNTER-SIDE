namespace NKC.Office;

public class NKCOfficeFloor : NKCOfficeFloorBase
{
	public override bool GetFunitureInvert(NKCOfficeFunitureData funitureData)
	{
		return funitureData.bInvert;
	}

	protected override bool GetFunitureInvert()
	{
		return false;
	}
}
