using NKM.Guild;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Guild;

public class NKCPopupGuildLvInfoSlot : MonoBehaviour
{
	public Text m_lbLevel;

	public Text m_lbMember;

	public Text m_lbPoint;

	public Text m_lbBonus;

	public void SetData(GuildExpTemplet expTemplet)
	{
		NKCUtil.SetLabelText(m_lbLevel, expTemplet.GuildLevel.ToString());
		NKCUtil.SetLabelText(m_lbMember, expTemplet.MaxMemberCount.ToString());
		NKCUtil.SetLabelText(m_lbPoint, expTemplet.WelfarePoint.ToString());
		NKCUtil.SetLabelText(m_lbBonus, "");
	}
}
