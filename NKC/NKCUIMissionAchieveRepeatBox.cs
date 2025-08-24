using NKM;
using UnityEngine;
using UnityEngine.UI;

namespace NKC;

public class NKCUIMissionAchieveRepeatBox : MonoBehaviour
{
	public enum REPEAT_BOX_STATE
	{
		BASE,
		INTRO,
		IDLE,
		COMPLETE
	}

	public delegate void OnButton(NKMMissionTemplet missionTemplet);

	public Animator m_Ani;

	public Image m_imgRequirePoint;

	public Text m_lbRequirePoint;

	public NKCUIComStateButton m_btn;

	public OnButton m_dOnButton;

	public void SetData(NKMMissionTemplet missionTemplet, OnButton onButton)
	{
		NKMUserData nKMUserData = NKCScenManager.CurrentUserData();
		if (nKMUserData == null)
		{
			return;
		}
		NKMMissionTabTemplet missionTabTemplet = NKMMissionManager.GetMissionTabTemplet(missionTemplet.m_MissionTabId);
		if (missionTabTemplet != null)
		{
			m_dOnButton = onButton;
			m_btn.PointerClick.RemoveAllListeners();
			m_btn.PointerClick.AddListener(delegate
			{
				OnClick(missionTemplet);
			});
			NKCUtil.SetLabelText(m_lbRequirePoint, missionTemplet.m_Times.ToString());
			string text = string.Empty;
			switch (missionTabTemplet.m_MissionType)
			{
			case NKM_MISSION_TYPE.REPEAT_DAILY:
				text = "AB_UI_NKM_UI_MISSION_REPEAT_TOP_ICON_DAILY";
				break;
			case NKM_MISSION_TYPE.REPEAT_WEEKLY:
				text = "AB_UI_NKM_UI_MISSION_REPEAT_TOP_ICON_WEEKLY";
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				NKCUtil.SetImageSprite(m_imgRequirePoint, NKCResourceUtility.GetOrLoadAssetResource<Sprite>("AB_UI_NKM_UI_MISSION_SPRITE", text));
			}
			NKMMissionData missionData = nKMUserData.m_MissionData.GetMissionData(missionTemplet);
			if (missionData == null || NKMMissionManager.CheckCanReset(missionTemplet.m_ResetInterval, missionData) || missionData.times < missionTemplet.m_Times)
			{
				PlayAni(REPEAT_BOX_STATE.BASE);
			}
			else if (missionData.IsComplete || missionData.mission_id > missionTemplet.m_MissionID)
			{
				PlayAni(REPEAT_BOX_STATE.COMPLETE);
			}
			else if (missionData.times >= missionTemplet.m_Times)
			{
				PlayAni(REPEAT_BOX_STATE.INTRO);
			}
			else
			{
				PlayAni(REPEAT_BOX_STATE.BASE);
			}
		}
	}

	private void PlayAni(REPEAT_BOX_STATE state)
	{
		switch (state)
		{
		case REPEAT_BOX_STATE.BASE:
			m_Ani.Play("NKM_UI_MISSION_REPEAT_REWARD_BOX_BASE");
			break;
		case REPEAT_BOX_STATE.INTRO:
		case REPEAT_BOX_STATE.IDLE:
			m_Ani.Play("NKM_UI_MISSION_REPEAT_REWARD_BOX_TOUCH_INTRO");
			break;
		case REPEAT_BOX_STATE.COMPLETE:
			m_Ani.Play("NKM_UI_MISSION_REPEAT_REWARD_BOX_COMPLETE");
			break;
		}
	}

	public void OnClick(NKMMissionTemplet missionTemplet)
	{
		m_dOnButton?.Invoke(missionTemplet);
	}
}
