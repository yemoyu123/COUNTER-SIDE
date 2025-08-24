using BehaviorDesigner.Runtime;
using NKC.UI.Component;
using UnityEngine;

namespace NKC.BT.Office;

public class BTOfficeEmotion : BTOfficeActionBase
{
	[Header("플레이할 애니 이름. 비어있으면 아래 eEmotion 사용")]
	public SharedString AnimName;

	[Header("확률. 100에서 100%")]
	public int Probability = 30;

	[Header("AnimName 비어있는 경우 이쪽 사용")]
	public NKCUIComCharacterEmotion.Type eEmotion;

	[Header("애니 속도")]
	public float m_fSpeed = 1f;

	public override void OnStart()
	{
		if (m_Character == null || m_OfficeBuilding == null)
		{
			bActionSuccessFlag = false;
			return;
		}
		bActionSuccessFlag = true;
		if (Random.Range(0, 100) < Probability)
		{
			if (string.IsNullOrEmpty(AnimName.Value))
			{
				m_Character.PlayEmotion(eEmotion, m_fSpeed);
			}
			else
			{
				m_Character.PlayEmotion(AnimName.Value, m_fSpeed);
			}
		}
	}
}
