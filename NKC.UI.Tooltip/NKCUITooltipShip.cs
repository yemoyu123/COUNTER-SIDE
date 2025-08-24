using NKM.Templet;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Tooltip;

public class NKCUITooltipShip : NKCUITooltipBase
{
	public NKCUISlot m_slot;

	public Text m_name;

	public Text m_counter;

	public Image m_grade;

	[Header("등급")]
	public Sprite m_spSSR;

	public Sprite m_spSR;

	public Sprite m_spR;

	public Sprite m_spN;

	public override void Init()
	{
		m_slot.Init();
	}

	public override void SetData(NKCUITooltip.Data data)
	{
		if (!(data is NKCUITooltip.UnitData { UnitTempletBase: var unitTempletBase } unitData))
		{
			Debug.LogError("Tooltip Unit Data is null");
			return;
		}
		m_slot.SetData(unitData.Slot, bShowName: false, bShowNumber: false, bEnableLayoutElement: false, null);
		m_name.text = NKCUISlot.GetName(unitData.Slot.eType, unitData.Slot.ID);
		m_counter.text = NKCUtilString.GetUnitStyleName(unitTempletBase.m_NKM_UNIT_STYLE_TYPE);
		NKCUtil.SetImageSprite(m_grade, GetGradeSprite(unitTempletBase.m_NKM_UNIT_GRADE), bDisableIfSpriteNull: true);
	}

	private Sprite GetGradeSprite(NKM_UNIT_GRADE unitGrade)
	{
		return unitGrade switch
		{
			NKM_UNIT_GRADE.NUG_N => m_spN, 
			NKM_UNIT_GRADE.NUG_R => m_spR, 
			NKM_UNIT_GRADE.NUG_SR => m_spSR, 
			NKM_UNIT_GRADE.NUG_SSR => m_spSSR, 
			_ => null, 
		};
	}
}
