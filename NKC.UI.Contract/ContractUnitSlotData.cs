using NKM.Templet;

namespace NKC.UI.Contract;

public class ContractUnitSlotData
{
	public int UnitID;

	public NKM_UNIT_GRADE grade { get; }

	public NKM_UNIT_STYLE_TYPE type { get; }

	public NKM_UNIT_ROLE_TYPE role { get; }

	public string Name { get; }

	public float Percent { get; }

	public bool RatioUp { get; }

	public bool Awaken { get; }

	public bool Rearm { get; }

	public bool Pickup { get; }

	public ContractUnitSlotData(int _unitID, NKM_UNIT_GRADE _grade, NKM_UNIT_STYLE_TYPE _type, NKM_UNIT_ROLE_TYPE _role, bool _Awaken, bool _Rearm, string _name, float _percent, bool _ratioUp, bool _pickup)
	{
		UnitID = _unitID;
		grade = _grade;
		type = _type;
		role = _role;
		Name = _name;
		Percent = _percent;
		Awaken = _Awaken;
		Rearm = _Rearm;
		RatioUp = _ratioUp;
		Pickup = _pickup;
	}
}
