using System.Text;
using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI.Component;

public class NKCUIDeckConditionSlot : MonoBehaviour
{
	public Image m_imgMark;

	public Text m_lbCondition;

	public Sprite m_spNormal;

	public Color m_colNormal;

	public Sprite m_spForbidden;

	public Color m_colForbidden;

	public void SetCondition(NKMDeckCondition.EventDeckCondition condition)
	{
		if (condition.IsProhibited())
		{
			NKCUtil.SetImageSprite(m_imgMark, m_spForbidden);
			NKCUtil.SetLabelTextColor(m_lbCondition, m_colForbidden);
		}
		else
		{
			NKCUtil.SetImageSprite(m_imgMark, m_spNormal);
			NKCUtil.SetLabelTextColor(m_lbCondition, m_colNormal);
		}
		NKCUtil.SetLabelText(m_lbCondition, NKCUtilString.GetDeckConditionString(condition));
	}

	public void SetCondition(NKMDeckCondition.EventDeckCondition condition, int teamTotalCount)
	{
		bool num = condition.IsProhibited();
		if (num)
		{
			NKCUtil.SetImageSprite(m_imgMark, m_spForbidden);
			NKCUtil.SetLabelTextColor(m_lbCondition, m_colForbidden);
		}
		else
		{
			NKCUtil.SetImageSprite(m_imgMark, m_spNormal);
			NKCUtil.SetLabelTextColor(m_lbCondition, m_colNormal);
		}
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append(NKCUtilString.GetDeckConditionString(condition));
		if (!num)
		{
			stringBuilder.Append(" ");
			if (!condition.IsValueOk(teamTotalCount))
			{
				stringBuilder.AppendFormat("(<color=#ff0000>{0}</color>/{1})", teamTotalCount, condition.Value);
			}
			else
			{
				stringBuilder.AppendFormat("({0}/{1})", teamTotalCount, condition.Value);
			}
		}
		NKCUtil.SetLabelText(m_lbCondition, stringBuilder.ToString());
	}

	public void SetCondition(NKMDeckCondition.GameCondition condition)
	{
		NKCUtil.SetImageSprite(m_imgMark, m_spNormal);
		if (condition.IsPenalty())
		{
			NKCUtil.SetLabelTextColor(m_lbCondition, m_colForbidden);
		}
		else
		{
			NKCUtil.SetLabelTextColor(m_lbCondition, m_colNormal);
		}
		NKCUtil.SetLabelText(m_lbCondition, NKCUtilString.GetGameConditionString(condition));
	}
}
