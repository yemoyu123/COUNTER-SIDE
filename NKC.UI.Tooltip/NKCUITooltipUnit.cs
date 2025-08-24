using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Tooltip;

public class NKCUITooltipUnit : NKCUITooltipShip
{
	[Header("유닛 타입")]
	public Image m_roleIcon;

	public Text m_roleText;

	public Image m_attackTypeIcon;

	public Text m_attackTypeText;

	public override void SetData(NKCUITooltip.Data data)
	{
		if (data is NKCUITooltip.UnitData { UnitTempletBase: { } unitTempletBase })
		{
			base.SetData(data);
			Sprite orLoadUnitRoleIcon = NKCResourceUtility.GetOrLoadUnitRoleIcon(unitTempletBase, bSmall: true);
			if (orLoadUnitRoleIcon != null)
			{
				NKCUtil.SetGameobjectActive(m_roleIcon, bValue: true);
				NKCUtil.SetGameobjectActive(m_roleText, bValue: true);
				NKCUtil.SetImageSprite(m_roleIcon, orLoadUnitRoleIcon);
				NKCUtil.SetLabelText(m_roleText, NKCUtilString.GetRoleText(unitTempletBase));
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_roleIcon, bValue: false);
				NKCUtil.SetGameobjectActive(m_roleText, bValue: false);
			}
			Sprite orLoadUnitAttackTypeIcon = NKCResourceUtility.GetOrLoadUnitAttackTypeIcon(unitTempletBase, bSmall: true);
			if (orLoadUnitAttackTypeIcon != null)
			{
				NKCUtil.SetGameobjectActive(m_attackTypeIcon, bValue: true);
				NKCUtil.SetGameobjectActive(m_attackTypeText, bValue: true);
				NKCUtil.SetImageSprite(m_attackTypeIcon, orLoadUnitAttackTypeIcon, bDisableIfSpriteNull: true);
				NKCUtil.SetLabelText(m_attackTypeText, NKCUtilString.GetAtkTypeText(unitTempletBase));
			}
			else
			{
				NKCUtil.SetGameobjectActive(m_attackTypeIcon, bValue: false);
				NKCUtil.SetGameobjectActive(m_attackTypeText, bValue: false);
			}
		}
	}
}
