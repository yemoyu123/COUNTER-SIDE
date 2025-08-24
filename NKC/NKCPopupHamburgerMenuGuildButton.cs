using NKC.UI;
using NKC.UI.Guild;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCPopupHamburgerMenuGuildButton : NKCPopupHamburgerMenuSimpleButton
{
	public GameObject m_objGuild;

	public NKCUIGuildBadge m_BadgeUI;

	public Text m_lbGuildName;

	public void SetGuildData()
	{
		if (m_objGuild != null)
		{
			NKCUtil.SetGameobjectActive(m_objGuild, NKCGuildManager.HasGuild());
			if (m_objGuild.activeSelf)
			{
				m_BadgeUI.SetData(NKCGuildManager.MyGuildData.badgeId, bOpponent: false);
				NKCUtil.SetLabelText(m_lbGuildName, NKCUtilString.GetUserGuildName(NKCGuildManager.MyGuildData.name, bOpponent: false));
			}
		}
	}
}
