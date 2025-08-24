using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC.UI;

[RequireComponent(typeof(Text))]
public class NKCUIComSpecialStringChanger : MonoBehaviour
{
	public enum eStringType
	{
		OperatorSkill,
		Buff
	}

	public eStringType m_eType;

	public string m_targetStrID;

	public int m_targetLevel = 1;

	public int m_targetTimeLevel = 1;

	public bool m_bTranslateAtStart;

	public string m_Key;

	public void Translate()
	{
		Text component = GetComponent<Text>();
		if (!(component == null))
		{
			string msg;
			switch (m_eType)
			{
			case eStringType.OperatorSkill:
				msg = NKCOperatorUtil.MakeOperatorSkillDesc(NKCOperatorUtil.GetSkillTemplet(m_targetStrID), m_targetLevel);
				break;
			case eStringType.Buff:
			{
				NKMBuffTemplet buffTempletByStrID = NKMBuffManager.GetBuffTempletByStrID(m_targetStrID);
				msg = NKCUtilString.ApplyBuffValueToString(NKCStringTable.GetString(m_Key), buffTempletByStrID, m_targetLevel, m_targetTimeLevel);
				break;
			}
			default:
				msg = "";
				break;
			}
			NKCUtil.SetLabelText(component, msg);
		}
	}

	private void Awake()
	{
		if (!m_bTranslateAtStart)
		{
			Translate();
		}
	}

	private void Start()
	{
		if (m_bTranslateAtStart)
		{
			Translate();
		}
	}
}
