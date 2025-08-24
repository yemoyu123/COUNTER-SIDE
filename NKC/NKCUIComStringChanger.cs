using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace NKC;

public class NKCUIComStringChanger : MonoBehaviour
{
	public bool m_bTranslateAtStart;

	public List<TargetStringInfoToChange> m_lstTargetStringInfoToChange = new List<TargetStringInfoToChange>();

	public void Translate()
	{
		for (int i = 0; i < m_lstTargetStringInfoToChange.Count; i++)
		{
			TargetStringInfoToChange targetStringInfoToChange = m_lstTargetStringInfoToChange[i];
			NKCUtil.SetLabelText(targetStringInfoToChange.m_lbText, NKCStringTable.GetString(targetStringInfoToChange.m_Key));
			if (targetStringInfoToChange.m_doTweenAni != null && targetStringInfoToChange.m_doTweenAni.targetType == DOTweenAnimation.TargetType.Text)
			{
				targetStringInfoToChange.m_doTweenAni.endValueString = NKCStringTable.GetString(targetStringInfoToChange.m_Key);
			}
			if (targetStringInfoToChange.m_inputField != null)
			{
				targetStringInfoToChange.m_inputField.text = NKCStringTable.GetString(targetStringInfoToChange.m_Key);
			}
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
