using BehaviorDesigner.Runtime.Tasks;
using NKC.Office;

namespace NKC.BT.Office;

public abstract class BTOfficeConditionBase : Conditional
{
	protected NKCOfficeCharacter m_Character;

	protected NKCOfficeBuildingBase m_OfficeBuilding;

	public override void OnAwake()
	{
		m_Character = GetComponent<NKCOfficeCharacter>();
		m_OfficeBuilding = m_Character?.OfficeBuilding;
	}
}
