using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Tooltip;

public class NKCUITooltipSkill : NKCUITooltipBase
{
	public NKCUIShipSkillSlot m_slot;

	public Text m_type;

	public Text m_name;

	public GameObject m_objCoolTime;

	public Image m_imgCoolTime;

	public Text m_lbCoolTime;

	public override void Init()
	{
		m_slot.Init(null);
	}

	public override void SetData(NKCUITooltip.Data data)
	{
		if (!(data is NKCUITooltip.ShipSkillData { ShipSkillTemplet: var shipSkillTemplet }))
		{
			Debug.LogError("Tooltip ShipSkillData is null");
			return;
		}
		m_slot.SetData(shipSkillTemplet);
		m_type.text = NKCUtilString.GetSkillTypeName(shipSkillTemplet.m_NKM_SKILL_TYPE);
		m_name.text = shipSkillTemplet.GetName();
		if (shipSkillTemplet.m_NKM_SKILL_TYPE == NKM_SKILL_TYPE.NST_SHIP_ACTIVE && shipSkillTemplet.m_fCooltimeSecond > 0f)
		{
			NKCUtil.SetGameobjectActive(m_objCoolTime, bValue: true);
			NKCUtil.SetImageSprite(m_imgCoolTime, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_POPUP_OK_CANCEL_BOX_SPRITE", "NKM_UI_COMMON_ICON_TIME"));
			NKCUtil.SetLabelText(m_lbCoolTime, string.Format(NKCUtilString.GET_STRING_TIME_SECOND_ONE_PARAM, shipSkillTemplet.m_fCooltimeSecond));
		}
		else
		{
			NKCUtil.SetGameobjectActive(m_objCoolTime, bValue: false);
		}
	}
}
